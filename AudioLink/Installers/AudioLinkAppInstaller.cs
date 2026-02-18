using AudioLink.Assets;
using AudioLink.Providers;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace AudioLink.Installers
{
    [UsedImplicitly]
    internal class AudioLinkAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            GameObject audioLinkGameObject = new("AudioLink")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            Object.DontDestroyOnLoad(audioLinkGameObject);
            audioLinkGameObject.SetActive(false);
            AssetBundleManager assetBundleManager = new();
            AudioLink audioLink = audioLinkGameObject.AddComponent<AudioLink>();
            audioLink.audioMaterial = assetBundleManager.Material;
            audioLink.audioRenderTexture = assetBundleManager.RenderTexture;
            audioLinkGameObject.SetActive(true);

            Container.Bind<AssetBundleManager>().FromInstance(assetBundleManager);
            Container.Bind<AudioLink>().FromInstance(audioLink);
            Container.BindInterfacesTo<UserInfoProvider>().AsSingle();
        }
    }
}
