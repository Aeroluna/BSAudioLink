using System;
using System.Linq;
using IPA.Utilities;
using JetBrains.Annotations;
using SiraUtil.Affinity;
using UnityEngine;

namespace AudioLink.Providers
{
    internal class MenuProvider : IAffinity
    {
        private static readonly FieldAccessor<SongPreviewPlayer.AudioSourceVolumeController, AudioSource>.Accessor _audioSourceAccessor =
            FieldAccessor<SongPreviewPlayer.AudioSourceVolumeController, AudioSource>.GetAccessor("audioSource");

        private static readonly FieldAccessor<ColorManagerInstaller, ColorSchemeSO>.Accessor _colorSchemeAccessor =
            FieldAccessor<ColorManagerInstaller, ColorSchemeSO>.GetAccessor("_menuColorScheme");

        private static ColorScheme? _menuColorScheme;

        private readonly Scripts.AudioLink _audioLink;
        private readonly PlayerDataModel _playerDataModel;

        [UsedImplicitly]
        private MenuProvider(Scripts.AudioLink audioLink, PlayerDataModel playerDataModel)
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
                    _menuColorScheme = _colorSchemeAccessor(ref colorManagerInstaller).colorScheme;
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
            SongPreviewPlayer.AudioSourceVolumeController audioSourceController = ____audioSourceControllers[____activeChannel];
            _audioLink.SetAudioSource(_audioSourceAccessor(ref audioSourceController));
        }
    }
}
