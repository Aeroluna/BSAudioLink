using System.Linq;
using AudioLink.Assets;
using AudioLink.Installers;
using IPA;
using IPA.Config;
using JetBrains.Annotations;
using SiraUtil.Zenject;

namespace AudioLink
{
    [Plugin(RuntimeOptions.DynamicInit)]
    internal class Plugin
    {
        internal const string CAPABILITY = "AudioLink";

        [UsedImplicitly]
        [Init]
        public Plugin(Config config, Zenjector zenjector)
        {
            AssetBundleManager.LoadFromMemoryAsync();
            zenjector.Install<AudioLinkPlayerInstaller>(Location.Player);
        }

        public static bool Enabled { get; private set; }

#pragma warning disable CA1822
        [UsedImplicitly]
        [OnEnable]
        public void OnEnable()
        {
            if (IPA.Loader.PluginManager.EnabledPlugins.Any(x => x.Id == "SongCore"))
            {
                ToggleCapability(true);
            }

            Enabled = true;
        }

        [UsedImplicitly]
        [OnDisable]
        public void OnDisable()
        {
            if (IPA.Loader.PluginManager.EnabledPlugins.Any(x => x.Id == "SongCore"))
            {
                ToggleCapability(false);
            }

            Enabled = false;
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
