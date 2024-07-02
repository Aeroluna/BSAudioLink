using AudioLink.Assets;
using AudioLink.Providers;
using JetBrains.Annotations;
using Zenject;

namespace AudioLink.Installers
{
    [UsedImplicitly]
    internal class AudioLinkAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<AssetBundleManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<Scripts.AudioLink>().AsSingle();
            Container.BindInterfacesTo<UserInfoProvider>().AsSingle();
        }
    }
}
