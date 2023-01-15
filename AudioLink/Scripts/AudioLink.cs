﻿using System;
using AudioLink.Assets;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace AudioLink.Scripts
{
    // from https://github.com/llealloo/vrc-udon-audio-link/blob/master/AudioLink/Scripts/AudioLink.cs
    internal class AudioLink : ITickable
    {
        private const float AUDIOLINK_VERSION_NUMBER = 3.01f;

        private const int RIGHT_CHANNEL_TEST_DELAY = 300;

        private const float GAIN = 1f;
        private const float BASS = 1f;
        private const float TREBLE = 1f;
        private const float X0 = 0.0f;
        private const float X1 = 0.25f;
        private const float X2 = 0.5f;
        private const float X3 = 0.75f;
        private const float THRESHOLD0 = 0.45f;
        private const float THRESHOLD1 = 0.45f;
        private const float THRESHOLD2 = 0.45f;
        private const float THRESHOLD3 = 0.45f;
        private const float FADE_LENGTH = 0.25f;
        private const float FADE_EXP_FALLOFF = 0.75f;
        private const int THEME_COLOR_MODE = 1;

        // i'm sure mawntee will bother me to implement this at some point
        private const string CUSTOM_STRING1 = "";
        private const string CUSTOM_STRING2 = "";

        private static readonly int _audioTexture = Shader.PropertyToID("_AudioTexture");

        private static readonly int _fadeLength = Shader.PropertyToID("_FadeLength");
        private static readonly int _fadeExpFalloff = Shader.PropertyToID("_FadeExpFalloff");
        private static readonly int _gain = Shader.PropertyToID("_Gain");
        private static readonly int _bass = Shader.PropertyToID("_Bass");
        private static readonly int _treble = Shader.PropertyToID("_Treble");
        private static readonly int _x0 = Shader.PropertyToID("_X0");
        private static readonly int _x1 = Shader.PropertyToID("_X1");
        private static readonly int _x2 = Shader.PropertyToID("_X2");
        private static readonly int _x3 = Shader.PropertyToID("_X3");
        private static readonly int _threshold0 = Shader.PropertyToID("_Threshold0");
        private static readonly int _threshold1 = Shader.PropertyToID("_Threshold1");
        private static readonly int _threshold2 = Shader.PropertyToID("_Threshold2");
        private static readonly int _threshold3 = Shader.PropertyToID("_Threshold3");
        private static readonly int _sourceVolume = Shader.PropertyToID("_SourceVolume");
        private static readonly int _sourceSpatialBlend = Shader.PropertyToID("_SourceSpatialBlend");
        private static readonly int _sourcePosition = Shader.PropertyToID("_SourcePosition");
        private static readonly int _themeColorMode = Shader.PropertyToID("_ThemeColorMode");
        private static readonly int _customThemeColor0ID = Shader.PropertyToID("_CustomThemeColor0");
        private static readonly int _customThemeColor1ID = Shader.PropertyToID("_CustomThemeColor1");
        private static readonly int _customThemeColor2ID = Shader.PropertyToID("_CustomThemeColor2");
        private static readonly int _customThemeColor3ID = Shader.PropertyToID("_CustomThemeColor3");

        private static readonly int _stringCustom1 = Shader.PropertyToID("_StringCustom1");
        private static readonly int _stringCustom2 = Shader.PropertyToID("_StringCustom2");

        private static readonly int _advancedTimeProps0 = Shader.PropertyToID("_AdvancedTimeProps0");
        private static readonly int _advancedTimeProps1 = Shader.PropertyToID("_AdvancedTimeProps1");
        private static readonly int _playerCountAndData = Shader.PropertyToID("_PlayerCountAndData");
        private static readonly int _versionNumberAndFPSProperty = Shader.PropertyToID("_VersionNumberAndFPSProperty");

        private static readonly int _samples0L = Shader.PropertyToID("_Samples0L");
        private static readonly int _samples1L = Shader.PropertyToID("_Samples1L");
        private static readonly int _samples2L = Shader.PropertyToID("_Samples2L");
        private static readonly int _samples3L = Shader.PropertyToID("_Samples3L");

        private static readonly int _samples0R = Shader.PropertyToID("_Samples0R");
        private static readonly int _samples1R = Shader.PropertyToID("_Samples1R");
        private static readonly int _samples2R = Shader.PropertyToID("_Samples2R");
        private static readonly int _samples3R = Shader.PropertyToID("_Samples3R");

        private readonly Material _audioMaterial;

        ////private float[] _spectrumValues = new float[1024]; // unused in original script
        ////private float[] _spectrumValuesTrim = new float[1023];
        private readonly float[] _audioFramesL = new float[1023 * 4];
        private readonly float[] _audioFramesR = new float[1023 * 4];
        private readonly float[] _samples = new float[1023];

        private Color _customThemeColor0;
        private Color _customThemeColor1;
        private Color _customThemeColor2;
        private Color _customThemeColor3;

        private AudioSource? _audioSource;

        // Mechanism to provide sync'd instance time to all avatars.
        private double _elapsedTime;
        private double _elapsedTimeMSW;
        private int _networkTimeMS;
        private double _networkTimeMSAccumulatedError;
        private double _fpsTime;
        private int _fpsCount;

        // Fix for AVPro mono game output bug (if running the game with a mono output source like a headset)
        private int _rightChannelTestCounter;
        private bool _ignoreRightChannel;

        [UsedImplicitly]
        private AudioLink()
        {
            _audioMaterial = AssetBundleManager.Material;

            UpdateSettings();
            UpdateCustomStrings();

            Shader.SetGlobalTexture(_audioTexture, AssetBundleManager.RenderTexture, RenderTextureSubElement.Default);
        }

        public void Tick()
        {
            // Tested: There does not appear to be any drift updating it this way.
            _elapsedTime += Time.deltaTime;

            // Advance the current network time by a little.
            // this algorithm also takes into account sub-millisecond jitter.
            {
                double deltaTimeMS = Time.deltaTime * 1000.0;
                int advanceTimeMS = (int)deltaTimeMS;
                _networkTimeMSAccumulatedError += deltaTimeMS - advanceTimeMS;
                if (_networkTimeMSAccumulatedError > 1)
                {
                    _networkTimeMSAccumulatedError--;
                    advanceTimeMS++;
                }

                _networkTimeMS += advanceTimeMS;
            }

            _fpsCount++;

            if (_elapsedTime >= _fpsTime)
            {
                FPSUpdate();
            }

            // use _AdvancedTimeProps0.w for Debugging
            _audioMaterial.SetVector(_advancedTimeProps0, new Vector4(
                (float)_elapsedTime,
                (float)_elapsedTimeMSW,
                (float)DateTime.Now.TimeOfDay.TotalSeconds));

            // Jan 1, 1970 = 621355968000000000.0 ticks.
            double utcSecondsUnix = (DateTime.UtcNow.Ticks / 10000000.0) - 62135596800.0;
            _audioMaterial.SetVector(_advancedTimeProps1, new Vector4(
                _networkTimeMS & 65535,
                _networkTimeMS >> 16,
                (float)Math.Floor(utcSecondsUnix / 86400),
                (float)(utcSecondsUnix % 86400)));

            // General Profiling Notes:
            //    Profiling done on 2021-05-26 on an Intel Intel Core i7-8750H CPU @ 2.20GHz
            //    Running loop 255 times (So divide all times by 255)
            //    Base load of system w/o for loop: ~420us in merlin profile land.
            //    With loop, with just summer: 1.2ms / 255
            //    Calling material.SetVeactor( ... new Vector4 ) in the loop:  2.7ms / 255
            //    Setting a float in the loop (to see if there's a difference): 1.9ms / 255
            //                             but setting 4 floats individually... is 3.0ms / 255
            //    The whole shebang with Networking.GetServerTimeInMilliseconds(); 2.3ms / 255
            //    Material.SetFloat with Networking.GetServerTimeInMilliseconds(); 2.3ms / 255
            //    Material.SetFloat with Networking.GetServerTimeInMilliseconds(), twice; 2.9ms / 255
            //    Casting and encoding as UInt32 as 2 floats, to prevent aliasing, twice: 5.1ms / 255
            //    Casting and encoding as UInt32 as 2 floats, to prevent aliasing, once: 3.2ms / 255
            // ReSharper disable once InvertIf
            if (_audioSource != null)
            {
                SendAudioOutputData();

                // Used to correct for the volume of the audio source component
                _audioMaterial.SetFloat(_sourceVolume, _audioSource.volume);
                _audioMaterial.SetFloat(_sourceSpatialBlend, _audioSource.spatialBlend);
                _audioMaterial.SetVector(_sourcePosition, _audioSource.transform.position);
            }
        }

        internal void SetAudioSource(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        internal void SetColorScheme(ColorScheme colorScheme)
        {
            _customThemeColor0 = colorScheme.environmentColor0;
            _customThemeColor1 = colorScheme.environmentColor1;
            if (colorScheme.supportsEnvironmentColorBoost)
            {
                _customThemeColor2 = colorScheme.environmentColor0Boost;
                _customThemeColor3 = colorScheme.environmentColor1Boost;
            }
            else
            {
                _customThemeColor2 = colorScheme.environmentColor0;
                _customThemeColor3 = colorScheme.environmentColor1;
            }

            UpdateThemeColors();
        }

        internal void UpdateSettings()
        {
            _audioMaterial.SetFloat(_x0, X0);
            _audioMaterial.SetFloat(_x1, X1);
            _audioMaterial.SetFloat(_x2, X2);
            _audioMaterial.SetFloat(_x3, X3);
            _audioMaterial.SetFloat(_threshold0, THRESHOLD0);
            _audioMaterial.SetFloat(_threshold1, THRESHOLD1);
            _audioMaterial.SetFloat(_threshold2, THRESHOLD2);
            _audioMaterial.SetFloat(_threshold3, THRESHOLD3);
            _audioMaterial.SetFloat(_gain, GAIN);
            _audioMaterial.SetFloat(_fadeLength, FADE_LENGTH);
            _audioMaterial.SetFloat(_fadeExpFalloff, FADE_EXP_FALLOFF);
            _audioMaterial.SetFloat(_bass, BASS);
            _audioMaterial.SetFloat(_treble, TREBLE);
        }

        // Note: These might be changed frequently so as an optimization, they're in a different function
        // rather than bundled in with the other things in UpdateSettings().
        internal void UpdateThemeColors()
        {
            _audioMaterial.SetInt(_themeColorMode, THEME_COLOR_MODE);
            _audioMaterial.SetColor(_customThemeColor0ID, _customThemeColor0);
            _audioMaterial.SetColor(_customThemeColor1ID, _customThemeColor1);
            _audioMaterial.SetColor(_customThemeColor2ID, _customThemeColor2);
            _audioMaterial.SetColor(_customThemeColor3ID, _customThemeColor3);
        }

        internal void UpdateCustomStrings()
        {
            UpdateGlobalString(_stringCustom1, CUSTOM_STRING1);
            UpdateGlobalString(_stringCustom2, CUSTOM_STRING2);
        }

        private static float IntToFloatBits24Bit(uint value)
        {
            uint frac = value & 0x007FFFFF;
            return (frac / 8388608F) * 1.1754944e-38F;
        }

        // Only happens once per second.
        private void FPSUpdate()
        {
            _audioMaterial.SetVector(_versionNumberAndFPSProperty, new Vector4(AUDIOLINK_VERSION_NUMBER, 0, _fpsCount, 1));
            _audioMaterial.SetVector(_playerCountAndData, new Vector4(0, 0, 0, 0));
            _fpsCount = 0;
            _fpsTime++;

            // Other things to handle every second.

            // This handles wrapping of the ElapsedTime so we don't lose precision
            // onthe floating point.
            const double ElapsedTimeMSWBoundary = 1024;
            if (_elapsedTime >= ElapsedTimeMSWBoundary)
            {
                // For particularly long running instances, i.e. several days, the first
                // few frames will be spent federating _elapsedTime into _elapsedTimeMSW.
                // This is fine.  It just means over time, the
                _fpsTime = 0;
                _elapsedTime -= ElapsedTimeMSWBoundary;
                _elapsedTimeMSW++;
            }

            // Finely adjust our network time estimate if needed.
            int networkTimeMSNow = (int)(Time.time * 1000.0f);
            int networkTimeDelta = networkTimeMSNow - _networkTimeMS;
            switch (networkTimeDelta)
            {
                case > 3000:
                case < -3000:
                    // Major upset, reset.
                    _networkTimeMS = networkTimeMSNow;
                    break;
                default:
                    // Slowly correct the timebase.
                    _networkTimeMS += networkTimeDelta / 20;
                    break;
            }
        }

        private void UpdateGlobalString(int nameID, string input)
        {
            const int maxLength = 32;
            if (input.Length > maxLength)
            {
                input = input.Substring(0, maxLength);
            }

            // Get unicode codepoints
            int[] codePoints = new int[input.Length];
            int codePointsLength = 0;
            for (int i = 0; i < input.Length; i++)
            {
                codePoints[codePointsLength++] = char.ConvertToUtf32(input, i);
                if (char.IsHighSurrogate(input[i]))
                {
                    i += 1;
                }
            }

            // Pack them into vectors
            Vector4[] vecs = new Vector4[maxLength / 4]; // 4 chars per vector
            int j = 0;
            for (int i = 0; i < vecs.Length; i++)
            {
                if (j < codePoints.Length)
                {
                    vecs[i].x = IntToFloatBits24Bit((uint)codePoints[j++]);
                }
                else
                {
                    break;
                }

                if (j < codePoints.Length)
                {
                    vecs[i].y = IntToFloatBits24Bit((uint)codePoints[j++]);
                }
                else
                {
                    break;
                }

                if (j < codePoints.Length)
                {
                    vecs[i].z = IntToFloatBits24Bit((uint)codePoints[j++]);
                }
                else
                {
                    break;
                }

                if (j < codePoints.Length)
                {
                    vecs[i].w = IntToFloatBits24Bit((uint)codePoints[j++]);
                }
                else
                {
                    break;
                }
            }

            // Expose the vectors to shader
            _audioMaterial.SetVectorArray(nameID, vecs);
        }

        private void SendAudioOutputData()
        {
            if (_audioSource == null)
            {
                return;
            }

            _audioSource.GetOutputData(_audioFramesL, 0);                // left channel

            if (_rightChannelTestCounter > 0)
            {
                if (_ignoreRightChannel)
                {
                    Array.Copy(_audioFramesL, 0, _audioFramesR, 0, 4092);
                }
                else
                {
                    _audioSource.GetOutputData(_audioFramesR, 1);
                }

                _rightChannelTestCounter--;
            }
            else
            {
                _rightChannelTestCounter = RIGHT_CHANNEL_TEST_DELAY;      // reset test countdown
                _audioFramesR[0] = 0f;                                  // reset tested array element to zero just in case
                _audioSource.GetOutputData(_audioFramesR, 1);            // right channel test
                _ignoreRightChannel = _audioFramesR[0] == 0f;
            }

            Array.Copy(_audioFramesL, 0, _samples, 0, 1023); // 4092 - 1023 * 4
            _audioMaterial.SetFloatArray(_samples0L, _samples);
            Array.Copy(_audioFramesL, 1023, _samples, 0, 1023); // 4092 - 1023 * 3
            _audioMaterial.SetFloatArray(_samples1L, _samples);
            Array.Copy(_audioFramesL, 2046, _samples, 0, 1023); // 4092 - 1023 * 2
            _audioMaterial.SetFloatArray(_samples2L, _samples);
            Array.Copy(_audioFramesL, 3069, _samples, 0, 1023); // 4092 - 1023 * 1
            _audioMaterial.SetFloatArray(_samples3L, _samples);

            Array.Copy(_audioFramesR, 0, _samples, 0, 1023); // 4092 - 1023 * 4
            _audioMaterial.SetFloatArray(_samples0R, _samples);
            Array.Copy(_audioFramesR, 1023, _samples, 0, 1023); // 4092 - 1023 * 3
            _audioMaterial.SetFloatArray(_samples1R, _samples);
            Array.Copy(_audioFramesR, 2046, _samples, 0, 1023); // 4092 - 1023 * 2
            _audioMaterial.SetFloatArray(_samples2R, _samples);
            Array.Copy(_audioFramesR, 3069, _samples, 0, 1023); // 4092 - 1023 * 1
            _audioMaterial.SetFloatArray(_samples3R, _samples);
        }
    }
}
