﻿using JetBrains.Annotations;
using UnityEngine;

namespace AudioLink.Scripts
{
    [PublicAPI]
    public enum MediaPlaying
    {
        None = 0,
        Playing = 1,
        Paused = 2,
        Stopped = 3,
        Loading = 4,
        Streaming = 5,
        Error = 6
    }

    [PublicAPI]
    public enum MediaLoop
    {
        None = 0,
        Loop = 1,
        LoopOne = 2,
        Random = 3,
        RandomLoop = 4
    }

    public partial class AudioLink
    {
        private static readonly int _mediaVolume = Shader.PropertyToID("_MediaVolume");
        private static readonly int _mediaTime = Shader.PropertyToID("_MediaTime");
        private static readonly int _mediaPlaying = Shader.PropertyToID("_MediaPlaying");
        private static readonly int _mediaLoop = Shader.PropertyToID("_MediaLoop");

        /// <summary>
        /// Set Media Volume display. Volume is in range 0.0f to 1.0f.
        /// </summary>
        /// <param name="volume">The volume to set between 0.0f and 1.0f</param>
        public void SetMediaVolume(float volume)
        {
            _audioMaterial.SetFloat(_mediaVolume, volume);
        }

        /// <summary>
        /// Set Media Time display. Time is in range 0.0f and 1.0f.
        /// </summary>
        /// <param name="time">The time to set between 0.0f and 1.0f</param>
        public void SetMediaTime(float time)
        {
            _audioMaterial.SetFloat(_mediaTime, time);
        }

        /// <summary>
        /// Set Media Playing display.
        /// None    0 (0.0f),
        /// Playing 1 (1.0f),
        /// Paused  2 (2.0f),
        /// Stopped 3 (3.0f),
        /// Loading 4 (4.0f).
        /// </summary>
        /// <param name="playingstate">The playing state to set.</param>
        public void SetMediaPlaying(MediaPlaying playingstate)
        {
            int state = (int)playingstate;
            _audioMaterial.SetFloat(_mediaPlaying, state);
        }

        /// <summary>
        /// Set Media Loop display.
        /// None       0 (0.0f),
        /// Loop       1 (1.0f),
        /// LoopOne    2 (2.0f),
        /// Random     3 (3.0f),
        /// RandomLoop 4 (4.0f).
        /// </summary>
        /// <param name="loopstate">The loop state to set.</param>
        public void SetMediaLoop(MediaLoop loopstate)
        {
            int loop = (int)loopstate;
            _audioMaterial.SetFloat(_mediaLoop, loop);
        }
    }
}
