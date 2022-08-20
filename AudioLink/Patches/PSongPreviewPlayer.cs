using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AudioLink.Patches
{
    [HarmonyPatch(typeof(SongPreviewPlayer))]
    [HarmonyPatch(nameof(SongPreviewPlayer.CrossfadeTo), new Type[] { typeof(AudioClip), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(Action) })]
    internal class PSongPreviewPlayerA
    {
        private static FieldInfo s_AudioSourceField = null;


        internal static void Postfix(   ref AudioClip audioClip,
                                        ref AudioClip ____defaultAudioClip, ref int ____activeChannel, ref SongPreviewPlayer.AudioSourceVolumeController[] ____audioSourceControllers)
        {
            try
            {
                if (audioClip == ____defaultAudioClip && ____defaultAudioClip && ____defaultAudioClip.name == "Menu")
                {
                    Plugin.AudioLink?.Configure(null as AudioSource);
                    return;
                }

                foreach (var _audioSourceController in ____audioSourceControllers)
                {
                    if (s_AudioSourceField == null)
                        s_AudioSourceField = _audioSourceController.GetType().GetField("audioSource", BindingFlags.Instance | BindingFlags.Public);

                    var _audioSource = (AudioSource) s_AudioSourceField.GetValue(_audioSourceController);
                    if (_audioSource.clip == audioClip && ____audioSourceControllers.IndexOf(_audioSourceController) == ____activeChannel)
                    {
                        var playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault()?.playerData ?? null;
                        var colorScheme = playerData != null ? playerData.colorSchemesSettings.GetSelectedColorScheme() : null;

                        Plugin.AudioLink?.Configure(_audioSource);
                        Plugin.AudioLink?.ConfigureDefault(colorScheme);
                        return;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }


    [HarmonyPatch(typeof(SongPreviewPlayer))]
    [HarmonyPatch(nameof(SongPreviewPlayer.CrossfadeToDefault), new Type[] { })]
    internal class PSongPreviewPlayerB
    {
        internal static void Postfix()
        {
            try
            {
                Plugin.AudioLink?.Configure(null as AudioSource);
            }
            catch (Exception)
            {

            }
        }
    }
}
