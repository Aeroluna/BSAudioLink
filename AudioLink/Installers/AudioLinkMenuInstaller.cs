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

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("NalulunaMenu"))
            {
                Container.BindInterfacesTo<NalulunaMenuProvider>().AsSingle();
            }
        }
    }
}
