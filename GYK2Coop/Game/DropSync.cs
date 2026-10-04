using System;
using System.Collections.Generic;
using System.IO;
using GYK2Coop.Net;
using HarmonyLib;
using UnityEngine;

namespace GYK2Coop.Game
{
    /// <summary>
    /// Mirrors items lying on the ground (DropData): bodies, dropped crates, loot.
    ///
    /// Bodies are always mirrored, wherever they appear. Each game still gets its own body
    /// deliveries, so both players see both bodies and either player can pick up, carry and
    /// work on either one. Other drops are only mirrored near the player who made them, and
    /// matched against identical drops at the same spot so drops both games spawn on their own
    /// are not doubled.
    /// </summary>
    internal static class DropSync
    {
        private const float MatchDistance = 0.35f;

        // Remote drop id -> local drop id, for drops matched by item + position.
        private static readonly Dictionary<Guid, Guid> remoteToLocal = new Dictionary<Guid, Guid>();
        private static readonly HashSet<string> warned = new HashSet<string>();

        public static void Reset()
        {
            remoteToLocal.Clear();
        }

        private static bool ShouldSend(DropData drop)
        {
            if (!WorldSync.Active || WorldSync.ApplyingRemote > 0 || drop == null || !CoopPlugin.SyncWorldObjects.Value || !GameBridge.InGame)
                return false;
            Item item = drop.Item;
            if (item == null || item.IsEmpty)
                return false;
            // Resource sparks / tech points fly to whoever made them; each player keeps their own.
            if (drop.IsResDrop)
                return false;
            if (GameBridge.ItemIsBody(item))
                return true;
            Vector3 d = drop.Position - GameBridge.PlayerVisualPosition;
            d.y = 0f;
            float r = CoopPlugin.SyncRadius.Value;
            return d.sqrMagnitude <= r * r;
        }

        // ------------------------------------------------------------ local changes

        internal static void OnLocalAdd(GameSceneData scene, DropData drop)
        {
            if (!ShouldSend(drop))
                return;
            Item item = drop.Item;
            byte[] bytes;
            try
            {
                bytes = GameSerializer.Serialize<Item>(item);
            }
            catch (Exception e)
            {
                string id = GameBridge.ItemDefId(item);
                if (warned.Add(id))
                    CoopPlugin.Log.LogWarning("Not syncing dropped '" + id + "': " + e.Message);
                return;
            }
            Guid g = GameBridge.ItemGuid(item);
            string def = GameBridge.ItemDefId(item);
            bool body = GameBridge.ItemIsBody(item);
            Vector3 pos = drop.Position;
            WorldSync.Send?.Invoke(Protocol.Build(MsgType.DropAdd, w =>
            {
                w.WriteStr(scene.id);
                w.Write(g.ToByteArray());
                w.WriteStr(def);
                w.Write(body);
                w.WriteVec3(pos);
                w.WriteBlob(bytes);
            }));
        }

        internal static void OnLocalRemove(GameSceneData scene, DropData drop)
        {
            // Same rules as adding: bodies anywhere, other drops near the player, never resource sparks.
            if (!ShouldSend(drop))
                return;
            Guid g = GameBridge.ItemGuid(drop.Item);
            string def = GameBridge.ItemDefId(drop.Item);
            Vector3 pos = drop.Position;
            WorldSync.Send?.Invoke(Protocol.Build(MsgType.DropRemove, w =>
            {
                w.WriteStr(scene.id);
                w.Write(g.ToByteArray());
                w.WriteStr(def);
                w.WriteVec3(pos);
            }));
        }

        // ------------------------------------------------------------ remote changes

        private static DropData FindDrop(GameSceneData scene, Guid g)
        {
            if (scene == null || g == Guid.Empty)
                return null;
            foreach (DropData d in scene.droppedItems)
                if (d?.Item != null && GameBridge.ItemGuid(d.Item) == g)
                    return d;
            foreach (DropData d in scene.queuedDrops)
                if (d?.Item != null && GameBridge.ItemGuid(d.Item) == g)
                    return d;
            return null;
        }

        private static DropData FindSimilar(GameSceneData scene, string defId, Vector3 pos)
        {
            float best = MatchDistance * MatchDistance;
            DropData found = null;
            foreach (DropData d in scene.droppedItems)
            {
                if (d?.Item == null || d.IsRemoving || GameBridge.ItemDefId(d.Item) != defId)
                    continue;
                float dist = (d.Position - pos).sqrMagnitude;
                if (dist <= best)
                {
                    best = dist;
                    found = d;
                }
            }
            return found;
        }

        private static DropData FindLocal(GameSceneData scene, Guid remote)
        {
            DropData d = FindDrop(scene, remote);
            if (d == null && remoteToLocal.TryGetValue(remote, out Guid local))
                d = FindDrop(scene, local);
            return d;
        }

        public static void ApplyAdd(BinaryReader r)
        {
            string sceneId = r.ReadString();
            var g = new Guid(r.ReadBytes(16));
            string defId = r.ReadString();
            bool body = r.ReadBoolean();
            Vector3 pos = r.ReadVec3();
            byte[] bytes = r.ReadBlob();
            if (!WorldSync.Active || !GameBridge.InGame || bytes == null)
                return;

            GameSceneData scene = GameBridge.FindScene(sceneId);
            if (scene == null || FindLocal(scene, g) != null)
                return;
            if (!body)
            {
                DropData twin = FindSimilar(scene, defId, pos);
                if (twin != null)
                {
                    remoteToLocal[g] = GameBridge.ItemGuid(twin.Item);
                    return;
                }
            }

            Item item;
            try
            {
                item = GameSerializer.Deserialize<Item>(bytes);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Could not read synced drop '" + defId + "': " + e.Message);
                return;
            }
            if (item == null || item.IsEmpty)
                return;

            WorldSync.ApplyingRemote++;
            try
            {
                List<string> loaded = GameBridge.LoadedSceneIds;
                if (loaded == null || loaded.Contains(sceneId))
                    scene.AddDrop(item, pos);
                else
                    scene.AddDropToQueue(item, pos);
                if (body)
                    WorldSync.BodiesDirty = true;
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Placing synced drop '" + defId + "' failed: " + e.Message);
            }
            finally
            {
                WorldSync.ApplyingRemote--;
            }
        }

        public static void ApplyRemove(BinaryReader r)
        {
            string sceneId = r.ReadString();
            var g = new Guid(r.ReadBytes(16));
            r.ReadString();
            r.ReadVec3();
            if (!WorldSync.Active || !GameBridge.InGame)
                return;

            GameSceneData scene = GameBridge.FindScene(sceneId);
            DropData d = FindLocal(scene, g);
            remoteToLocal.Remove(g);
            if (d == null || d.IsRemoving)
                return;

            WorldSync.ApplyingRemote++;
            try
            {
                if (d.Item != null && GameBridge.ItemDefId(d.Item) == GameBridge.CorpseItemId)
                    WorldSync.BodiesDirty = true;
                scene.RemoveDrop(d);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Removing synced drop failed: " + e.Message);
            }
            finally
            {
                WorldSync.ApplyingRemote--;
            }
        }
    }

    [HarmonyPatch(typeof(GameSceneData), nameof(GameSceneData.AddDrop), new[] { typeof(DropData) })]
    internal static class Patch_GameSceneData_AddDrop
    {
        private static void Postfix(GameSceneData __instance, DropData __result)
        {
            if (__result == null || !WorldSync.Active)
                return;
            try
            {
                DropSync.OnLocalAdd(__instance, __result);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("AddDrop hook: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(GameSceneData), nameof(GameSceneData.RemoveDrop))]
    internal static class Patch_GameSceneData_RemoveDrop
    {
        private static void Prefix(GameSceneData __instance, DropData drop, out bool __state)
        {
            // RemoveDrop also runs for drops already being removed; only report the first time.
            __state = WorldSync.Active && drop != null && !drop.IsRemoving
                && (__instance.droppedItems.Contains(drop) || __instance.queuedDrops.Contains(drop));
        }

        private static void Postfix(GameSceneData __instance, DropData drop, bool __state)
        {
            if (!__state)
                return;
            try
            {
                DropSync.OnLocalRemove(__instance, drop);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("RemoveDrop hook: " + e.Message);
            }
        }
    }
}
