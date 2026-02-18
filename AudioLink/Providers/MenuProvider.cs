using System;
using System.Linq;
using AudioLink.Extras;
using JetBrains.Annotations;
using SiraUtil.Affinity;
using UnityEngine;

namespace AudioLink.Providers
{
    internal class MenuProvider : IAffinity
    {
        private static ColorScheme? _menuColorScheme;

        private readonly AudioLink _audioLink;
        private readonly PlayerDataModel _playerDataModel;

        [UsedImplicitly]
        private MenuProvider(AudioLink audioLink, PlayerDataModel playerDataModel)
        {
            _audioLink = audioLink;
            _playerDataModel = playerDataModel;
            RefreshColorScheme();
        }

        private static ColorScheme MenuColorScheme
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_menuColorScheme == null)
                {
                    ColorManagerInstaller? colorManagerInstaller = Resources.FindObjectsOfTypeAll<ColorManagerInstaller>().First();
                    _menuColorScheme = colorManagerInstaller._menuColorScheme.colorScheme;
                }

                return _menuColorScheme;
            }
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(ColorsOverrideSettingsPanelController), nameof(ColorsOverrideSettingsPanelController.HandleOverrideColorsToggleValueChanged))]
        [AffinityPatch(typeof(ColorsOverrideSettingsPanelController), nameof(ColorsOverrideSettingsPanelController.HandleEditColorSchemeControllerDidChangeColorScheme))]
        [AffinityPatch(typeof(ColorsOverrideSettingsPanelController), "HandleDropDownDidSelectCellWithIdx")]
        private void RefreshColorScheme()
        {
            _audioLink.SetColorScheme(_playerDataModel.playerData.colorSchemesSettings.GetOverrideColorScheme() ?? MenuColorScheme);
        }

        [AffinityPostfix]
        [AffinityPatch(
            typeof(SongPreviewPlayer),
            nameof(SongPreviewPlayer.CrossfadeTo),
            AffinityMethodType.Normal,
            null,
            typeof(AudioClip),
            typeof(float),
            typeof(float),
            typeof(float),
            typeof(bool),
            typeof(Action))]
        private void SongPreviewPlayerProvide(int ____activeChannel, SongPreviewPlayer.AudioSourceVolumeController[] ____audioSourceControllers)
        {
            _audioLink.SetAudioSource(____audioSourceControllers[____activeChannel].audioSource);
        }
    }
}
