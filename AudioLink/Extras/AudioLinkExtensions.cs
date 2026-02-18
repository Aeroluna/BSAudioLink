using UnityEngine;

namespace AudioLink.Extras
{
    public static class AudioLinkExtensions
    {
        internal static void SetAudioSource(this AudioLink audioLink, AudioSource audioSource)
        {
            audioLink.audioSource = audioSource;
        }

        internal static void SetColorScheme(this AudioLink audioLink, ColorScheme colorScheme)
        {
            audioLink.themeColorMode = 1;

            audioLink.customThemeColor0 = colorScheme.environmentColor0;
            audioLink.customThemeColor1 = colorScheme.environmentColor1;
            if (colorScheme.supportsEnvironmentColorBoost)
            {
                audioLink.customThemeColor2 = colorScheme.environmentColor0Boost;
                audioLink.customThemeColor3 = colorScheme.environmentColor1Boost;
            }
            else
            {
                audioLink.customThemeColor2 = colorScheme.environmentColor0;
                audioLink.customThemeColor3 = colorScheme.environmentColor1;
            }

            audioLink.UpdateThemeColors();
        }
    }
}
