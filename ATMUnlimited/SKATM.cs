using SeneekiMod.Utils;
using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.UI.ATM;
using HarmonyLib;

namespace SeneekiMod.ATMUnlimited
{
    internal static class SKATM
    {
        internal static bool? lastState = null;
        internal static void ApplyOverrides(bool forceLog)
        {
            bool unlimited = ModPreferences.ATMUnlimitedEnabled?.Value == true;

            if (forceLog || lastState != unlimited)
            {
                lastState = unlimited;
            }
        }
        [HarmonyPatch(typeof(ATM), nameof(ATM.Enter))]
        internal static class Patch_ATM_Enter
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                if (ModPreferences.ATMUnlimitedEnabled?.Value == true && ATM.WeeklyDepositSum > 0f)
                {
                    try
                    {
                        ATM.WeeklyDepositSum = 0f;
                    }
                    catch (System.Exception ex)
                    {
                        Loggos.Error($"[ATMUnlimited] Failed to reset ATM weekly deposit sum: {ex}");
                    }
                }
            }
        }
        [HarmonyPatch(typeof(ATMInterface), nameof(ATMInterface.ReturnToMenuButtonPressed))]
        internal static class Patch_ATMInterface_ReturnToMenuButtonPressed
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                if (ModPreferences.ATMUnlimitedEnabled?.Value == true && ATM.WeeklyDepositSum > 0f)
                {
                    try
                    {
                        ATM.WeeklyDepositSum = 0f;
                    }
                    catch (System.Exception ex)
                    {
                        Loggos.Error($"[ATMUnlimited] Failed to reset ATM weekly deposit sum: {ex}");
                    }
                }
            }
        }
    }

}

