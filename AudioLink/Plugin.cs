using AudioLink.Assets;
using AudioLink.Installers;
using BepInEx;
using BepInEx.Logging;
using SiraUtil.Zenject;

namespace AudioLink
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("BSIPA_Utilities")]
    [BepInDependency("SiraUtil")]
    [BepInProcess("Beat Saber.exe")]
    internal class Plugin : BaseUnityPlugin
    {
        internal const string CAPABILITY = "AudioLink";

        internal static ManualLogSource Log { get; private set; } = null!;

        private static void ToggleCapability(bool value)
        {
            if (value)
            {
                SongCore.Collections.RegisterCapability(CAPABILITY);
            }
            else
            {
                SongCore.Collections.DeregisterizeCapability(CAPABILITY);
            }
        }

        private void Awake()
        {
            Log = Logger;
            AssetBundleManager.LoadFromMemoryAsync();
            Zenjector zenjector = Zenjector.ConstructZenjector(Info);
            zenjector.Install<AudioLinkMenuInstaller>(Location.Menu);
            zenjector.Install<AudioLinkPlayerInstaller>(Location.Player);
            zenjector.Install<AudioLinkAppInstaller>(Location.App);
        }

#pragma warning disable CA1822
        private void Start()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("SongCore"))
            {
                ToggleCapability(true);
            }
        }
#pragma warning restore CA1822
    }
}
