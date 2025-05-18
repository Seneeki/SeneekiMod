using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.Property;
using MelonLoader.Utils;
using UnityEngine;
using HarmonyLib;
using System.Reflection.Emit;
using MelonLoader;
using Il2CppScheduleOne.Economy;


namespace SeneekiMod.Utils
{
    internal static class SKMoney
    {
        internal static MoneyManager SKmoneyManager => UnityEngine.Object.FindObjectOfType<MoneyManager>();

        internal static void DepositCash(int amount)
        {
            if (SKmoneyManager == null || amount <= 0 || SKmoneyManager.cashInstance.Balance < amount) return;
            SKmoneyManager.cashInstance.Balance -= amount;
            SKmoneyManager.onlineBalance += amount;
        }

        internal static void WithdrawCash(int amount)
        {
            if (SKmoneyManager == null || amount <= 0 || SKmoneyManager.onlineBalance < amount) return;
            SKmoneyManager.onlineBalance -= amount;
            SKmoneyManager.cashInstance.Balance += amount;
        }

        internal static void DepositAllCash()
        {
            if (SKmoneyManager == null || SKmoneyManager.cashInstance.Balance <= 0) return;
            SKmoneyManager.onlineBalance += SKmoneyManager.cashInstance.Balance;
            SKmoneyManager.cashInstance.Balance = 0;
        }
    } //End SKMoney
    internal static class ATMManager
    {
        private static ATM SKatmManager => UnityEngine.Object.FindObjectOfType<ATM>();
        internal static float WeeklyDepositSum
        {
            get => ATM.WeeklyDepositSum;
            set => ATM.WeeklyDepositSum = value;
        }
        [HarmonyPatch(typeof(ATM), "DropCash")]
        internal class ATMDropCashPatch        
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var code in instructions)                {
                    if (code.opcode == OpCodes.Ldc_I4_2) // MIN_CASH_DROP = 2
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4, 200); // New min
                    }
                    else if (code.opcode == OpCodes.Ldc_I4_8) // MAX_CASH_DROP = 8
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4, 500); // New max
                    }
                    else
                    {
                        yield return code;
                    }
                }
            }
        }
    }    // End ATMManager
        internal static class SKLaunderer
    {
        internal static void RemoveLaunderingCap(Business? business)
        {
            if (business == null)
                return;

            try
            {
                business.LaunderCapacity = float.MaxValue;
            }
            catch (System.Exception ex)
            {
                Loggos.Error($"[Laundering Plus] Failed to remove laundering cap: {ex}");
            }
        }
        internal static void ApplyCustomLaunderTime(LaunderingOperation? op)
        {
            if (op == null)
                return;

            try
            {
                float timeHours = ModPreferences.LaunderTimeHours?.Value ?? 24f;
                op.completionTime_Minutes = Mathf.CeilToInt(timeHours * 60f);
            }
            catch (System.Exception ex)
            {
                Loggos.Error($"[Laundering Plus] Failed to apply custom launder time: {ex}");
            }
        }
    } //End Launderer
    internal static class SeneekiPaths
    {
        public static readonly string UserDataRoot = Path.Combine(MelonEnvironment.UserDataDirectory, "SeneekiMod");

        public static string GetPath(string filename)
        {
            Loggos.Debug($"[SeneekiPaths] Path: {filename}");
            Loggos.Debug($"[SeneekiPaths] Path: {UserDataRoot}");
            return Path.Combine(UserDataRoot, filename);

        }
        internal static void EnsureDirectory()
        {
            if (!Directory.Exists(UserDataRoot))
                try
                {
                    Directory.CreateDirectory(UserDataRoot);
                    Loggos.Debug($"[SeneekiPaths] Created directory: {UserDataRoot}");
                }
                catch (Exception ex)
                {
                    Loggos.Error($"[SeneekiPaths] Failed to create directory: {ex}");
                }
        }
    } // End SeneekiPaths
    internal class Fields
    {
        internal static void AddIntField(string label, MelonPreferences_Entry<int>? entry, ref float y)
        {
            if (entry == null) return;

            GUI.Label(new Rect(20, y, 200, 20), $"{label}:");
            string input = GUI.TextField(new Rect(180, y, 60, 20), entry.Value.ToString());

            if (int.TryParse(input, out int newValue) && newValue != entry.Value)
            {
                entry.Value = newValue;
                entry.Save();
            }

            y += 30f;
        }
        internal static void AddFloatField(string label, MelonPreferences_Entry<float>? entry, ref float y)
        {
            if (entry == null) return;

            GUI.Label(new Rect(20, y, 200, 20), $"{label}:");
            string input = GUI.TextField(new Rect(180, y, 60, 20), entry.Value.ToString("0.##"));

            if (float.TryParse(input, out float newValue) && newValue != entry.Value)
            {
                entry.Value = newValue;
                entry.Save();
            }

            y += 30f;
        }

    }

}// End namespace

