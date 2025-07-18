using System;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Globalization;

namespace LocalSaveTimes;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} patches done: {harmony.GetPatchedMethods().ToList().Count} methods");
    } 
}

[HarmonyPatch]
public class PluginPatches
{
    [HarmonyPatch(typeof(Save), nameof(Save.TryParseSavedDateTime))]
    [HarmonyPrefix]
    private static bool ShowLocalTimePatch(string savedDateTime, ref DateTime dateTime, ref bool __result)
    {
        Plugin.Logger.LogDebug($"Trying to parse saved date time: {savedDateTime}");
        if (DateTime.TryParse(savedDateTime, null, DateTimeStyles.RoundtripKind, out DateTime parsedTime))
        {
            dateTime = parsedTime.Kind == DateTimeKind.Utc ? parsedTime.ToLocalTime() : parsedTime;
            __result = true;
            return false;
        }

        return true;
    }
}
