using System;
using System.Collections.Generic;
using System.IO;
using GYK2Coop.Net;
using HarmonyLib;
using UnityEngine;

namespace GYK2Coop.Game
{
    /// <summary>
    /// One player at a time per object.
    ///
    /// While a player has an object's window open (chest, grave, workbench...) or has a craft
    /// running on it, the other game treats the object as locked: it can't be targeted (no
    /// highlight, no E, no working on it) and shows "in use" / craft progress above it. When the
    /// owner is done, the final contents (storage and craft output) are sent before the unlock,
    /// so nothing is edited in two places at once and crafts only run in one game.
    /// </summary>
    internal static class ObjectLocks
    {
        public enum Reason : byte
        {
            InUse = 1,
            Crafting = 2,
        }

        private const float HeartbeatInterval = 1f;
        private const float RemoteExpiry = 5f;

        private class RemoteLock
        {
            public WgoData Wgo;
            public Reason Reason;
            public float Progress;
            public int Queued;
            public float LastSeen;
        }

        private class LocalLock
        {
            public WgoData Wgo;
            public Reason Reason;
            public float LastSent;
        }

        // Objects the other player holds.
        private static readonly Dictionary<Guid, RemoteLock> remote = new Dictionary<Guid, RemoteLock>();
        // Objects we hold, keyed by our object id.
        private static readonly Dictionary<Guid, LocalLock> local = new Dictionary<Guid, LocalLock>();
        // Crafts that were started from this game (the player was using the object when the queue filled).
        private static readonly HashSet<Guid> ownedCrafts = new HashSet<Guid>();

        public static Action<byte[]> Send;
        public static Func<bool> CoopPanelOpen;
        public static string RemoteName = "";

        public static void Reset()
        {
            remote.Clear();
            local.Clear();
            ownedCrafts.Clear();
        }

        public static bool IsHeldLocally(WgoData w)
        {
            return w != null && local.Count > 0 && local.ContainsKey(GameBridge.WgoGuid(w));
        }

        public static bool IsLockedByRemote(WgoData w)
        {
            if (w == null || remote.Count == 0)
                return false;
            return remote.TryGetValue(GameBridge.WgoGuid(w), out RemoteLock l) && Time.unscaledTime - l.LastSeen <= RemoteExpiry;
        }

        private static bool HasCraftQueue(WgoData w)
        {
            try
            {
                CraftComponent c = w?.CraftComponent;
                return c != null && c.HasCraftsInQueue;
            }
            catch
            {
                return false;
            }
        }

        private static float CraftProgress(WgoData w, out int queued)
        {
            queued = 0;
            try
            {
                CraftComponent c = w.CraftComponent;
                if (c == null)
                    return 0f;
                queued = c.CraftElementsQueue?.Count ?? 0;
                return c.CurrentCraftElement != null ? Mathf.Clamp01(c.CurrentCraftElement.ProgressTimeNormalized) : 0f;
            }
            catch
            {
                return 0f;
            }
        }

        /// <summary>Called a few times a second while playing together.</summary>
        public static void Tick()
        {
            if (Send == null || !GameBridge.InGame)
                return;
            float now = Time.unscaledTime;

            // What should we hold right now?
            var wanted = new Dictionary<Guid, LocalLock>();
            WgoData target = GameBridge.WgoUnderInteraction;
            bool windowOpen = GameBridge.GameWindowOpen && !(CoopPanelOpen?.Invoke() ?? false);
            if (target != null && windowOpen && !IsLockedByRemote(target))
            {
                Guid g = GameBridge.WgoGuid(target);
                wanted[g] = new LocalLock { Wgo = target, Reason = Reason.InUse };
                if (HasCraftQueue(target))
                    ownedCrafts.Add(g);
            }
            if (target != null && !IsLockedByRemote(target) && HasCraftQueue(target) && GameBridge.PlayerIsWorkingOrBuilding)
                ownedCrafts.Add(GameBridge.WgoGuid(target));

            List<Guid> finished = null;
            foreach (Guid g in ownedCrafts)
            {
                WgoData w = GameBridge.FindWgo(g);
                if (w == null || !HasCraftQueue(w))
                {
                    (finished ?? (finished = new List<Guid>())).Add(g);
                    continue;
                }
                if (!wanted.ContainsKey(g))
                    wanted[g] = new LocalLock { Wgo = w, Reason = Reason.Crafting };
                else
                    wanted[g].Reason = Reason.Crafting;
            }
            if (finished != null)
                foreach (Guid g in finished)
                    ownedCrafts.Remove(g);

            // Release what we no longer need: final contents first, then the unlock.
            List<Guid> released = null;
            foreach (KeyValuePair<Guid, LocalLock> kv in local)
            {
                if (wanted.ContainsKey(kv.Key))
                    continue;
                (released ?? (released = new List<Guid>())).Add(kv.Key);
                if (GameBridge.FindWgo(kv.Key) == kv.Value.Wgo)
                    WorldSync.SendContentsNow(kv.Value.Wgo);
                Guid id = kv.Key;
                Send(Protocol.Build(MsgType.ObjectUnlock, w => w.Write(id.ToByteArray())));
            }
            if (released != null)
                foreach (Guid g in released)
                    local.Remove(g);

            // Take / refresh what we need.
            foreach (KeyValuePair<Guid, LocalLock> kv in wanted)
            {
                bool isNew = !local.TryGetValue(kv.Key, out LocalLock held);
                if (isNew)
                {
                    held = kv.Value;
                    held.LastSent = -100f;
                    local[kv.Key] = held;
                }
                bool reasonChanged = held.Reason != kv.Value.Reason;
                held.Reason = kv.Value.Reason;
                held.Wgo = kv.Value.Wgo;
                if (!isNew && !reasonChanged && now - held.LastSent < HeartbeatInterval)
                    continue;
                held.LastSent = now;
                float progress = CraftProgress(held.Wgo, out int queued);
                Guid id = kv.Key;
                Reason reason = held.Reason;
                Send(Protocol.Build(MsgType.ObjectLock, w =>
                {
                    w.Write(id.ToByteArray());
                    w.Write((byte)reason);
                    w.Write(progress);
                    w.Write(queued);
                }));
            }
        }

        public static void ApplyLock(BinaryReader r)
        {
            var g = new Guid(r.ReadBytes(16));
            var reason = (Reason)r.ReadByte();
            float progress = r.ReadSingle();
            int queued = r.ReadInt32();
            WgoData w = WorldSync.FindLocalById(g);
            if (w == null)
                return;
            Guid localId = GameBridge.WgoGuid(w);
            // Both took it in the same instant: the host keeps it.
            if (local.ContainsKey(localId) && CoopRunnerIsHost)
                return;
            if (!remote.TryGetValue(localId, out RemoteLock l))
                remote[localId] = l = new RemoteLock();
            l.Wgo = w;
            l.Reason = reason;
            l.Progress = progress;
            l.Queued = queued;
            l.LastSeen = Time.unscaledTime;
        }

        public static void ApplyUnlock(BinaryReader r)
        {
            var g = new Guid(r.ReadBytes(16));
            WgoData w = WorldSync.FindLocalById(g);
            if (w != null)
                remote.Remove(GameBridge.WgoGuid(w));
            remote.Remove(g);
        }

        public static bool CoopRunnerIsHost;

        // ------------------------------------------------------------ labels

        private static GUIStyle style, shadow;

        /// <summary>Draws "in use" / craft progress over objects the other player holds.</summary>
        public static void DrawLabels()
        {
            if (remote.Count == 0)
                return;
            Camera cam = GameBridge.WorldCamera;
            if (cam == null)
                return;
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 13, fontStyle = FontStyle.Bold };
                style.normal.textColor = new Color(0.75f, 0.95f, 1f);
                shadow = new GUIStyle(style);
                shadow.normal.textColor = new Color(0, 0, 0, 0.85f);
            }
            float now = Time.unscaledTime;
            foreach (RemoteLock l in remote.Values)
            {
                if (l.Wgo == null || now - l.LastSeen > RemoteExpiry)
                    continue;
                Vector3 sp = cam.WorldToScreenPoint(l.Wgo.Position + Vector3.up * 2f);
                if (sp.z <= 0 || sp.x < -50 || sp.x > Screen.width + 50 || sp.y < -50 || sp.y > Screen.height + 50)
                    continue;
                string text = l.Reason == Reason.Crafting
                    ? RemoteName + " is crafting " + Mathf.RoundToInt(l.Progress * 100f) + "%" + (l.Queued > 1 ? " (+" + (l.Queued - 1) + " queued)" : "")
                    : "In use by " + RemoteName;
                var rect = new Rect(sp.x - 150, Screen.height - sp.y - 11, 300, 22);
                GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), text, shadow);
                GUI.Label(rect, text, style);
            }
        }
    }

    /// <summary>Objects the other player holds can't become the local player's interaction target.</summary>
    [HarmonyPatch(typeof(PlayerInteractionComponent), "GetWgoTargetsFromColliders")]
    internal static class Patch_PlayerInteraction_FilterLocked
    {
        private static void Postfix(List<Wgo> wgoTargets)
        {
            if (wgoTargets == null || wgoTargets.Count == 0)
                return;
            try
            {
                wgoTargets.RemoveAll(w => w != null && ObjectLocks.IsLockedByRemote(w.Data));
            }
            catch
            {
            }
        }
    }
}
