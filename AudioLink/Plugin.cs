using System.Linq;
using AudioLink.Assets;
using AudioLink.Installers;
using IPA;
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

            if (IPA.Loader.PluginManager.EnabledPlugins.Any(x => x.Id == "SongCore"))
            {
                ToggleCapability(true);
            }
        }

        public static IPA.Logging.Logger Logger { get; set; } = null!;

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
