using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using HarmonyLib;
using Il2CppScheduleOne.UI;
using SeneekiMod.Utils;

namespace SeneekiMod.Enhancements
{
    internal class MixNames
    {
        private static readonly List<string> name1Clean = new();
        private static readonly List<string> name1NSFW = new();
        private static readonly List<string> name2Clean = new();
        private static readonly List<string> name2NSFW = new();
        private static readonly List<string> name3Clean = new();
        private static readonly List<string> name3NSFW = new();

        private static readonly List<string> tempName1 = new();
        private static readonly List<string> tempName2 = new();

        private static readonly TextInfo TextInfo = CultureInfo.InvariantCulture.TextInfo;

        [HarmonyPatch(typeof(NewMixScreen), nameof(NewMixScreen.Open))]
        internal static class PrepareTemporaryNames
        {
            [HarmonyPostfix]
            private static void Postfix(NewMixScreen __instance)
            {
                if (ModPreferences.MoreMixNamesEnabled?.Value != true)
                    return;

                LoadNameData();

                tempName1.Clear();
                tempName2.Clear();

                if (__instance.name1Library != null)
                    tempName1.AddRange(__instance.name1Library.ToArray());
                if (__instance.name2Library != null)
                    tempName2.AddRange(__instance.name2Library.ToArray());

                tempName1.AddRange(name1Clean);
                tempName2.AddRange(name2Clean);

                for (int i = 0; i < 3; i++) tempName1.Add("MILF");
                for (int i = 0; i < 5; i++) tempName2.Add("Girl Scout");

                if (ModPreferences.NSFWMixNamesEnabled?.Value == true)
                {
                    tempName1.AddRange(name1NSFW);
                    tempName2.AddRange(name2NSFW);
                }
                try
                {
                    if (__instance.mixAlreadyExistsText != null)
                    {
                        const int maxTries = 10;
                        int tries = 0;

                        while (tries++ < maxTries)
                        {
                            __instance.RandomizeButtonClicked();

                            if (!__instance.mixAlreadyExistsText.activeSelf)
                            {
                                Loggos.Debug($"[MixNames] Unique name accepted after {tries} rerolls.");
                                break;
                            }
                        }

                        if (__instance.mixAlreadyExistsText.activeSelf)
                            Loggos.Warn("[MixNames] All generated names were duplicates after 10 tries.");
                    }
                }
                catch (Exception ex)
                {
                    Loggos.Error($"[MixNames] Error rerolling name: {ex}");
                }
            }
        }

        [HarmonyPatch(typeof(NewMixScreen), nameof(NewMixScreen.GenerateUniqueName))]
        internal static class MixNameOverride
        {
            [HarmonyPostfix]
            private static void Postfix(ref string __result)
            {
                if (ModPreferences.MoreMixNamesEnabled?.Value != true)
                    return;

                string first = GetRandomSafe(tempName1);
                string second = GetRandomSafe(tempName2);

                if (first == "Rusty" && second == "Trombone")
                {
                    Loggos.Debug("[MixNames] Rusty Trombone detected — skipping third name.");
                    __result = FormatName(first, second);
                    return;
                }

                string? third = null;
                if (UnityEngine.Random.value < 0.15f)
                {
                    var pool = new List<string>(name3Clean);
                    if (ModPreferences.NSFWMixNamesEnabled?.Value == true)
                        pool.AddRange(name3NSFW);
                    third = TryGetThirdName(first, second, pool);
                }

                __result = SanitizeNameParts(first, second, third);
            }
        }

        private static void LoadNameData()
        {
            name1Clean.Clear(); name1NSFW.Clear();
            name2Clean.Clear(); name2NSFW.Clear();
            name3Clean.Clear(); name3NSFW.Clear();

            string path = SeneekiPaths.GetPath("names.json");
            if (!File.Exists(path))
            {
                Loggos.Warn("[MixNames] names.json not found.");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                foreach (Match match in Regex.Matches(json, "\"(\\w+)\"\\s*:\\s*\\[(.*?)\\]", RegexOptions.Singleline))
                {
                    string key = match.Groups[1].Value;
                    string rawList = match.Groups[2].Value;

                    var values = Regex.Matches(rawList, "\"(.*?)\"")
                        .Cast<Match>()
                        .Select(m => m.Groups[1].Value.Trim())
                        .ToList();

                    switch (key)
                    {
                        case "name1Clean": name1Clean.AddRange(values); break;
                        case "name1NSFW": name1NSFW.AddRange(values); break;
                        case "name2Clean": name2Clean.AddRange(values); break;
                        case "name2NSFW": name2NSFW.AddRange(values); break;
                        case "name3Clean": name3Clean.AddRange(values); break;
                        case "name3NSFW": name3NSFW.AddRange(values); break;
                        default: break;
                    }
                }

                Loggos.Debug("[MixNames] Loaded name lists from JSON manually.");
            }
            catch (Exception ex)
            {
                Loggos.Error($"[MixNames] Failed to load names.json: {ex}");
            }
        }

        private static string GetRandomSafe(List<string> list)
        {
            return list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)] : "Error";
        }

        private static string TryGetThirdName(string first, string second, List<string> pool)
        {
            for (int i = 0; i < 3; i++)
            {
                string candidate = pool[UnityEngine.Random.Range(0, pool.Count)];
                if (candidate != first && candidate != second)
                    return candidate;
            }
            return "Error";
        }

        private static string SanitizeNameParts(string first, string second, string? third = null)
        {
            var clean = new[] { first, second, third ?? "" }
                .SelectMany(w => w.Split(' '))
                .Select(w => w.Trim().ToLowerInvariant())
                .ToArray();

            var group1 = new[] { "dick", "cock", "dick punch", "dong", "balls" };
            var group2 = new[] { "cunt", "cunt cheese", "piss flaps", "pussy", "pussy lips", "flaps", "panties", "milf", "girl scout", "cooch" };

            for (int i = 0; i < clean.Length - 1; i++)
                if (clean[i] == clean[i + 1])
                    clean[i + 1] = "";

            foreach (var group in new[] { group1, group2 })
            {
                var matches = clean.Where(p => group.Contains(p)).ToList();
                if (matches.Count > 1)
                    for (int i = 1; i < matches.Count; i++)
                        clean[Array.IndexOf(clean, matches[i])] = "";
            }

            bool hasGroup1 = clean.Any(p => group1.Contains(p));
            bool hasGroup2 = clean.Any(p => group2.Contains(p));

            if (hasGroup1 && hasGroup2)
            {
                var thirdPool = new List<string>(name3Clean);
                if (ModPreferences.NSFWMixNamesEnabled?.Value == true)
                    thirdPool.AddRange(name3NSFW);

                for (int i = clean.Length - 1; i >= 2; i--)
                {
                    if (!string.IsNullOrWhiteSpace(clean[i]))
                    {
                        for (int tries = 0; tries < 2; tries++)
                        {
                            var replacement = thirdPool[UnityEngine.Random.Range(0, thirdPool.Count)];
                            if (!group1.Contains(replacement.ToLower()) && !group2.Contains(replacement.ToLower()) && !clean.Contains(replacement.ToLower()))
                            {
                                clean[i] = replacement.ToLowerInvariant();
                                Loggos.Debug($"[MixNames] Group conflict replaced with '{replacement}'");
                                goto done;
                            }
                        }
                        Loggos.Debug("[MixNames] Group conflict unresolved. Removed third word.");
                        clean[i] = "";
                    }
                }
            }

        done:
            return CapEach(string.Join(" ", clean.Where(p => !string.IsNullOrWhiteSpace(p))));
        }

        private static string FormatName(string first, string second)
        {
            return $"{CapEach(first)} {CapEach(second)}";
        }

        private static string CapEach(string input)
        {
            return string.Join(" ", input.Split(' ')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(p =>
                {
                    p = p.Trim();
                    if (p.Equals("milf", StringComparison.InvariantCultureIgnoreCase))
                        return "MILF";

                    return p.Length switch
                    {
                        0 => "",
                        1 => p.ToUpperInvariant(),
                        2 => p.ToUpperInvariant(),
                        _ => char.ToUpperInvariant(p[0]) + p.Substring(1).ToLowerInvariant()
                    };
                }));
        }

    }
}
