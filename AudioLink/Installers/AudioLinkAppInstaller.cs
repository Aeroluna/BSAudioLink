using JetBrains.Annotations;
using Zenject;

namespace AudioLink.Installers
{
    [UsedImplicitly]
    internal class AudioLinkAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Scripts.AudioLink>().AsSingle();
        }
    }
}
