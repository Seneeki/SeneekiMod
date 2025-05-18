using HarmonyLib;
using Il2CppScheduleOne.Economy;
using SeneekiMod.Utils;

namespace SeneekiMod.Enhancements
{
    internal class FastDeal
    {
        [HarmonyPatch(typeof(Customer), nameof(Customer.OfferDealValid))]
        internal static class CustomDealCooldownPatch
        {
            [HarmonyPrefix]
            internal static bool Prefix(Customer __instance, ref string invalidReason, ref bool __result)
            {
                int? customCooldown = ModPreferences.CustomDealCooldownMinutes?.Value;
                if (customCooldown == null || customCooldown <= 0)
                    return true; // fallback to game logic

                int cooldown = customCooldown.Value;

                if (__instance.TimeSinceLastDealCompleted < cooldown ||
                    __instance.TimeSinceLastDealOffered < cooldown ||
                    __instance.TimeSinceInstantDealOffered < cooldown)
                {
                    invalidReason = $"Customer is already fuck-eyed. Let them rest.";
                    __result = false;
                    return false;
                }

                invalidReason = string.Empty;
                __result = true;
                return false; // Skip original logic
            }
        }
    }
}
