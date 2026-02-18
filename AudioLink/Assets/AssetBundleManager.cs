using System.IO;
using UnityEngine;

namespace AudioLink.Assets
{
    internal class AssetBundleManager
    {
        private const string PATH = "AudioLink.Assets.Bundle";

        internal AssetBundleManager()
        {
            byte[] bytes;
            using (Stream stream = typeof(AssetBundleManager).Assembly.GetManifestResourceStream(PATH)!)
            using (MemoryStream memoryStream = new())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

#if V1_29_1
            const uint crc = 3706163252;
#else
            const uint crc = 2240245297;
#endif
            AssetBundle bundle = AssetBundle.LoadFromMemory(bytes, crc);
            Material = bundle.LoadAsset<Material>("assets/com.llealloo.audiolink/runtime/materials/mat_audiolink.mat");
            RenderTexture = bundle.LoadAsset<CustomRenderTexture>("assets/com.llealloo.audiolink/runtime/rendertextures/rt_audiolink.asset");
            RenderTexture.updateMode = CustomRenderTextureUpdateMode.Realtime;
            bundle.Unload(false);
        }

        internal Material Material { get; }

        internal CustomRenderTexture RenderTexture { get; }
    }
}
