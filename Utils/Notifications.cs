using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI;
using UnityEngine;

namespace SeneekiMod.Utils
{
    internal static class SKNotify
    {
        internal static void ShowNotification(string? message = null, string? imageFile = "icon.png")
        {
            var instance = Singleton<NotificationsManager>.Instance;
            if (instance == null)
            {
                Loggos.Warn("[SKNotify] NotificationsManager instance not found.");
                return;
            }
            SeneekiPaths.EnsureDirectory();
            string iconPath = SeneekiPaths.GetPath(imageFile!);
            string sender = "Seneeki LLC";
            string body = message ?? $"Payment Received: ${ModPreferences.ReallyDirtyMoney?.Value:N0}.";
            Sprite? icon = LoadSprite(iconPath);
            if (icon == null)
            {
                Loggos.Warn("[SKNotify] Sprite load failed — using no icon.");
            }
            instance.SendNotification(sender, body, icon, 5f, true);
        }
        private static Sprite? LoadSprite(string iconPath)
        {
            if (!File.Exists(iconPath))
            {
                Loggos.Warn($"[SKNotify] Icon not found at path: {iconPath}");
                return null;
            }
            try
            {
                byte[] data = File.ReadAllBytes(iconPath);
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!tex.LoadImage(data))
                {
                    Loggos.Warn("[SKNotify] Failed to load image data into texture.");
                    return null;
                }

                tex.filterMode = FilterMode.Point;
                return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            catch (Exception ex)
            {
                Loggos.Error($"[SKNotify] Exception while loading sprite: {ex}");
                return null;
            }
        }

    }
}
