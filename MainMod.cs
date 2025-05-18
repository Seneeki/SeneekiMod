using MelonLoader;
using SeneekiMod.Utils;
using SeneekiMod.Cheats;
using SeneekiMod.Enhancements;

namespace SeneekiMod
{
    internal class MainMod : MelonMod
    {

        public override void OnInitializeMelon()
        {
            Loggos.Header("═════════════════════════════════════════════════════════════════════════");
            Loggos.Footer(" SeneekiMod - Begin Initialization");

            ModPreferences.Init();

            Loggos.Footer(" SeneekiMod - Initialization Complete");
            Loggos.Footer("═════════════════════════════════════════════════════════════════════════");
        }

        public override void OnUpdate()
        {
            ModPreferences.OnUpdate();
            EmployeeMove.Init();
        }

        public override void OnLateUpdate()
        {
            ModPreferences.OnLateUpdate();
            try
            {
                AddMoney.GiveDirtyMoney();
            }
            catch (System.Exception ex)
            {
                Loggos.Error($"[Dirty Money] Failed to give really dirty money: {ex}");
            }
        }
    }
}
