using JetBrains.Annotations;
using Zenject;

namespace AudioLink.Providers
{
    internal class GameProvider : IInitializable
    {
        private readonly Scripts.AudioLink _audioLink;
        private readonly ColorScheme _colorScheme;
        private readonly AudioTimeSyncController _audioTimeSyncController;

        [UsedImplicitly]
        private GameProvider(Scripts.AudioLink audioLink, AudioTimeSyncController audioTimeSyncController, ColorScheme colorScheme)
        {
            _audioLink = audioLink;
            _audioTimeSyncController = audioTimeSyncController;
            _colorScheme = colorScheme;
        }

        public void Initialize()
        {
            _audioLink.SetAudioSource(_audioTimeSyncController._audioSource);
            _audioLink.SetColorScheme(_colorScheme);
        }
    }
}
