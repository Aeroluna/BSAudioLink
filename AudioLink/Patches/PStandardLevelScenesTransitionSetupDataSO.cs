using HarmonyLib;

namespace AudioLink.Patches
{
    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO))]
    [HarmonyPatch(nameof(StandardLevelScenesTransitionSetupDataSO.Init))]
    internal class PStandardLevelScenesTransitionSetupDataSO
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
