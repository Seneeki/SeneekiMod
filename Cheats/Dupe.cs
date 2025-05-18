using UnityEngine;
using Il2CppScheduleOne.PlayerScripts;
using SeneekiMod.Utils;
using System.Reflection;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.UI.Shop;


namespace SeneekiMod.Cheats
{
    internal static class Dupe
    {
        private static bool KP = false;
        internal static void Update()
        {
            if (!ModPreferences.DupeEnabled!.Value) return;

            bool PK = Input.GetKey((KeyCode)308) && Input.GetKey((KeyCode)304) && Input.GetKey((KeyCode)306); // Alt + Shift + Ctrl

            if (PK && !KP)
            {
                KP = true;
                TryDuplicateEquippedItem();
            }
            else if (!PK)
            {
                KP = false;
            }
        }

        private static void TryDuplicateEquippedItem()
        {
            var inv = PlayerInventory.Instance;
            if (inv?.hotbarSlots == null) return;

            foreach (var slot in inv.hotbarSlots)
            {
                if (slot != null && slot.IsEquipped)
                {
                    var instance = slot.ItemInstance;
                    if (instance == null) return;

                    int originalQty = instance.Quantity;
                    int newQty = (int)(originalQty * ModPreferences.DupeMultiplier!.Value);
                    int cap = 16384;                     
                    if (newQty > cap) newQty = cap; // Prevents overflow
                    Loggos.Info("[SeneekiMod]Item dupe capped at 16384");
                    if (newQty > instance.StackLimit)
                    {
                        if (ModPreferences.RespectStackLimit?.Value == true)
                        {
                            newQty = instance.StackLimit;
                        }
                        else
                        {
                            try
                            {
                                var def = instance.Definition;
                                if (def != null)
                                {
                                    def.StackLimit = newQty;
                                    Loggos.Debug($"[Duplicator] Increased stack limit to {newQty}");
                                }
                                else
                                {
                                    Loggos.Warn("[Duplicator] Item definition was null.");
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Loggos.Error($"[Duplicator] Failed to raise stack limit: {ex}");
                            }

                        }
                    }

                    try
                    {
                        var setQtyMethod = instance.GetType().GetMethod("SetQuantity", BindingFlags.Instance | BindingFlags.Public);
                        if (setQtyMethod != null)
                        {
                            instance.SetQuantity(newQty); // Valid method call
                        }
                        else
                        {
                            instance.Quantity = newQty;
                        }

                        instance.onDataChanged?.Invoke();
                        inv.Reequip(); // Ensure UI reflects changes
                        Loggos.Debug($"[Duplicator] Duplicated item from {originalQty} to {newQty}");
                    }
                    catch (System.Exception ex)
                    {
                        Loggos.Error($"[Duplicator] Failed to apply duplicated quantity: {ex}");
                    }

                    return; // Only one equipped item is ever duplicated
                }
            }
        }

    }
}
