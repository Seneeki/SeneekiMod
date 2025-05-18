using HarmonyLib;
using Il2CppScheduleOne.UI;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using SeneekiMod.Utils;

namespace SeneekiMod.Cheats
{
    [HarmonyPatch(typeof(PickpocketScreen), "Update")]
    internal static class Pickpocket
    {
        private static readonly HashSet<PickpocketScreen> unlockedScreens = new();

        private static void Postfix(PickpocketScreen __instance)
        {
            if (!ModPreferences.UnlockPocketsEnabled?.Value == true)
                return;

            if (__instance == null || __instance.Slots == null)
                return;

            if (!__instance.IsOpen || __instance.isSliding)
            {
                unlockedScreens.Remove(__instance);
                return;
            }

            if (unlockedScreens.Contains(__instance))
                return;

            try
            {
                if (__instance.Slots is Il2CppArrayBase<ItemSlotUI> slots && slots.Count > 0)
                {
                    for (int s = 0; s < slots.Count; s++)
                    {
                        if (slots[s] != null)
                            __instance.SetSlotLocked(s, false);
                    }

                    unlockedScreens.Add(__instance);
                }
            }
            catch (System.Exception ex)
            {
                Loggos.Error($"[Pickpocket] Failed unlocking slots: {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(PickpocketScreen), nameof(PickpocketScreen.Open))]
    internal static class PickpocketEasyMode
    {
        [HarmonyPostfix]
        private static void Postfix(PickpocketScreen __instance)
        {
            if (!ModPreferences.UnlockPocketsEnabled?.Value == true)
                return;

            try
            {
                // Force green zone width visually
                if (__instance.GreenAreas is Il2CppArrayBase<RectTransform> areas)
                {
                    foreach (var area in areas)
                    {
                        if (area != null)
                        {
                            var size = area.sizeDelta;
                            size.x = 160f; // large green zone
                            area.sizeDelta = size;
                        }
                    }
                }

                // Force underlying min/max width to match
                __instance.GreenAreaMinWidth = 160f;
                __instance.GreenAreaMaxWidth = 160f;

                // Slow down the slider
                __instance.SlideTime *= 2.5f;
                __instance.SlideTimeMaxMultiplier = 1f;

                // More leniency
                __instance.Tolerance *= 2f;
            }
            catch (System.Exception ex)
            {
                Loggos.Error($"[Pickpocket] Failed to apply easy mode settings: {ex}");
            }
        }
    }
}
