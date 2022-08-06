using System.IO;
using UnityEngine;

namespace AudioLink.Assets
{
    internal static class AssetBundleManager
    {
        private const string PATH = "AudioLink.Assets.Bundle";

        internal static Material Material { get; private set; } = null!;

        internal static RenderTexture RenderTexture { get; private set; } = null!;

        internal static void LoadFromMemoryAsync()
        {
            byte[] bytes;

            using (Stream stream = typeof(AssetBundleManager).Assembly.GetManifestResourceStream(PATH)!)
            using (MemoryStream memoryStream = new())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            AssetBundle bundle = AssetBundle.LoadFromMemory(bytes, 83812045);
            Material = bundle.LoadAsset<Material>("assets/audioLink/materials/mat_audiolink.mat");
            RenderTexture = bundle.LoadAsset<RenderTexture>("assets/audiolink/rendertextures/rt_audiolink.asset");
        }
    }
}
