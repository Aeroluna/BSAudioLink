using System;
using System.Reflection;
using AudioLink.Extras;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace AudioLink.Providers
{
    // NalulunaMenu creates its AudioSource and has to be handled specially
    internal class NalulunaMenuProvider : IInitializable, IDisposable
    {
        private static readonly Harmony _harmony = new("aeroluna.AudioLink");

        private static AudioLink? _audioLink;

        [UsedImplicitly]
        private NalulunaMenuProvider(AudioLink audioLink)
        {
            _audioLink = audioLink;
        }

        public void Initialize()
        {
            Plugin.Logger.Info("Hooking NalulunaMenu...");
            _harmony.PatchAll(typeof(NalulunaMenuProvider));
        }

        public void Dispose()
        {
            _harmony.UnpatchSelf();
        }

        // hopefully affinity will implement targetmethod (hopefully) but until then we have to use harmony
        [UsedImplicitly]
        [HarmonyTargetMethod]
        private static MethodBase TargetMethod()
        {
            Type typeToPatch = Type.GetType("NalulunaMenu.NalulunaMenuController,NalulunaMenu")
                               ?? throw new InvalidOperationException("Unable to resolve [NalulunaMenuController] type.");
            return AccessTools.Method(typeToPatch, "ResumeMusicCoroutine");
        }

        [UsedImplicitly]
        [HarmonyPostfix]
        private static void NalulunaMenuProvide(AudioSource ____audioSource)
        {
            _audioLink?.SetAudioSource(____audioSource);
        }
    }
}
