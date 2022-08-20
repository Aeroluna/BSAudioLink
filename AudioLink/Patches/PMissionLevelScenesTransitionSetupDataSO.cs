using HarmonyLib;

namespace AudioLink.Patches
{
    [HarmonyPatch(typeof(MissionLevelScenesTransitionSetupDataSO))]
    [HarmonyPatch(nameof(MissionLevelScenesTransitionSetupDataSO.Init))]
    internal class PMissionLevelScenesTransitionSetupDataSO
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
