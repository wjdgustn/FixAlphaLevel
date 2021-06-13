using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ADOFAI;
using GDMiniJSON;
using HarmonyLib;

namespace FixAlphaLevel.MainPatch {
    [HarmonyPatch(typeof(RDFile), "ReadAllText")]

    internal static class Test {
        private static bool Prefix(ref string __result, string path, Encoding encoding = null) {
            if (!path.EndsWith(".adofai")) return true;
            
            if (encoding == null)
                encoding = RDFile.NonSwitchDefaultEncoding;
            var level = File.ReadAllText(path, encoding);

            if (!(Json.Deserialize(level) is Dictionary<string, object> dict))
                return false;
            Dictionary<string, object> dictionary = dict;
            Dictionary<string, object> dict1 = dictionary["settings"] as Dictionary<string, object>;
            
            var key = "version";
            var version = dict1.ContainsKey(key) ? Convert.ToInt32(dict1[key]) : 1;
            
            if (version == 4) level = level.Replace("decorationImage", "decText");

            __result = level;
            return false;
        }
    }
    
    [HarmonyPatch(typeof(LevelData), "Decode")]

    internal static class FixAlphaLevel {
        private static void Postfix(LevelData __instance) {
            if (__instance.version == 4) __instance.version = 3;
        }
    }
}