using MelonLoader;
using SeneekiMod.Cheats;
using UnityEngine;
using static SeneekiMod.ATMUnlimited.SKATM;


namespace SeneekiMod.Utils
{
    public static class ModPreferences
    {
        internal static MelonPreferences_Category? SKFinancial { get; private set; }
        internal static MelonPreferences_Category? SKCheatMenu { get; private set; }
        internal static MelonPreferences_Category? SKQoL { get; private set; }
        internal static MelonPreferences_Entry<bool>? ATMUnlimitedEnabled { get; private set; }
        internal static MelonPreferences_Entry<bool>? LaunderingPlusEnabled { get; private set; }
        internal static MelonPreferences_Entry<int>? LaunderTimeHours { get; private set; }
        internal static MelonPreferences_Entry<float>? WeeklyDeposit { get; private set; }
        internal static MelonPreferences_Entry<int>? DupeMultiplier { get; private set; }
        internal static MelonPreferences_Entry<bool>? DupeEnabled { get; private set; }
        internal static MelonPreferences_Entry<bool>? RespectStackLimit { get; private set; }
        internal static MelonPreferences_Entry<float>? ReallyDirtyMoney { get; private set; }
        internal static MelonPreferences_Entry<bool>? UnlockPocketsEnabled { get; private set; }
        internal static MelonPreferences_Entry<bool>? DealPatchEnabled { get; private set; }
        internal static MelonPreferences_Entry<int>? CustomDealCooldownMinutes { get; private set; }
        internal static MelonPreferences_Entry<bool>? MoreMixNamesEnabled { get; private set; }
        internal static MelonPreferences_Entry<bool>? NSFWMixNamesEnabled { get; private set; }



        internal static void Init()
        {
            try
            {
                SKFinancial = MelonPreferences.CreateCategory("SeneekiMod:_Financial_Mods", "SeneekiMod: Financial Mods");
                SKCheatMenu = MelonPreferences.CreateCategory("SeneekiMod:_Cheat_Menu", "SeneekiMod: Cheat Menu");
                SKQoL = MelonPreferences.CreateCategory("SeneekiMod:_QoL", "SeneekiMod: Quality of Life");
                //Financial Mods
                ATMUnlimitedEnabled = SKFinancial.CreateEntry("ATMUnlimited", false, "ATM Unlimited", "If enabled, disables ATM deposit limits.");
                //WeeklyDeposit = SKFinancial.CreateEntry("WeeklyDeposit", 100000f, "Weekly Deposit Limit", "The maximum amount of money you can deposit in a week.");
                LaunderingPlusEnabled = SKFinancial.CreateEntry("EnableLaunderingPlus", true, "Enable Laundering Plus", "Removes laundering caps.");
                LaunderTimeHours = SKFinancial.CreateEntry("LaunderTimeHours", 12, "Launder Time (Hours)", "How long laundering takes per entry.");

                //Cheats
                DupeEnabled = SKCheatMenu.CreateEntry("DupeEnabled", true, "Enable Dupe Cheat", "Toggles the item dupe cheat on or off.");
                RespectStackLimit = SKCheatMenu.CreateEntry("RespectStackLimit", true, "Respect Stack Limit", "If disabled, duplicating will raise the stack limit to match the duplicated quantity.");
                DupeMultiplier = SKCheatMenu.CreateEntry("DupeMultiplier", 2, "Duplication Multiplier", "The multiplier for the amount of items you get when duplicating. Default is 2.");
                ReallyDirtyMoney = SKCheatMenu.CreateEntry("ReallyDirtyMoney", 1000f, "Really Dirty Money", "The amount of money you can conjure at once. Default is 1,000.");
                UnlockPocketsEnabled = SKCheatMenu.CreateEntry("UnlockPocketsEnabled", true, "Unlock Pockets", "If enabled, you can unlock all pockets in the game.");
                //QoL
                DealPatchEnabled = SKQoL.CreateEntry("DealPatchEnabled", true, "Enable Deal Patch", "If enabled, deal timer is ignored.");
                CustomDealCooldownMinutes = SKQoL.CreateEntry("CustomDealCooldownMinutes", 60, "Customer Deal Cooldown (minutes)", "Sets a custom cooldown in minutes for customer deals. Default is 60 mins (1 hour).");
                MoreMixNamesEnabled = SKQoL.CreateEntry("EnableMoreMixNames",false,"Enable More Mix Names","Adds additional random mix name options when creating a new product.");
                NSFWMixNamesEnabled = SKQoL.CreateEntry("EnableNSFWMixNames",false,"Enable NSFW Mix Names","Includes inappropriate/explicit terms when generating mix names. Disable for clean mode.");

                //Load preferences
                LogCurrentState();

            }
            catch (Exception ex)
            {
                Loggos.Error($"[ModPreferences] Failed to initialize preferences: {ex}");
            }
        }
        internal static void LogCurrentState()
        {
            bool atmUnlimited = ATMUnlimitedEnabled?.Value == true;
            bool laundering = LaunderingPlusEnabled?.Value == true;
            bool dupe = DupeEnabled?.Value == true;
            bool pockets = UnlockPocketsEnabled?.Value == true;
            bool moreNames = MoreMixNamesEnabled?.Value == true;
            bool nsfwNames = NSFWMixNamesEnabled?.Value == true;

            Loggos.Unique($"[ATMUnlimited] ATM weekly deposit limit override is now {(atmUnlimited ? "enabled" : "disabled")}.");
            Loggos.Unique($"[Laundering Plus] Laundering Plus is now {(laundering ? "enabled" : "disabled")}.");
            Loggos.Unique($"[Duplicator] Duplication cheat is now {(dupe ? "enabled" : "disabled")}.");
            Loggos.Unique($"[Dirty Money] Dirty Money is set to ${ReallyDirtyMoney?.Value:N0}.");
            Loggos.Unique($"[More Deals] More Deals is now {(DealPatchEnabled?.Value == true ? "enabled" : "disabled")}.");
            Loggos.Unique($"[MixNames] Extra Mix Names are {(moreNames ? "enabled" : "disabled")}.");
            Loggos.Unique($"[MixNames] NSFW Mix Names are {(nsfwNames ? "enabled" : "disabled")}.");
            Loggos.Unique($"[Pickpocket] Easy Pickpocketing is {(pockets ? "enabled" : "disabled")}.");
        }
        private static bool pressedAlready = false;
        internal static void OnUpdate()
        {
            bool keysPressed = Input.GetKey((KeyCode)306) && Input.GetKey((KeyCode)285); // LeftCtrl + F4
            if (keysPressed && !pressedAlready)
            {
                pressedAlready = true;
                MelonPreferences.Load();
                try 
                { 
                    ApplyOverrides(true);
                }
                catch (Exception ex)
                {
                    Loggos.Error($"[ModPreferences] Failed to apply overrides: {ex}");
                }
                 LogCurrentState();
                return;
            }
            else if (!keysPressed)
            {
                pressedAlready = false;
            }
        }
        internal static void OnLateUpdate()
        {
            if (DupeEnabled?.Value == true)
            {
                try
                {
                    Dupe.Update();
                }
                catch (Exception ex)
                {
                    Loggos.Error($"[ModPreferences] Failed to update dupe: {ex}");
                }
            }
        }
    }
}


