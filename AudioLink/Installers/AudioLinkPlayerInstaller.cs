using AudioLink.Providers;
using JetBrains.Annotations;
using Zenject;

namespace AudioLink.Installers
{
    [UsedImplicitly]
    internal class AudioLinkPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameProvider>().AsSingle().NonLazy();
        }
    }
}
