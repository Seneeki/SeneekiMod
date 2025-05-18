using HarmonyLib;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.Property;
using SeneekiMod.Utils;

namespace SeneekiMod.Laundering
{
    [HarmonyPatch(typeof(LaunderingInterface))]
    internal static class LaunderingPlus
    {

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void Postfix_Initialize(Business bus)
        {
            if (ModPreferences.LaunderingPlusEnabled?.Value != true)
                return;
            SKLaunderer.RemoveLaunderingCap(bus);
        }

        [HarmonyPatch("CreateEntry")]
        [HarmonyPostfix]
        public static void Postfix_CreateEntry(LaunderingOperation op)
        {
            if (ModPreferences.LaunderingPlusEnabled?.Value != true)
                return;

            SKLaunderer.RemoveLaunderingCap(op?.business);
            SKLaunderer.ApplyCustomLaunderTime(op);
        }
    }
}
