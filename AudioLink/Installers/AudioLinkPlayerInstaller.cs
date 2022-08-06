using JetBrains.Annotations;
using Zenject;

namespace AudioLink.Installers
{
    [UsedImplicitly]
    internal class AudioLinkPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (!Plugin.Enabled)
            {
                return;
            }

            Container.BindInterfacesTo<Scripts.AudioLink>().AsSingle().NonLazy();
        }
    }
}
