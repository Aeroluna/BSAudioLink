using AudioLink.Assets;
using IPA;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace AudioLink
{
    [Plugin(RuntimeOptions.DynamicInit)]
    internal class Plugin
    {
        internal const string HARMONY_ID = "com.github.aeroluna.audiolink";
        internal const string CAPABILITY = "AudioLink";
        internal static IPA.Logging.Logger Logger;
        internal static Harmony m_Harmony;

        public static Components.AudioLink AudioLink { get; private set; }
        public static bool Enabled { get; private set; }

        [Init]
        public Plugin(IPA.Logging.Logger logger)
        {
            Logger = logger;
            AssetBundleManager.LoadFromMemoryAsync();

        }

#pragma warning disable CA1822
        [OnEnable]
        public void OnEnable()
        {
            if (IPA.Loader.PluginManager.EnabledPlugins.Any(x => x.Id == "SongCore"))
            {
                ToggleCapability(true);
            }

            Enabled = true;

            try
            {
                m_Harmony = new Harmony(HARMONY_ID);
                m_Harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (System.Exception p_Exception)
            {
                Logger.Error("[OnEnable] Error:");
                Logger.Error(p_Exception);
            }

            AudioLink = new GameObject("AudioLink").AddComponent<Components.AudioLink>();
            GameObject.DontDestroyOnLoad(AudioLink.gameObject);
        }
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
