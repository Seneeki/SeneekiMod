using System.Drawing;
using MelonLoader;

namespace SeneekiMod.Utils
{
    internal static class Loggos
    {
        private static readonly HashSet<string> info = new();
        private static readonly HashSet<string> success = new();
        private static readonly HashSet<string> warn = new();
        private static readonly HashSet<string> error = new();
        private static readonly HashSet<string> unique = new();
        private static readonly HashSet<string> header = new();
        private static readonly HashSet<string> footer = new();
        private static readonly HashSet<string> debug = new();

        internal static void Info(string msg) =>
            Once(info, Color.FromArgb(64, 117, 230, 147), msg);        
        internal static void Success(string msg) =>
            Once(success, Color.FromArgb(255, 29, 168, 84), msg);      
        internal static void Warn(string msg) =>
            Once(warn, Color.FromArgb(128, 221, 237, 71), msg);       
        internal static void Error(string msg) =>
            Once(error, Color.FromArgb(128, 178, 34, 34), msg);      
        internal static void Unique(string msg) =>
            Once(unique, Color.FromArgb(128, 43, 156, 245), msg);   
        internal static void Header(string msg) =>
            Once(header, Color.FromArgb(0, 245, 133, 59), msg);     
        internal static void Footer(string msg) =>
            Once(footer, Color.FromArgb(255, 245, 133, 59), msg);    
        internal static void Debug(string msg) =>
            Once(debug, Color.FromArgb(0, 28, 102, 237), msg);

        private static void Once(HashSet<string> store, object colour, string msg, double delay = 1.0)
        {
            if (msg == null) return;

            lock (store)
            {
                if (!store.Add(msg)) return;
            }

            if (colour is ConsoleColor cc)
                MelonLogger.Msg(cc, msg);
            else if (colour is Color c)
                MelonLogger.Msg(c, msg);
            else
                MelonLogger.Msg(msg);

            _ = YeetLater(store, msg, delay);
        }


        internal static async Task YeetLater(HashSet<string> store, string msg, double sec)
        {
            await Task.Delay(TimeSpan.FromSeconds(sec));
            lock (store) store.Remove(msg);
        }


        internal static void WipeItClean()
        {
            info.Clear();
            success.Clear();
            warn.Clear();
            error.Clear();
            unique.Clear();
            header.Clear();
            footer.Clear();
        }
    }
}