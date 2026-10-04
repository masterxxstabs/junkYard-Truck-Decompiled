using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace GYK2Coop
{
    [BepInPlugin(Guid, Name, Version)]
    public class CoopPlugin : BaseUnityPlugin
    {
        public const string Guid = "gyk2coop.multiplayer";
        public const string Name = "Graveyard Keeper 2 Co-op";
        public const string Version = "0.2.0";

        // Bump whenever the wire format changes; host and guest must match.
        public const int ProtocolVersion = 2;

        internal static ManualLogSource Log;

        internal static ConfigEntry<string> PlayerName;
        internal static ConfigEntry<int> Port;
        internal static ConfigEntry<string> LastAddress;
        internal static ConfigEntry<KeyCode> ToggleKey;
        internal static ConfigEntry<bool> SyncWorldObjects;
        internal static ConfigEntry<float> SyncRadius;
        internal static ConfigEntry<bool> SyncTime;
        internal static ConfigEntry<string> ExcludedWgoTypes;

        private void Awake()
        {
            Log = Logger;

            PlayerName = Config.Bind("General", "PlayerName", SafeUserName(), "Name shown above your character for the other player.");
            Port = Config.Bind("Network", "Port", 7777, "TCP port the host listens on. The host must forward this port (or use a VPN like Tailscale / ZeroTier / Radmin VPN).");
            LastAddress = Config.Bind("Network", "LastAddress", "127.0.0.1", "Last address typed into the Join box.");
            ToggleKey = Config.Bind("General", "ToggleKey", KeyCode.F8, "Key that opens the co-op panel.");
            SyncWorldObjects = Config.Bind("Sync", "SyncWorldObjects", true, "Mirror world objects (trees chopped, things built, chests, crops) that either player changes.");
            SyncRadius = Config.Bind("Sync", "SyncRadius", 15f, "Only object changes within this many metres of the player who made them are mirrored. Keeps background simulation from being sent twice.");
            SyncTime = Config.Bind("Sync", "SyncTime", true, "Keep the guest's day and time of day locked to the host's.");
            ExcludedWgoTypes = Config.Bind("Sync", "ExcludedWgoTypes", "ZombieWgoData", "Comma-separated WgoData class names that are never mirrored (AI-driven things each game simulates itself).");

            var harmony = new Harmony(Guid);
            foreach (Type t in typeof(CoopPlugin).Assembly.GetTypes())
            {
                if (t.GetCustomAttributes(typeof(HarmonyPatch), false).Length == 0)
                    continue;
                try
                {
                    harmony.CreateClassProcessor(t).Patch();
                }
                catch (Exception e)
                {
                    // One broken hook (e.g. after a game update) should not take the whole mod down.
                    Log.LogError("Failed to apply patch " + t.Name + ": " + e);
                }
            }

            // BepInEx's own manager object is destroyed in some Unity games, so run from our own object.
            var go = new GameObject("GYK2Coop");
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<CoopRunner>();

            Log.LogInfo(Name + " " + Version + " loaded. Press " + ToggleKey.Value + " to open the co-op panel.");
        }

        private static string SafeUserName()
        {
            try
            {
                string n = Environment.UserName;
                if (!string.IsNullOrEmpty(n))
                    return n;
            }
            catch
            {
            }
            return "Keeper";
        }
    }
}
