using System.Collections.Generic;
using System.Linq;
using AudioLink.Assets;
using AudioLink.Installers;
using IPA;
using IPA.Loader;
using JetBrains.Annotations;
using SiraUtil.Zenject;

namespace AudioLink
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    internal class Plugin
    {
        internal const string CAPABILITY = "AudioLink";

        [UsedImplicitly]
        [Init]
        public Plugin(IPA.Logging.Logger logger, Zenjector zenjector)
        {
            Logger = logger;
            AssetBundleManager.LoadFromMemoryAsync();
            zenjector.Install<AudioLinkMenuInstaller>(Location.Menu);
            zenjector.Install<AudioLinkPlayerInstaller>(Location.Player);
            zenjector.Install<AudioLinkAppInstaller>(Location.App);
        }

        public static IPA.Logging.Logger Logger { get; set; } = null!;

#pragma warning disable CA1822
        [UsedImplicitly]
        [OnEnable]
        public void OnEnable()
        {
            IEnumerable<PluginMetadata> allPlugins = PluginManager.EnabledPlugins.Concat(PluginManager.DisabledPlugins);
            if (allPlugins.Any(x => x.Id == "SongCore"))
            {
                ToggleCapability(true);
            }
        }
#pragma warning restore CA1822

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
    }
}
