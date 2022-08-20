using HarmonyLib;

namespace AudioLink.Patches
{
    [HarmonyPatch(typeof(MultiplayerLevelScenesTransitionSetupDataSO))]
    [HarmonyPatch(nameof(MultiplayerLevelScenesTransitionSetupDataSO.Init))]
    internal class PMultiplayerLevelScenesTransitionSetupDataSO
    {
        internal static void Postfix(ref ColorScheme overrideColorScheme)
        {
            try
            {
                Plugin.AudioLink?.Configure(overrideColorScheme);
            }
            catch (System.Exception)
            {

            }
        }
    }
}
