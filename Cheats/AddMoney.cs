using UnityEngine;
using SeneekiMod.Utils;
using Il2CppScheduleOne.Money;


namespace SeneekiMod.Cheats
{
    internal static class AddMoney
    {
        private static bool greedyBeggar = false;

        internal static void GiveDirtyMoney()
        {
            bool LCandPlus = Input.GetKey((KeyCode)306) && (Input.GetKey((KeyCode)270) || Input.GetKey((KeyCode)43) || Input.GetKey((KeyCode)61)); // LeftCtrl + KP_Plus or Plus or Equals
            bool LCandMinus = Input.GetKey((KeyCode)306) && (Input.GetKey((KeyCode)45) || Input.GetKey((KeyCode)269));                             // LeftCtrl + Minus of KP_Minus

            if (LCandPlus && !greedyBeggar)
            {
                greedyBeggar = true;
                try
                {
                    int dirtyCash = Mathf.RoundToInt(ModPreferences.ReallyDirtyMoney!.Value);
                    MoneyManager.Instance.ChangeCashBalance(dirtyCash, visualizeChange: true, playCashSound: true);
                }
                catch (System.Exception ex)
                {
                    Loggos.Error($"[Dirty Money] Failed to add dirty money: {ex}");
                }
            }
            else if (LCandMinus && !greedyBeggar)
            {
                greedyBeggar = true;
                try
                {
                    int dirtyCash = Mathf.RoundToInt(ModPreferences.ReallyDirtyMoney!.Value);
                    MoneyManager.Instance.CreateOnlineTransaction("Totally Legit Cash",dirtyCash,1,"Courtesy of Seneeki LLC");
                    MoneyManager._ShowOnlineBalanceChange_d__55 moveNext = new(0);
                    MoneyManager.Instance.CashSound.Play();
                    SKNotify.ShowNotification();
                }
                catch (System.Exception ex)
                {
                    Loggos.Error($"[Dirty Money] Failed to give dirty online money: {ex}");
                }
            }
            else if (!LCandPlus && !LCandMinus)
            {
                greedyBeggar = false;
            }
        }
    }
}
