using IPA.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace AudioLink.Providers
{
    internal class GameProvider : IInitializable
    {
        private static readonly FieldAccessor<AudioTimeSyncController, AudioSource>.Accessor _audioSourceAccessor =
            FieldAccessor<AudioTimeSyncController, AudioSource>.GetAccessor("_audioSource");

        private readonly Scripts.AudioLink _audioLink;
        private readonly ColorScheme _colorScheme;
        private AudioTimeSyncController _audioTimeSyncController;

        [UsedImplicitly]
        private GameProvider(Scripts.AudioLink audioLink, AudioTimeSyncController audioTimeSyncController, ColorScheme colorScheme)
        {
            _audioLink = audioLink;
            _audioTimeSyncController = audioTimeSyncController;
            _colorScheme = colorScheme;
        }

        public void Initialize()
        {
            _audioLink.SetAudioSource(_audioSourceAccessor(ref _audioTimeSyncController));
            _audioLink.SetColorScheme(_colorScheme);
        }
    }
}
