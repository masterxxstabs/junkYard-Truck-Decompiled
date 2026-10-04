using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GYK2Coop.Net;
using HarmonyLib;

namespace GYK2Coop.Game
{
    /// <summary>
    /// The host owns the story. Its quest statuses and objective arrow are copied to the guest so
    /// the guest sees the same quest markers ("!" over NPCs, the objective arrow, the quest list).
    /// While connected, the guest's own game can't start, finish or cancel quests (that would
    /// hand out rewards twice and drift from the host).
    /// </summary>
    internal static class QuestSync
    {
        private const float Interval = 1f;

        private static float nextSend;
        private static string lastKey;

        /// <summary>True on the guest while playing in the host's world.</summary>
        public static bool GuestLocked;
        private static bool applying;
        internal static bool BlockLocalChanges => GuestLocked && !applying;

        public static void Reset()
        {
            lastKey = null;
            nextSend = 0f;
            GuestLocked = false;
        }

        private static List<QuestData> Quests => MainGame.Instance?.GameSave?.questSystemData?.questCollection?.quests;

        /// <summary>Host: send quest state whenever it changes (checked once a second).</summary>
        public static void HostTick(Action<byte[]> send, float now)
        {
            if (now < nextSend)
                return;
            nextSend = now + Interval;
            List<QuestData> quests = Quests;
            if (quests == null)
                return;

            Guid arrow = GameBridge.ObjectiveArrowTarget;
            var sb = new StringBuilder(arrow.ToString("N"));
            var entries = new List<KeyValuePair<string, QuestData>>(quests.Count);
            foreach (QuestData q in quests)
            {
                if (q == null)
                    continue;
                string id = QuestId(q);
                if (string.IsNullOrEmpty(id))
                    continue;
                entries.Add(new KeyValuePair<string, QuestData>(id, q));
                sb.Append(id).Append((int)q.status).Append(q.isHidden ? 'h' : '-').Append(q.isUnknown ? 'u' : '-');
            }
            string key = sb.ToString();
            if (key == lastKey)
                return;
            lastKey = key;

            send(Protocol.Build(MsgType.QuestState, w =>
            {
                w.Write(arrow.ToByteArray());
                w.Write(entries.Count);
                foreach (KeyValuePair<string, QuestData> e in entries)
                {
                    w.Write(e.Key);
                    w.Write((byte)e.Value.status);
                    w.Write(e.Value.isHidden);
                    w.Write(e.Value.isUnknown);
                }
            }));
        }

        /// <summary>Host's quests changed while the guest was loading: make sure it gets a fresh copy.</summary>
        public static void ForceResend()
        {
            lastKey = null;
            nextSend = 0f;
        }

        private static System.Reflection.FieldInfo questIdField;

        private static string QuestId(QuestData q)
        {
            if (questIdField == null)
                questIdField = AccessTools.Field(typeof(QuestData), "id");
            return questIdField?.GetValue(q) as string;
        }

        /// <summary>Guest: copy the host's quest statuses (data only: no rewards, no scripts).</summary>
        public static void Apply(BinaryReader r)
        {
            var arrow = new Guid(r.ReadBytes(16));
            int n = r.ReadInt32();
            var states = new Dictionary<string, (QuestStatus status, bool hidden, bool unknown)>(n);
            for (int i = 0; i < n; i++)
            {
                string id = r.ReadString();
                var status = (QuestStatus)r.ReadByte();
                bool hidden = r.ReadBoolean();
                bool unknown = r.ReadBoolean();
                states[id] = (status, hidden, unknown);
            }
            if (!GameBridge.InGame)
                return;

            QuestCollectionData col = MainGame.Instance.GameSave.questSystemData?.questCollection;
            if (col?.questsCache == null)
                return;

            applying = true;
            try
            {
                bool statusChanged = false;
                foreach (KeyValuePair<string, (QuestStatus status, bool hidden, bool unknown)> kv in states)
                {
                    if (!col.questsCache.TryGetValue(kv.Key, out QuestData q) || q == null)
                        continue;
                    if (q.status != kv.Value.status)
                    {
                        q.status = kv.Value.status;
                        statusChanged = true;
                    }
                    q.isHidden = kv.Value.hidden;
                    q.isUnknown = kv.Value.unknown;
                }
                if (statusChanged && col.questStatusFilteredQuests != null)
                {
                    foreach (List<QuestData> list in col.questStatusFilteredQuests.Values)
                        list?.Clear();
                    foreach (QuestData q in col.quests)
                    {
                        if (q == null)
                            continue;
                        if (!col.questStatusFilteredQuests.TryGetValue(q.status, out List<QuestData> list) || list == null)
                            col.questStatusFilteredQuests[q.status] = list = new List<QuestData>();
                        list.Add(q);
                    }
                }
                GameBridge.SetObjectiveArrow(arrow);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Quest sync failed: " + e.Message);
            }
            finally
            {
                applying = false;
            }
        }
    }

    // While the guest is in the host's world, quests only move when the host's do.
    [HarmonyPatch]
    internal static class Patch_GuestQuestLock
    {
        private static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            foreach (string name in new[] { "StartQuest", "AwaitQuest", "CompleteQuest", "CancelQuest", "ResetQuestToAvailable" })
            {
                System.Reflection.MethodInfo m = AccessTools.Method(typeof(QuestSystemData), name);
                if (m != null)
                    yield return m;
            }
            foreach (string name in new[] { "Start", "Await", "Complete", "Cancel", "ResetToAvailable" })
            {
                System.Reflection.MethodInfo m = AccessTools.Method(typeof(QuestData), name);
                if (m != null)
                    yield return m;
            }
        }

        private static bool Prefix()
        {
            return !QuestSync.BlockLocalChanges;
        }
    }
}
