using System.Linq;
using AudioLink.Providers;
using JetBrains.Annotations;
using Zenject;

namespace AudioLink.Installers
{
    [UsedImplicitly]
    internal class AudioLinkMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MenuProvider>().AsSingle();

            if (IPA.Loader.PluginManager.EnabledPlugins.Any(x => x.Id == "NalulunaMenu"))
            {
                Container.BindInterfacesTo<NalulunaMenuProvider>().AsSingle();
            }
        }
    }
}
