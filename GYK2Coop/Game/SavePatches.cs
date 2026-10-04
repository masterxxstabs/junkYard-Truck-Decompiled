using System;
using HarmonyLib;

namespace GYK2Coop.Game
{
    /// <summary>
    /// While a guest is in the host's world, nothing may be written to the guest's own save
    /// slots: the world belongs to the host and the guest's character is stored by the host.
    /// </summary>
    [HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.Save))]
    internal static class Patch_SaveSystem_Save
    {
        internal static bool BlockSaves;

        private static bool Prefix(Action callbackSuccessful)
        {
            if (!BlockSaves)
                return true;
            CoopPlugin.Log.LogInfo("Guest in co-op: skipping local save (the host keeps the world and your character).");
            try
            {
                callbackSuccessful?.Invoke();
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Save callback threw: " + e.Message);
            }
            return false;
        }
    }
}
