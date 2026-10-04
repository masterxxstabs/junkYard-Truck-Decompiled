using System;
using System.Collections.Generic;
using System.IO;
using GYK2Coop.Net;
using HarmonyLib;
using UnityEngine;

namespace GYK2Coop.Game
{
    /// <summary>
    /// Mirrors world objects (WgoData: trees, rocks, things built or torn down) between the two
    /// games.
    ///
    /// Safety rules (0.3.0, after 0.1/0.2 damaged a save):
    ///  - Objects that already exist are never replaced. Only brand-new objects are added.
    ///  - A removal is only sent when the local player has just been working or building, and
    ///    never in bursts (the game removes whole groups of objects when it unloads content).
    ///  - A removal is only applied to an object with exactly the same id and definition, and
    ///    never to an object that holds items or has a craft queued.
    ///  - Bursts of incoming removals are refused.
    /// </summary>
    internal static class WorldSync
    {
        private const float MatchDistance = 0.35f;
        private const int MaxObjectBytes = 512 * 1024;
        private const float PlayerActionWindow = 3f;
        private const int BurstLimit = 6;
        private const float BurstWindow = 2f;

        private struct PendingOp
        {
            public bool IsRemove;
            public WgoData Wgo;
            public Guid Guid;
            public string SceneId;
            public string DefId;
            public Vector3 Position;
            public bool PlayerCaused;
            public Guid OldGuid;
        }

        private static readonly List<PendingOp> pending = new List<PendingOp>();
        // Remote object id -> local object id, for objects both games created on their own.
        private static readonly Dictionary<Guid, Guid> remoteToLocal = new Dictionary<Guid, Guid>();
        private static readonly HashSet<string> warnedDefs = new HashSet<string>();
        private static readonly Queue<float> recentLocalRemoves = new Queue<float>();
        private static readonly Queue<float> recentRemoteRemoves = new Queue<float>();
        private static HashSet<string> excludedTypes;
        private static float lastPlayerAction = -100f;
        private static float remoteRemovesBlockedUntil;

        internal static int ApplyingRemote;

        public static Action<byte[]> Send;
        public static Action<string> Notice;
        public static bool Active;

        public static void Reset()
        {
            pending.Clear();
            remoteToLocal.Clear();
            recentLocalRemoves.Clear();
            recentRemoteRemoves.Clear();
            remoteRemovesBlockedUntil = 0f;
            watched.Clear();
            deferred.Clear();
            BodiesDirty = false;
            DropSync.Reset();
            Active = false;
        }

        /// <summary>Called every frame while playing; remembers when the player last worked or built.</summary>
        public static void TrackPlayerActivity()
        {
            if (GameBridge.PlayerIsWorkingOrBuilding)
                lastPlayerAction = Time.unscaledTime;
        }

        private static bool PlayerActedRecently => Time.unscaledTime - lastPlayerAction <= PlayerActionWindow;

        private static bool ShouldTrack(WgoData w)
        {
            if (!Active || ApplyingRemote > 0 || w == null || !CoopPlugin.SyncWorldObjects.Value || !GameBridge.InGame)
                return false;
            if (excludedTypes == null)
            {
                excludedTypes = new HashSet<string>();
                foreach (string s in CoopPlugin.ExcludedWgoTypes.Value.Split(','))
                    if (s.Trim().Length > 0)
                        excludedTypes.Add(s.Trim());
            }
            if (excludedTypes.Contains(w.GetType().Name))
                return false;
            // Walking NPCs and creatures are driven by each game's own simulation.
            MovementComponent mc = w.MovementComponent;
            if (mc != null && mc.IsMoving)
                return false;
            Vector3 d = w.Position - GameBridge.PlayerVisualPosition;
            d.y = 0f;
            float r = CoopPlugin.SyncRadius.Value;
            return d.sqrMagnitude <= r * r;
        }

        // ------------------------------------------------------------ local changes (Harmony hooks)

        internal static void OnLocalAdd(GameSceneData scene, WgoData w)
        {
            if (!ShouldTrack(w))
                return;
            // Serialize at end of frame: callers often set more fields after adding.
            pending.Add(new PendingOp { Wgo = w, Guid = GameBridge.WgoGuid(w), SceneId = scene.id });
        }

        internal static void OnLocalRemove(GameSceneData scene, WgoData w)
        {
            if (!ShouldTrack(w))
                return;
            Guid g = GameBridge.WgoGuid(w);
            // Added and removed in the same frame: nobody needs to hear about it.
            int idx = pending.FindIndex(o => !o.IsRemove && o.Guid == g);
            if (idx >= 0)
            {
                pending.RemoveAt(idx);
                return;
            }
            // Always recorded: the game changes objects in place (empty grave -> grave with a body)
            // by removing and re-adding the same object, which Flush turns into a change.
            pending.Add(new PendingOp
            {
                IsRemove = true,
                Guid = g,
                SceneId = scene.id,
                DefId = GameBridge.WgoDefId(w),
                Position = w.Position,
                PlayerCaused = PlayerActedRecently || GameBridge.WgoUnderInteraction == w,
            });
        }

        /// <summary>Called once per frame (LateUpdate) while playing together.</summary>
        public static void Flush()
        {
            if (!Active || Send == null)
            {
                pending.Clear();
                return;
            }

            float now = Time.unscaledTime;
            if (now >= nextWatchTick)
            {
                nextWatchTick = now + WatchInterval;
                TickWatched();
                RetryDeferred();
            }
            if (pending.Count == 0)
                return;

            // Pair "removed X" + "added X again" (same id, same frame) into one in-place change.
            var changes = new List<PendingOp>();
            var adds = new List<PendingOp>();
            var removes = new List<PendingOp>();
            foreach (PendingOp op in pending)
            {
                if (op.IsRemove)
                {
                    removes.Add(op);
                    continue;
                }
                int r = removes.FindIndex(x => x.Guid == op.Guid);
                if (r >= 0)
                {
                    PendingOp change = op;
                    change.DefId = removes[r].DefId; // the definition before the change
                    change.OldGuid = op.Guid;
                    change.PlayerCaused = removes[r].PlayerCaused;
                    removes.RemoveAt(r);
                    changes.Add(change);
                }
                else
                {
                    adds.Add(op);
                }
            }
            pending.Clear();

            // ReplaceWgoData: "removed X" + "added a new Y on exactly the same spot" is also a change of X.
            for (int i = adds.Count - 1; i >= 0; i--)
            {
                PendingOp add = adds[i];
                Vector3 p = add.Wgo != null ? add.Wgo.Position : Vector3.zero;
                int r = removes.FindIndex(x => x.SceneId == add.SceneId && (x.Position - p).sqrMagnitude < 0.0025f);
                if (r < 0)
                    continue;
                add.DefId = removes[r].DefId;
                add.OldGuid = removes[r].Guid;
                add.PlayerCaused = removes[r].PlayerCaused;
                removes.RemoveAt(r);
                adds.RemoveAt(i);
                changes.Add(add);
            }
            // Story, simulation (crops growing...) and content unloads happen in both games anyway.
            removes.RemoveAll(x => !x.PlayerCaused);
            changes.RemoveAll(x => !x.PlayerCaused);

            while (recentLocalRemoves.Count > 0 && now - recentLocalRemoves.Peek() > BurstWindow)
                recentLocalRemoves.Dequeue();
            bool burst = removes.Count > 0 && recentLocalRemoves.Count + removes.Count > BurstLimit;
            if (burst)
                CoopPlugin.Log.LogWarning("Not mirroring " + removes.Count + " removals at once (looks like the game, not the player).");

            foreach (PendingOp op in changes)
            {
                try
                {
                    if (GameBridge.FindWgo(op.Guid) == op.Wgo)
                        SendChange(op.Wgo, op.SceneId, op.DefId, op.OldGuid);
                }
                catch (Exception e)
                {
                    CoopPlugin.Log.LogWarning("World sync send failed: " + e.Message);
                }
            }
            foreach (PendingOp op in adds)
            {
                try
                {
                    if (GameBridge.FindWgo(op.Guid) == op.Wgo)
                        SendAdd(op.Wgo, op.SceneId);
                }
                catch (Exception e)
                {
                    CoopPlugin.Log.LogWarning("World sync send failed: " + e.Message);
                }
            }
            if (burst)
                return;
            foreach (PendingOp op in removes)
            {
                recentLocalRemoves.Enqueue(now);
                Send(Protocol.Build(MsgType.WgoRemove, w =>
                {
                    w.WriteStr(op.SceneId);
                    w.Write(op.Guid.ToByteArray());
                    w.WriteStr(op.DefId);
                    w.WriteVec3(op.Position);
                }));
            }
        }

        // ------------------------------------------------------------ in-place changes and contents

        private const float WatchSeconds = 8f;
        private const float WatchInterval = 0.5f;
        private const float DeferLimitSeconds = 300f;

        private class Watched
        {
            public WgoData Wgo;
            public float Until;
            public string ItemsKey;
        }

        private class Incoming
        {
            public bool IsChange;
            public Guid Guid;
            public Guid NewGuid;
            public string OldDef;
            public string NewDef;
            public List<byte[]> Items;
            public float ReceivedAt;
        }

        // Objects the local player used recently; their contents are re-sent when they change.
        private static readonly Dictionary<Guid, Watched> watched = new Dictionary<Guid, Watched>();
        // Changes that arrived while the local copy was busy. Latest per object wins.
        private static readonly Dictionary<Guid, Incoming> deferred = new Dictionary<Guid, Incoming>();
        private static float nextWatchTick;

        /// <summary>Set when something that affects the unburied-body count was applied.</summary>
        public static bool BodiesDirty;

        /// <summary>Cheap fingerprint of an object's contents: item ids, counts and identities.</summary>
        private static string ItemsKey(WgoData w)
        {
            List<Item> items = w.Inventory?.Data?.Inventory;
            if (items == null || items.Count == 0)
                return "";
            var sb = new System.Text.StringBuilder();
            foreach (Item it in items)
            {
                if (it == null || it.IsEmpty)
                    continue;
                sb.Append(GameBridge.ItemGuid(it).ToString("N")).Append(':').Append(GameBridge.ItemDefId(it)).Append(':').Append(it.Count).Append(';');
            }
            return sb.ToString();
        }

        private static List<byte[]> SerializeItems(WgoData w)
        {
            var list = new List<byte[]>();
            List<Item> items = w.Inventory?.Data?.Inventory;
            if (items == null)
                return list;
            foreach (Item it in items)
            {
                if (it == null || it.IsEmpty)
                    continue;
                list.Add(GameSerializer.Serialize<Item>(it));
            }
            return list;
        }

        private static void WriteItems(BinaryWriter w, List<byte[]> items)
        {
            w.Write(items.Count);
            foreach (byte[] b in items)
                w.WriteBlob(b);
        }

        private static List<byte[]> ReadItems(BinaryReader r)
        {
            int n = r.ReadInt32();
            var list = new List<byte[]>(Math.Max(0, Math.Min(n, 1024)));
            for (int i = 0; i < n; i++)
                list.Add(r.ReadBlob());
            return list;
        }

        private static void SendChange(WgoData w, string sceneId, string oldDef, Guid oldGuid)
        {
            List<byte[]> items;
            try
            {
                items = SerializeItems(w);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Not syncing change of '" + oldDef + "': " + e.Message);
                return;
            }
            Guid g = GameBridge.WgoGuid(w);
            string newDef = GameBridge.WgoDefId(w);
            Send(Protocol.Build(MsgType.WgoChange, wr =>
            {
                wr.WriteStr(sceneId);
                wr.Write(oldGuid.ToByteArray());
                wr.Write(g.ToByteArray());
                wr.WriteStr(oldDef);
                wr.WriteStr(newDef);
                WriteItems(wr, items);
            }));
            Watch(w, Time.unscaledTime);
        }

        private static void SendItems(WgoData w, List<byte[]> items)
        {
            Guid g = GameBridge.WgoGuid(w);
            string def = GameBridge.WgoDefId(w);
            Send(Protocol.Build(MsgType.WgoItems, wr =>
            {
                wr.Write(g.ToByteArray());
                wr.WriteStr(def);
                WriteItems(wr, items);
            }));
        }

        private static void Watch(WgoData w, float now)
        {
            Guid g = GameBridge.WgoGuid(w);
            if (watched.TryGetValue(g, out Watched wt))
            {
                wt.Wgo = w;
                wt.Until = now + WatchSeconds;
            }
            else
            {
                watched[g] = new Watched { Wgo = w, Until = now + WatchSeconds, ItemsKey = ItemsKey(w) };
            }
        }

        /// <summary>
        /// Contents of the object the local player is using (grave, chest...) are re-sent when they
        /// change. Never while a craft is queued on it: running crafts stay in one game.
        /// </summary>
        private static void TickWatched()
        {
            if (!CoopPlugin.SyncWorldObjects.Value)
                return;
            float now = Time.unscaledTime;
            WgoData target = GameBridge.WgoUnderInteraction;
            if (target != null && ShouldTrack(target))
                Watch(target, now);
            if (watched.Count == 0)
                return;

            List<Guid> expired = null;
            foreach (KeyValuePair<Guid, Watched> kv in watched)
            {
                Watched wt = kv.Value;
                if (GameBridge.FindWgo(kv.Key) != wt.Wgo)
                {
                    (expired ?? (expired = new List<Guid>())).Add(kv.Key);
                    continue;
                }
                if (HasCraftQueue(wt.Wgo))
                {
                    wt.Until = Mathf.Max(wt.Until, now + WatchSeconds);
                    continue;
                }
                string key = ItemsKey(wt.Wgo);
                if (key != wt.ItemsKey)
                {
                    try
                    {
                        List<byte[]> items = SerializeItems(wt.Wgo);
                        wt.ItemsKey = key;
                        SendItems(wt.Wgo, items);
                    }
                    catch (Exception e)
                    {
                        CoopPlugin.Log.LogWarning("Not syncing contents of '" + GameBridge.WgoDefId(wt.Wgo) + "': " + e.Message);
                        wt.ItemsKey = key;
                    }
                }
                if (now > wt.Until)
                    (expired ?? (expired = new List<Guid>())).Add(kv.Key);
            }
            if (expired != null)
                foreach (Guid g in expired)
                    watched.Remove(g);
        }

        public static void ApplyChange(BinaryReader r)
        {
            r.ReadString(); // scene id (the object is found by its id)
            var u = new Incoming
            {
                IsChange = true,
                Guid = new Guid(r.ReadBytes(16)),
                NewGuid = new Guid(r.ReadBytes(16)),
                OldDef = r.ReadString(),
                NewDef = r.ReadString(),
                Items = ReadItems(r),
                ReceivedAt = Time.unscaledTime,
            };
            Queue(u);
        }

        public static void ApplyItems(BinaryReader r)
        {
            var u = new Incoming
            {
                Guid = new Guid(r.ReadBytes(16)),
                NewDef = r.ReadString(),
                Items = ReadItems(r),
                ReceivedAt = Time.unscaledTime,
            };
            Queue(u);
        }

        private static void Queue(Incoming u)
        {
            if (!Active || !GameBridge.InGame)
                return;
            // A newer change supersedes an older waiting one, but keep "this was a change".
            if (deferred.TryGetValue(u.Guid, out Incoming old))
            {
                if (old.IsChange && !u.IsChange)
                {
                    old.Items = u.Items;
                    old.ReceivedAt = u.ReceivedAt;
                    u = old;
                }
                deferred.Remove(u.Guid);
            }
            if (!TryApply(u))
                deferred[u.Guid] = u;
        }

        private static void RetryDeferred()
        {
            if (deferred.Count == 0)
                return;
            float now = Time.unscaledTime;
            var done = new List<Guid>();
            foreach (KeyValuePair<Guid, Incoming> kv in deferred)
                if (now - kv.Value.ReceivedAt > DeferLimitSeconds || TryApply(kv.Value))
                    done.Add(kv.Key);
            foreach (Guid g in done)
                deferred.Remove(g);
        }

        /// <summary>True if changing this object now would pull it out from under the local game.</summary>
        private static bool IsBusyLocally(WgoData w)
        {
            if (HasCraftQueue(w))
                return true;
            return GameBridge.WgoUnderInteraction == w && GameBridge.PlayerIsWorkingOrBuilding;
        }

        /// <returns>false if it has to wait.</returns>
        private static bool TryApply(Incoming u)
        {
            WgoData local = GameBridge.FindWgo(u.Guid);
            if (local == null && remoteToLocal.TryGetValue(u.Guid, out Guid alias))
                local = GameBridge.FindWgo(alias);
            if (local == null)
                return true; // We don't have this object; nothing to change.

            string def = GameBridge.WgoDefId(local);
            bool needsChange = u.IsChange && def == u.OldDef && u.NewDef != u.OldDef;
            if (!needsChange && def != u.NewDef)
                return true; // Our copy went its own way; leave it alone.
            if (IsBusyLocally(local))
                return false;

            List<Item> items = new List<Item>();
            foreach (byte[] b in u.Items)
            {
                Item it;
                try
                {
                    it = b != null ? GameSerializer.Deserialize<Item>(b) : null;
                }
                catch (Exception e)
                {
                    CoopPlugin.Log.LogWarning("Could not read synced contents of '" + def + "': " + e.Message);
                    return true;
                }
                if (it != null && !it.IsEmpty)
                    items.Add(it);
            }

            ApplyingRemote++;
            try
            {
                if (needsChange)
                {
                    // The game's own in-place change: same object, new definition.
                    MainGame.Instance.GameSave.worldData.ChangeWgoData(local, u.NewDef);
                }
                ReplaceContents(local, items);
                BodiesDirty = true;
                // The other game replaced the object with a new one; remember which of ours it is.
                Guid localGuid = GameBridge.WgoGuid(local);
                if (u.IsChange && u.NewGuid != Guid.Empty && u.NewGuid != localGuid)
                    remoteToLocal[u.NewGuid] = localGuid;
                if (watched.TryGetValue(GameBridge.WgoGuid(local), out Watched wt))
                    wt.ItemsKey = ItemsKey(local); // don't echo it back
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Applying synced change to '" + def + "' failed: " + e.Message);
            }
            finally
            {
                ApplyingRemote--;
            }
            return true;
        }

        /// <summary>Makes the object's storage hold exactly <paramref name="items"/>, using the game's own add/remove so views update.</summary>
        private static void ReplaceContents(WgoData w, List<Item> items)
        {
            Inventory inv = w.Inventory;
            List<Item> current = inv?.Data?.Inventory;
            if (inv == null || current == null)
                return;

            var wanted = new Dictionary<Guid, Item>();
            foreach (Item it in items)
                wanted[GameBridge.ItemGuid(it)] = it;

            foreach (Item it in new List<Item>(current))
            {
                if (it == null || it.IsEmpty)
                    continue;
                Guid g = GameBridge.ItemGuid(it);
                if (wanted.TryGetValue(g, out Item same) && same.Count == it.Count && GameBridge.ItemDefId(same) == GameBridge.ItemDefId(it))
                    wanted.Remove(g); // already identical
                else
                    inv.RemoveItemFromInventoryByUID(it);
            }
            if (wanted.Count > 0)
                inv.AddItemsToInventory(new List<Item>(wanted.Values));
        }

        private static void SendAdd(WgoData w, string sceneId)
        {
            // Containers and crafting stations with contents are never sent: the items would then
            // exist in both games.
            if (GameBridge.HasStoredItems(w) || HasCraftQueue(w))
                return;
            byte[] bytes = TrySerialize(w);
            if (bytes == null)
                return;
            Guid g = GameBridge.WgoGuid(w);
            string def = GameBridge.WgoDefId(w);
            Vector3 pos = w.Position;
            Send(Protocol.Build(MsgType.WgoAdd, wr =>
            {
                wr.WriteStr(sceneId);
                wr.Write(g.ToByteArray());
                wr.WriteStr(def);
                wr.WriteVec3(pos);
                wr.WriteBlob(bytes);
            }));
        }

        private static byte[] TrySerialize(WgoData w)
        {
            string def = GameBridge.WgoDefId(w);
            try
            {
                byte[] b = GameSerializer.Serialize<WgoData>(w);
                if (b != null && b.Length > MaxObjectBytes)
                {
                    if (warnedDefs.Add(def))
                        CoopPlugin.Log.LogWarning("Not syncing '" + def + "': serialized size " + b.Length + " bytes is too large.");
                    return null;
                }
                return b;
            }
            catch (Exception e)
            {
                if (warnedDefs.Add(def))
                    CoopPlugin.Log.LogWarning("Not syncing '" + def + "': " + e.Message);
                return null;
            }
        }

        private static bool HasCraftQueue(WgoData w)
        {
            try
            {
                CraftComponent c = w.CraftComponent;
                return c != null && c.HasCraftsInQueue;
            }
            catch
            {
                return true;
            }
        }

        // ------------------------------------------------------------ remote changes

        public static void ApplyAdd(BinaryReader r)
        {
            string sceneId = r.ReadString();
            var guid = new Guid(r.ReadBytes(16));
            string defId = r.ReadString();
            Vector3 pos = r.ReadVec3();
            byte[] bytes = r.ReadBlob();
            if (!Active || !GameBridge.InGame || bytes == null)
                return;

            GameSceneData scene = GameBridge.FindScene(sceneId);
            if (scene == null)
                return;

            // Already have it (same id, or the same kind of object on the same spot): keep ours.
            if (GameBridge.FindWgo(guid) != null)
                return;
            WgoData twin = GameBridge.FindWgoByDefAndPosition(defId, sceneId, pos, MatchDistance);
            if (twin != null)
            {
                remoteToLocal[guid] = GameBridge.WgoGuid(twin);
                return;
            }

            WgoData incoming;
            try
            {
                incoming = GameSerializer.Deserialize<WgoData>(bytes);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Could not read synced object '" + defId + "': " + e.Message);
                return;
            }
            if (incoming == null || GameBridge.WgoDefId(incoming) != defId || !GameBridge.HasDefinition(incoming))
            {
                CoopPlugin.Log.LogWarning("Synced object '" + defId + "' arrived incomplete; ignored.");
                return;
            }
            if (GameBridge.HasStoredItems(incoming) || HasCraftQueue(incoming))
                return;

            ApplyingRemote++;
            try
            {
                incoming.PrepareForGame();
                scene.AddWgoData(incoming, true);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Adding synced object '" + defId + "' failed, rolling back: " + e.Message);
                try
                {
                    if (GameBridge.FindWgo(GameBridge.WgoGuid(incoming)) == incoming)
                        scene.RemoveWgoData(incoming, true);
                }
                catch
                {
                }
            }
            finally
            {
                ApplyingRemote--;
            }
        }

        public static void ApplyRemove(BinaryReader r)
        {
            string sceneId = r.ReadString();
            var guid = new Guid(r.ReadBytes(16));
            string defId = r.ReadString();
            r.ReadVec3();
            if (!Active || !GameBridge.InGame)
                return;

            float now = Time.unscaledTime;
            if (now < remoteRemovesBlockedUntil)
                return;
            while (recentRemoteRemoves.Count > 0 && now - recentRemoteRemoves.Peek() > BurstWindow)
                recentRemoteRemoves.Dequeue();
            recentRemoteRemoves.Enqueue(now);
            if (recentRemoteRemoves.Count > BurstLimit)
            {
                remoteRemovesBlockedUntil = now + 10f;
                CoopPlugin.Log.LogWarning("Refusing a burst of removals from the other player.");
                Notice?.Invoke("Blocked a burst of object removals from the other game. Your world was not changed.");
                return;
            }

            // Exact id only (or an object we matched ourselves when it was created). No guessing.
            WgoData existing = GameBridge.FindWgo(guid);
            if (existing == null && remoteToLocal.TryGetValue(guid, out Guid local))
                existing = GameBridge.FindWgo(local);
            remoteToLocal.Remove(guid);
            if (existing == null || GameBridge.WgoDefId(existing) != defId)
                return;
            if (GameBridge.HasStoredItems(existing) || HasCraftQueue(existing))
            {
                CoopPlugin.Log.LogInfo("Kept '" + defId + "': it holds items or a craft in this game.");
                return;
            }
            GameSceneData scene = GameBridge.FindScene(existing.WorldId);
            if (scene == null)
                return;

            ApplyingRemote++;
            try
            {
                scene.RemoveWgoData(existing, true);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Removing synced object '" + defId + "' failed: " + e.Message);
            }
            finally
            {
                ApplyingRemote--;
            }
        }
    }

    [HarmonyPatch(typeof(GameSceneData), nameof(GameSceneData.AddWgoData), new[] { typeof(WgoData), typeof(bool) })]
    internal static class Patch_GameSceneData_AddWgoData
    {
        private static void Postfix(GameSceneData __instance, WgoData wgoData)
        {
            if (!WorldSync.Active)
                return;
            try
            {
                WorldSync.OnLocalAdd(__instance, wgoData);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("AddWgoData hook: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(GameSceneData), nameof(GameSceneData.AddWgoData), new[] { typeof(string), typeof(Vector3), typeof(string), typeof(bool) })]
    internal static class Patch_GameSceneData_AddWgoDataById
    {
        private static void Postfix(GameSceneData __instance, WgoData __result)
        {
            if (!WorldSync.Active || __result == null)
                return;
            try
            {
                WorldSync.OnLocalAdd(__instance, __result);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("AddWgoData hook: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(GameSceneData), nameof(GameSceneData.RemoveWgoData))]
    internal static class Patch_GameSceneData_RemoveWgoData
    {
        private static void Prefix(WgoData wgoData, out bool __state)
        {
            __state = false;
            if (!WorldSync.Active)
                return;
            try
            {
                // RemoveWgoData is a no-op for objects that are not in the world.
                __state = wgoData != null && GameBridge.FindWgo(GameBridge.WgoGuid(wgoData)) == wgoData;
            }
            catch
            {
            }
        }

        private static void Postfix(GameSceneData __instance, WgoData wgoData, bool __state)
        {
            if (!__state)
                return;
            try
            {
                WorldSync.OnLocalRemove(__instance, wgoData);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("RemoveWgoData hook: " + e.Message);
            }
        }
    }
}
