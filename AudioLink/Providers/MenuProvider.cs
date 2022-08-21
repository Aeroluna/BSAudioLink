using System;
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

        private readonly Scripts.AudioLink _audioLink;
        private readonly PlayerDataModel _playerDataModel;

        [UsedImplicitly]
        private MenuProvider(Scripts.AudioLink audioLink, PlayerDataModel playerDataModel)
        {
            _audioLink = audioLink;
            _playerDataModel = playerDataModel;
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

        [AffinityPostfix]
        [AffinityPatch(typeof(ColorManagerInstaller), nameof(ColorManagerInstaller.InstallBindings))]
        private void ColorManagerInstallerProvide(ColorSchemeSO ____menuColorScheme)
        {
            _audioLink.SetColorScheme(_playerDataModel.playerData.colorSchemesSettings.GetOverrideColorScheme() ?? ____menuColorScheme.colorScheme);
        }
    }
}
