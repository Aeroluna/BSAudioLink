using HarmonyLib;
using System;
using UnityEngine;

namespace AudioLink.Patches
{
    [HarmonyPatch(typeof(AudioTimeSyncController))]
    [HarmonyPatch(nameof(AudioTimeSyncController.StartSong), new Type[] { typeof(float) })]
    internal class PAudioTimeSyncController
    {
        internal static void Postfix(ref AudioSource ____audioSource)
        {
            try
            {
                Plugin.AudioLink?.Configure(____audioSource);
            }
            catch (Exception)
            {

            }
        }
    }
}
