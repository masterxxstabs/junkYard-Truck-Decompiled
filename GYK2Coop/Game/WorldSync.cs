using System;
using System.Collections.Generic;
using System.IO;
using GYK2Coop.Net;
using HarmonyLib;
using UnityEngine;

namespace GYK2Coop.Game
{
    /// <summary>
    /// Mirrors world objects (WgoData: trees, rocks, buildings, chests, crafting stations,
    /// garden beds...) between the two games.
    ///
    /// Both games keep simulating the same world, so only changes made near the player who
    /// caused them are sent, and incoming additions are matched against objects the receiver
    /// already has (same definition, same spot) so things both games spawn on their own are
    /// not doubled.
    /// </summary>
    internal static class WorldSync
    {
        private const float MatchDistance = 0.35f;
        private const int MaxObjectBytes = 512 * 1024;
        private const float WatchSeconds = 8f;
        private const float WatchInterval = 0.5f;

        private enum OpKind
        {
            Upsert,
            Remove,
        }

        private struct PendingOp
        {
            public OpKind Kind;
            public WgoData Wgo;
            public Guid Guid;
            public string SceneId;
            public string DefId;
            public Vector3 Position;
        }

        private class Watched
        {
            public WgoData Wgo;
            public float Until;
            public ulong Hash;
            public Vector3 LastPosition;
        }

        private static readonly List<PendingOp> pending = new List<PendingOp>();
        private static readonly Dictionary<Guid, Watched> watched = new Dictionary<Guid, Watched>();
        // Remote object id -> local object id, for objects matched by definition + position.
        private static readonly Dictionary<Guid, Guid> remoteToLocal = new Dictionary<Guid, Guid>();
        private static readonly HashSet<string> warnedDefs = new HashSet<string>();
        private static HashSet<string> excludedTypes;
        private static float nextWatchTick;

        internal static int ApplyingRemote;

        public static Action<byte[]> Send;
        public static bool Active;

        public static void Reset()
        {
            pending.Clear();
            watched.Clear();
            remoteToLocal.Clear();
            Active = false;
        }

        private static bool ShouldTrack(WgoData w, string sceneId)
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
            Vector3 p = GameBridge.PlayerVisualPosition;
            Vector3 d = w.Position - p;
            d.y = 0f;
            float r = CoopPlugin.SyncRadius.Value;
            return d.sqrMagnitude <= r * r;
        }

        // ------------------------------------------------------------ local changes (Harmony hooks)

        internal static void OnLocalAdd(GameSceneData scene, WgoData w)
        {
            if (!ShouldTrack(w, scene.id))
                return;
            // Serialize at end of frame: callers often set more fields after adding.
            pending.Add(new PendingOp { Kind = OpKind.Upsert, Wgo = w, Guid = GameBridge.WgoGuid(w), SceneId = scene.id });
        }

        internal static void OnLocalRemove(GameSceneData scene, WgoData w)
        {
            if (!ShouldTrack(w, scene.id))
                return;
            Guid g = GameBridge.WgoGuid(w);
            // Added and removed in the same frame: nobody needs to hear about it.
            int idx = pending.FindIndex(o => o.Kind == OpKind.Upsert && o.Guid == g);
            if (idx >= 0)
            {
                pending.RemoveAt(idx);
                return;
            }
            pending.Add(new PendingOp
            {
                Kind = OpKind.Remove,
                Guid = g,
                SceneId = scene.id,
                DefId = GameBridge.WgoDefId(w),
                Position = w.Position,
            });
            watched.Remove(g);
        }

        /// <summary>Called once per frame (LateUpdate) while playing together.</summary>
        public static void Flush()
        {
            if (!Active || Send == null)
            {
                pending.Clear();
                return;
            }

            if (pending.Count > 0)
            {
                foreach (PendingOp op in pending)
                {
                    try
                    {
                        if (op.Kind == OpKind.Upsert)
                        {
                            if (GameBridge.FindWgo(op.Guid) == op.Wgo)
                                SendUpsert(op.Wgo, op.SceneId, isNew: true);
                        }
                        else
                        {
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

            if (Time.unscaledTime >= nextWatchTick)
            {
                nextWatchTick = Time.unscaledTime + WatchInterval;
                TickWatched();
            }
        }

        /// <summary>
        /// Objects the local player is aiming at / working on get their full state re-sent
        /// whenever it changes (chest contents, chopping progress, crafting queues, crops...).
        /// </summary>
        private static void TickWatched()
        {
            if (!CoopPlugin.SyncWorldObjects.Value)
                return;
            float now = Time.unscaledTime;

            WgoData target = GameBridge.WgoUnderInteraction;
            if (target != null && ShouldTrack(target, target.WorldId))
            {
                Guid g = GameBridge.WgoGuid(target);
                if (watched.TryGetValue(g, out Watched existing))
                {
                    existing.Wgo = target;
                    existing.Until = now + WatchSeconds;
                }
                else
                {
                    byte[] b = TrySerialize(target);
                    if (b != null)
                        watched[g] = new Watched { Wgo = target, Until = now + WatchSeconds, Hash = Hash(b), LastPosition = target.Position };
                }
            }

            if (watched.Count == 0)
                return;
            var expired = new List<Guid>();
            foreach (KeyValuePair<Guid, Watched> kv in watched)
            {
                Watched wt = kv.Value;
                if (GameBridge.FindWgo(kv.Key) != wt.Wgo)
                {
                    expired.Add(kv.Key);
                    continue;
                }
                bool moved = (wt.Wgo.Position - wt.LastPosition).sqrMagnitude > 0.0001f;
                wt.LastPosition = wt.Wgo.Position;
                byte[] b = TrySerialize(wt.Wgo);
                if (b != null)
                {
                    ulong h = Hash(b);
                    if (moved)
                        wt.Hash = h; // Something that walks around: both games simulate it.
                    else if (h != wt.Hash)
                    {
                        wt.Hash = h;
                        SendUpsert(wt.Wgo, wt.Wgo.WorldId, isNew: false, bytes: b);
                    }
                }
                if (now > wt.Until)
                    expired.Add(kv.Key);
            }
            foreach (Guid g in expired)
                watched.Remove(g);
        }

        private static void SendUpsert(WgoData w, string sceneId, bool isNew, byte[] bytes = null)
        {
            if (bytes == null)
                bytes = TrySerialize(w);
            if (bytes == null)
                return;
            Guid g = GameBridge.WgoGuid(w);
            string def = GameBridge.WgoDefId(w);
            Vector3 pos = w.Position;
            Send(Protocol.Build(MsgType.WgoUpsert, wr =>
            {
                wr.Write(isNew);
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

        private static ulong Hash(byte[] b)
        {
            // FNV-1a 64
            ulong h = 14695981039346656037UL;
            for (int i = 0; i < b.Length; i++)
            {
                h ^= b[i];
                h *= 1099511628211UL;
            }
            return h;
        }

        // ------------------------------------------------------------ remote changes

        private static WgoData FindLocal(Guid remoteGuid, string defId, string sceneId, Vector3 pos)
        {
            WgoData w = GameBridge.FindWgo(remoteGuid);
            if (w != null)
                return w;
            if (remoteToLocal.TryGetValue(remoteGuid, out Guid local))
            {
                w = GameBridge.FindWgo(local);
                if (w != null)
                    return w;
            }
            w = GameBridge.FindWgoByDefAndPosition(defId, sceneId, pos, MatchDistance);
            if (w != null)
                remoteToLocal[remoteGuid] = GameBridge.WgoGuid(w);
            return w;
        }

        public static void ApplyUpsert(BinaryReader r)
        {
            bool isNew = r.ReadBoolean();
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

            WgoData existing = FindLocal(guid, defId, sceneId, pos);
            if (existing != null && isNew)
                return; // Both games already made this object; keep ours.

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
            if (incoming == null)
                return;

            ApplyingRemote++;
            try
            {
                if (existing != null)
                {
                    // Keep the local identity so anything else pointing at this object stays valid.
                    GameBridge.CopyWgoIdentity(existing, incoming);
                    GameSceneData oldScene = GameBridge.FindScene(existing.WorldId) ?? scene;
                    oldScene.RemoveWgoData(existing, false);
                }
                incoming.PrepareForGame();
                scene.AddWgoData(incoming, true);

                Guid localGuid = GameBridge.WgoGuid(incoming);
                if (localGuid != guid)
                    remoteToLocal[guid] = localGuid;
                if (watched.TryGetValue(localGuid, out Watched wt))
                {
                    // Don't bounce the remote state straight back.
                    wt.Wgo = incoming;
                    byte[] b = TrySerialize(incoming);
                    if (b != null)
                        wt.Hash = Hash(b);
                }
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Applying synced object '" + defId + "' failed: " + e.Message);
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
            Vector3 pos = r.ReadVec3();
            if (!Active || !GameBridge.InGame)
                return;

            WgoData existing = FindLocal(guid, defId, sceneId, pos);
            remoteToLocal.Remove(guid);
            if (existing == null)
                return;
            GameSceneData scene = GameBridge.FindScene(existing.WorldId);
            if (scene == null)
                return;

            ApplyingRemote++;
            try
            {
                watched.Remove(GameBridge.WgoGuid(existing));
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
            try
            {
                if (__result != null)
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
