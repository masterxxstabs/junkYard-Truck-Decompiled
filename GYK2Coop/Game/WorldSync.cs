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
            if (!PlayerActedRecently)
                return; // Not something the player did (story, simulation, content unload...).
            pending.Add(new PendingOp
            {
                IsRemove = true,
                Guid = g,
                SceneId = scene.id,
                DefId = GameBridge.WgoDefId(w),
                Position = w.Position,
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
            if (pending.Count == 0)
                return;

            float now = Time.unscaledTime;
            int removes = 0;
            foreach (PendingOp op in pending)
                if (op.IsRemove)
                    removes++;
            while (recentLocalRemoves.Count > 0 && now - recentLocalRemoves.Peek() > BurstWindow)
                recentLocalRemoves.Dequeue();
            bool burst = removes > 0 && recentLocalRemoves.Count + removes > BurstLimit;
            if (burst)
                CoopPlugin.Log.LogWarning("Not mirroring " + removes + " removals at once (looks like the game, not the player).");

            foreach (PendingOp op in pending)
            {
                try
                {
                    if (!op.IsRemove)
                    {
                        if (GameBridge.FindWgo(op.Guid) == op.Wgo)
                            SendAdd(op.Wgo, op.SceneId);
                    }
                    else if (!burst)
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
                catch (Exception e)
                {
                    CoopPlugin.Log.LogWarning("World sync send failed: " + e.Message);
                }
            }
            pending.Clear();
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
