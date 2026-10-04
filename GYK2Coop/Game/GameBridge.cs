using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace GYK2Coop.Game
{
    /// <summary>
    /// Everything that touches Graveyard Keeper 2 internals goes through here. Members whose
    /// types live in LazyBearTechnology.dll are reached by reflection, so the mod does not
    /// hard-bind to that engine library and a game patch breaks one lookup instead of the load.
    /// </summary>
    internal static class GameBridge
    {
        private static FieldInfo playerDataPosition;
        private static FieldInfo playerDataCharState;
        private static FieldInfo controllerCurrentScene;
        private static FieldInfo wgoUniqueIdField;
        private static FieldInfo wgoDefIdField;
        private static FieldInfo environmentDayField;
        private static readonly Dictionary<Type, PropertyInfo> valueProps = new Dictionary<Type, PropertyInfo>();
        private static readonly Dictionary<Type, MemberInfo> guidMembers = new Dictionary<Type, MemberInfo>();
        private static readonly Dictionary<Type, PropertyInfo> idProps = new Dictionary<Type, PropertyInfo>();

        public static MainGame Main => MainGame.Instance;

        public static bool InGame
        {
            get
            {
                MainGame m = MainGame.Instance;
                return m != null && m.gameState == MainGame.GameState.InGame && MainGame.PlayerController != null && m.GameSave != null;
            }
        }

        public static bool AtMainMenu
        {
            get
            {
                MainGame m = MainGame.Instance;
                return m != null && m.gameState == MainGame.GameState.MainMenu;
            }
        }

        // ---------------------------------------------------------------- player

        public static PlayerController Player => MainGame.PlayerController;

        public static PlayerData LocalPlayerData
        {
            get
            {
                MainGame m = MainGame.Instance;
                return m?.GameSave?.playerData;
            }
        }

        /// <summary>World position of the player's visual root (what the camera follows).</summary>
        public static Vector3 PlayerVisualPosition
        {
            get
            {
                PlayerController p = Player;
                if (p == null)
                    return Vector3.zero;
                PlayerView view = p.View;
                return view != null ? view.transform.position : p.transform.position;
            }
        }

        public static Vector2 PlayerDirection
        {
            get
            {
                PlayerData d = LocalPlayerData;
                return d != null ? d.Direction : Vector2.down;
            }
        }

        public static int PlayerAnimState
        {
            get
            {
                PlayerController p = Player;
                if (p == null || p.View == null || p.View.PlayerAnimation == null)
                    return 0;
                Animator a = p.View.PlayerAnimation.Animator;
                if (a == null || !a.isActiveAndEnabled)
                    return 0;
                return a.GetInteger(AnimationComponentBase.idStateAnimator);
            }
        }

        public static string PlayerSceneId
        {
            get
            {
                PlayerController p = Player;
                if (p == null)
                    return "";
                if (controllerCurrentScene == null)
                    controllerCurrentScene = AccessTools.Field(typeof(PlayerController), "currentGameScene");
                object scene = controllerCurrentScene?.GetValue(p);
                if (scene == null || (scene is UnityEngine.Object uo && uo == null))
                    return "";
                return GetIdProperty(scene) ?? "";
            }
        }

        public static void SetPlayerDataPosition(PlayerData data, Vector3 pos)
        {
            if (playerDataPosition == null)
                playerDataPosition = AccessTools.Field(typeof(PlayerData), "position");
            SetNotificatorValue(playerDataPosition.GetValue(data), pos);
        }

        public static Vector3 GetPlayerDataPosition(PlayerData data)
        {
            if (playerDataPosition == null)
                playerDataPosition = AccessTools.Field(typeof(PlayerData), "position");
            object v = GetNotificatorValue(playerDataPosition.GetValue(data));
            return v is Vector3 p ? p : Vector3.zero;
        }

        public static int GetPlayerCharState(PlayerData data)
        {
            if (playerDataCharState == null)
                playerDataCharState = AccessTools.Field(typeof(PlayerData), "charState");
            object v = GetNotificatorValue(playerDataCharState.GetValue(data));
            return v == null ? 0 : Convert.ToInt32(v);
        }

        private static object GetNotificatorValue(object notificator)
        {
            if (notificator == null)
                return null;
            return ValueProp(notificator.GetType())?.GetValue(notificator, null);
        }

        private static void SetNotificatorValue(object notificator, object value)
        {
            if (notificator == null)
                return;
            ValueProp(notificator.GetType())?.SetValue(notificator, value, null);
        }

        private static PropertyInfo ValueProp(Type t)
        {
            if (!valueProps.TryGetValue(t, out PropertyInfo p))
            {
                p = AccessTools.Property(t, "Value");
                valueProps[t] = p;
            }
            return p;
        }

        private static string GetIdProperty(object o)
        {
            Type t = o.GetType();
            if (!idProps.TryGetValue(t, out PropertyInfo p))
            {
                p = AccessTools.Property(t, "Id");
                idProps[t] = p;
            }
            return p?.GetValue(o, null) as string;
        }

        private static readonly HashSet<int> WorkStates = new HashSet<int>
        {
            (int)AnimationState.ToolAxe,
            (int)AnimationState.ToolShovel,
            (int)AnimationState.ToolPickaxe,
            (int)AnimationState.ToolHammer,
            (int)AnimationState.WorkHands,
            (int)AnimationState.Planting,
            (int)AnimationState.AttackCommon,
            (int)AnimationState.AttackSpear,
            (int)AnimationState.AttackBow,
            (int)AnimationState.AttackMelee,
        };

        /// <summary>Chopping, digging, mining, planting, fighting, or in build mode right now.</summary>
        public static bool PlayerIsWorkingOrBuilding
        {
            get
            {
                PlayerController p = Player;
                if (p == null)
                    return false;
                try
                {
                    if (WorkStates.Contains(PlayerAnimState))
                        return true;
                    if (!p.IsControlEnabledByType(TakenControlType.ByBuilding))
                        return true;
                    BuildController bc = BuildController.Instance;
                    if (bc != null && bc.IsBuildModeActive)
                        return true;
                    PlayerInteractionComponent ic = InteractionOf(p);
                    return ic != null && ic.IsPaused;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void SetPlayerControlByUI(bool enabled)
        {
            try
            {
                Player?.SetControlTakenType(TakenControlType.ByUI, enabled);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("SetControlTakenType failed: " + e.Message);
            }
        }

        private static PlayerController cachedInteractionOwner;
        private static PlayerInteractionComponent cachedInteraction;

        private static PlayerInteractionComponent InteractionOf(PlayerController p)
        {
            if (p != cachedInteractionOwner || cachedInteraction == null)
            {
                cachedInteractionOwner = p;
                cachedInteraction = p != null ? p.GetComponentInChildren<PlayerInteractionComponent>() : null;
            }
            return cachedInteraction;
        }

        // ---------------------------------------------------------------- bodies

        public const string CorpseItemId = "body_corpse";

        /// <summary>Corpses the local player is carrying overhead.</summary>
        public static int LocalCarriedBodies
        {
            get
            {
                PlayerData d = LocalPlayerData;
                if (d == null || !d.HasOverheadItem)
                    return 0;
                int n = 0;
                foreach (Item item in d.OverheadItems)
                    if (item != null && !item.IsEmpty && ItemDefId(item) == CorpseItemId)
                        n += item.Count;
                return n;
            }
        }

        private static MethodInfo countBodyCorpses;

        /// <summary>
        /// Recomputes the "unburied bodies" counter the same way the game's own save fixer does
        /// (bodies on the ground, open graves, morgue tables, carried bodies), plus the bodies the
        /// other player is carrying, which only they can see in their game.
        /// </summary>
        public static void RecountBodies(int remoteCarried)
        {
            try
            {
                GameSave save = MainGame.Instance?.GameSave;
                if (save == null || save.playerData == null)
                    return;
                if (countBodyCorpses == null)
                    countBodyCorpses = AccessTools.Method(typeof(SaveCodeFixes_Helper), "CountBodyCorpses", new[] { typeof(WorldData) });
                if (countBodyCorpses == null)
                    return;
                int n = (int)countBodyCorpses.Invoke(null, new object[] { save.worldData }) + LocalCarriedBodies + Math.Max(0, remoteCarried);
                if (save.playerData.GetResInt("cur_bodies_count") != n)
                {
                    CoopPlugin.Log.LogInfo("Unburied bodies recounted: " + save.playerData.GetResInt("cur_bodies_count") + " -> " + n);
                    save.playerData.SetRes("cur_bodies_count", n);
                }
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Body recount failed: " + e.Message);
            }
        }

        /// <summary>Removes anything carried overhead from a character that was copied from the host.</summary>
        public static void ClearOverheadItems(PlayerData d)
        {
            try
            {
                AccessTools.Field(typeof(PlayerData), "overheadItems")?.SetValue(d, new List<Item>());
                AccessTools.Field(typeof(PlayerData), "overheadItemLegacy")?.SetValue(d, null);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Could not clear copied overhead items: " + e.Message);
            }
        }

        // ---------------------------------------------------------------- quests / objective arrow

        private static FieldInfo arrowField;
        private static object tutorialArrow;

        /// <summary>Id of the object the objective arrow points at (Guid.Empty if none).</summary>
        public static Guid ObjectiveArrowTarget
        {
            get
            {
                PlayerData d = LocalPlayerData;
                if (d == null)
                    return Guid.Empty;
                if (arrowField == null)
                    arrowField = AccessTools.Field(typeof(PlayerData), "tutorialArrowWgoId");
                return SGuidToGuid(arrowField?.GetValue(d));
            }
        }

        private static object TutorialArrow
        {
            get
            {
                if (tutorialArrow is UnityEngine.Object uo && uo != null)
                    return tutorialArrow;
                // UITutorialArrow is a LazySingleton<UITutorialArrow>; read its Instance.
                Type t = AccessTools.TypeByName("UITutorialArrow");
                PropertyInfo inst = t?.BaseType != null ? AccessTools.Property(t.BaseType, "Instance") : null;
                tutorialArrow = inst?.GetValue(null, null);
                return tutorialArrow;
            }
        }

        public static void SetObjectiveArrow(Guid target)
        {
            object arrow = TutorialArrow;
            if (arrow == null)
                return;
            try
            {
                if (target == Guid.Empty)
                {
                    if (ObjectiveArrowTarget != Guid.Empty)
                        AccessTools.Method(arrow.GetType(), "UnAttach")?.Invoke(arrow, null);
                    return;
                }
                if (ObjectiveArrowTarget == target)
                    return;
                WgoData w = FindWgo(target);
                if (w != null)
                    AccessTools.Method(arrow.GetType(), "Attach", new[] { typeof(WgoData) })?.Invoke(arrow, new object[] { w });
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Objective arrow sync failed: " + e.Message);
            }
        }

        public static Camera WorldCamera
        {
            get
            {
                try
                {
                    CameraSystem cs = CameraSystem.Instance;
                    if (cs != null && cs.MainCamera != null && cs.MainCamera.Camera != null)
                        return cs.MainCamera.Camera;
                }
                catch
                {
                }
                return Camera.main;
            }
        }

        // ---------------------------------------------------------------- world objects

        public static Guid WgoGuid(WgoData w)
        {
            if (wgoUniqueIdField == null)
                wgoUniqueIdField = AccessTools.Field(typeof(WgoData), "uniqueId");
            return SGuidToGuid(wgoUniqueIdField.GetValue(w));
        }

        /// <summary>LazyBearTechnology's SGuid wraps a System.Guid.</summary>
        public static Guid SGuidToGuid(object sguid)
        {
            if (sguid == null)
                return Guid.Empty;
            Type t = sguid.GetType();
            if (!guidMembers.TryGetValue(t, out MemberInfo m))
            {
                m = (MemberInfo)AccessTools.Property(t, "Guid") ?? AccessTools.Field(t, "Guid") ?? (MemberInfo)AccessTools.Field(t, "guid");
                guidMembers[t] = m;
            }
            object g = m is PropertyInfo pi ? pi.GetValue(sguid, null) : (m as FieldInfo)?.GetValue(sguid);
            return g is Guid guid ? guid : Guid.Empty;
        }

        // ---------------------------------------------------------------- items

        private static FieldInfo itemUniqueIdField;
        private static FieldInfo itemDefIdField;
        private static PropertyInfo itemDefinitionProp;
        private static FieldInfo itemDefIconField;
        private static FieldInfo itemDefGroupsField;

        public static Guid ItemGuid(Item item)
        {
            if (item == null)
                return Guid.Empty;
            if (itemUniqueIdField == null)
                itemUniqueIdField = AccessTools.Field(typeof(Item), "uniqueId");
            return SGuidToGuid(itemUniqueIdField?.GetValue(item));
        }

        public static string ItemDefId(Item item)
        {
            if (item == null)
                return "";
            if (itemDefIdField == null)
                itemDefIdField = AccessTools.Field(typeof(Item), "id");
            return itemDefIdField?.GetValue(item) as string ?? "";
        }

        private static object ItemDefinition(Item item)
        {
            if (item == null)
                return null;
            if (itemDefinitionProp == null)
                itemDefinitionProp = AccessTools.Property(typeof(Item), "Definition");
            return itemDefinitionProp?.GetValue(item, null);
        }

        public static string ItemIconId(Item item)
        {
            object def = ItemDefinition(item);
            if (def == null)
                return "";
            if (itemDefIconField == null)
                itemDefIconField = AccessTools.Field(def.GetType(), "iconId");
            return itemDefIconField?.GetValue(def) as string ?? "";
        }

        /// <summary>Corpses / bodies (the game tags them with the item groups "body" and "corpse").</summary>
        public static bool ItemIsBody(Item item)
        {
            object def = ItemDefinition(item);
            if (def == null)
                return false;
            if (itemDefGroupsField == null)
                itemDefGroupsField = AccessTools.Field(def.GetType(), "itemGroupIds");
            if (itemDefGroupsField?.GetValue(def) is System.Collections.IEnumerable groups)
            {
                foreach (object g in groups)
                {
                    string s = g as string;
                    if (s == "body" || s == "corpse")
                        return true;
                }
            }
            return false;
        }

        /// <summary>Icon ids of whatever the local player is carrying over their head (bodies, crates...).</summary>
        public static void GetOverheadIcons(List<string> into)
        {
            into.Clear();
            PlayerData d = LocalPlayerData;
            if (d == null || !d.HasOverheadItem)
                return;
            foreach (Item item in d.OverheadItems)
            {
                if (item == null || item.IsEmpty)
                    continue;
                string icon = ItemIconId(item);
                if (!string.IsNullOrEmpty(icon))
                    into.Add(icon);
            }
        }

        /// <summary>Current weight of every layer of the local player's animator (carry pose, armor...).</summary>
        public static float[] GetPlayerLayerWeights()
        {
            PlayerController p = Player;
            Animator a = p?.View?.PlayerAnimation?.Animator;
            if (a == null || !a.isActiveAndEnabled)
                return new float[0];
            var w = new float[a.layerCount];
            for (int i = 0; i < w.Length; i++)
                w[i] = a.GetLayerWeight(i);
            return w;
        }

        /// <summary>Gives <paramref name="target"/> the same identity object as <paramref name="source"/>.</summary>
        public static void CopyWgoIdentity(WgoData source, WgoData target)
        {
            if (wgoUniqueIdField == null)
                wgoUniqueIdField = AccessTools.Field(typeof(WgoData), "uniqueId");
            wgoUniqueIdField.SetValue(target, wgoUniqueIdField.GetValue(source));
        }

        private static PropertyInfo wgoDefinitionProp;

        /// <summary>The object's definition (from the game's balance data) resolves.</summary>
        public static bool HasDefinition(WgoData w)
        {
            try
            {
                if (wgoDefinitionProp == null)
                    wgoDefinitionProp = AccessTools.Property(typeof(WgoData), "Definition");
                return wgoDefinitionProp?.GetValue(w, null) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>True if the object's storage or craft inventory holds anything.</summary>
        public static bool HasStoredItems(WgoData w)
        {
            try
            {
                return InventoryHasItems(w.Inventory) || InventoryHasItems(w.CraftInventory);
            }
            catch
            {
                return true; // Unknown: treat as precious.
            }
        }

        private static bool InventoryHasItems(Inventory inventory)
        {
            List<Item> items = inventory?.Data?.Inventory;
            if (items == null)
                return false;
            foreach (Item it in items)
                if (it != null && !it.IsEmpty)
                    return true;
            return false;
        }

        public static string WgoDefId(WgoData w)
        {
            if (wgoDefIdField == null)
                wgoDefIdField = AccessTools.Field(typeof(WgoData), "id");
            return wgoDefIdField?.GetValue(w) as string ?? "";
        }

        public static WgoDataCache WorldCache
        {
            get
            {
                WorldData wd = MainGame.Instance?.GameSave?.worldData;
                if (wd == null || !wd.HasCache)
                    return null;
                return wd.Cache;
            }
        }

        public static WgoData FindWgo(Guid guid)
        {
            WgoDataCache c = WorldCache;
            if (c == null || guid == Guid.Empty)
                return null;
            c.wgoDataByUidCache.TryGetValue(guid, out WgoData w);
            return w;
        }

        /// <summary>Nearest object with the same definition in the same scene, within <paramref name="maxDist"/>.</summary>
        public static WgoData FindWgoByDefAndPosition(string defId, string sceneId, Vector3 pos, float maxDist)
        {
            WgoDataCache c = WorldCache;
            if (c == null || string.IsNullOrEmpty(defId))
                return null;
            if (!c.wgoDataByIdsCache.TryGetValue(defId, out List<WgoData> list) || list == null)
                return null;
            WgoData best = null;
            float bestSqr = maxDist * maxDist;
            foreach (WgoData w in list)
            {
                if (w == null || (sceneId != null && w.WorldId != sceneId))
                    continue;
                float d = (w.Position - pos).sqrMagnitude;
                if (d <= bestSqr)
                {
                    bestSqr = d;
                    best = w;
                }
            }
            return best;
        }

        public static GameSceneData FindScene(string sceneId)
        {
            WorldData wd = MainGame.Instance?.GameSave?.worldData;
            return wd?.GetGameSceneDataById(sceneId);
        }

        public static List<string> LoadedSceneIds
        {
            get
            {
                try
                {
                    return MainGame.Instance?.GameSave?.worldData?.LoadedScenes;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static WgoData WgoUnderInteraction
        {
            get
            {
                PlayerController p = Player;
                if (p == null)
                    return null;
                PlayerInteractionComponent ic = InteractionOf(p);
                if (ic == null || !ic.HasWgoUnderInteraction)
                    return null;
                Wgo wgo = ic.WgoUnderInteraction;
                return wgo != null ? wgo.Data : null;
            }
        }

        // ---------------------------------------------------------------- time

        public static void GetTime(out int day, out float timeOfDay)
        {
            EnvironmentEngine env = EnvironmentEngine.Instance;
            EnvironmentData data = MainGame.Instance?.GameSave?.environmentData;
            day = data != null ? data.Day : 0;
            timeOfDay = env != null ? env.timeOfDay : 0f;
        }

        public static void ApplyTime(int day, float timeOfDay)
        {
            EnvironmentEngine env = EnvironmentEngine.Instance;
            EnvironmentData data = MainGame.Instance?.GameSave?.environmentData;
            if (env == null || data == null)
                return;
            if (data.Day != day)
            {
                if (environmentDayField == null)
                    environmentDayField = AccessTools.Field(typeof(EnvironmentData), "day");
                environmentDayField?.SetValue(data, day);
            }
            float diff = Mathf.Abs(env.timeOfDay - timeOfDay);
            diff = Mathf.Min(diff, 1f - diff);
            if (diff > 0.003f)
                env.SetTimeOfDay(Mathf.Repeat(timeOfDay, 1f));
        }

        // ---------------------------------------------------------------- UI

        /// <summary>Closes the main menu window (it would otherwise stay on top after loading).</summary>
        public static void CloseMainMenu()
        {
            try
            {
                // Looked up by name: the window derives from LazyBearTechnology's LazyWindow<T>.
                Type windowType = AccessTools.TypeByName("UIMainMenuWindow");
                if (windowType == null)
                    return;
                foreach (UnityEngine.Object w in UnityEngine.Object.FindObjectsOfType(windowType))
                {
                    MethodInfo close = w.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .FirstOrDefault(m => m.Name == "Close" && m.GetParameters().Length == 0);
                    close?.Invoke(w, null);
                }
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Could not close main menu: " + e.Message);
            }
        }

        public static string HostSlotName
        {
            get
            {
                try
                {
                    return SaveSystem.RelevantSlot?.slotName ?? "unsaved";
                }
                catch
                {
                    return "unsaved";
                }
            }
        }
    }
}
