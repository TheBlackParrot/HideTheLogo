using System;
using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities.Async;
using SiraUtil.Converters;
using UnityEngine;

namespace HideTheLogo.Patches
{
    [HarmonyPatch]
    internal class LogoPatches
    {
        private static Transform _defaultMenuEnvironment;
        private static Transform _thunderMenuEnvironment;
        
        // Metallica environment only existed between 1.40.0 and 1.40.2, it was removed in 1.40.3
        private static bool GameVersionHasMetallica => IPA.Utilities.UnityGame.GameVersion.SemverValue?.Major == 1 &&
                                                       IPA.Utilities.UnityGame.GameVersion.SemverValue?.Minor == 40 &&
                                                       IPA.Utilities.UnityGame.GameVersion.SemverValue?.Patch <= 2;

        private static async Task WaitABit()
        {
            // lazy. it works. w/e
            await Task.Delay(1);
        }
        
        // ReSharper disable once InconsistentNaming
        [HarmonyPatch(typeof(MenuEnvironmentManager), "Start")]
        [HarmonyPatch(typeof(MenuEnvironmentManager), "ShowEnvironmentType")]
        [HarmonyPriority(int.MinValue)]
        internal static void Postfix(MenuEnvironmentManager __instance)
        {
            if (_defaultMenuEnvironment == null)
            {
                _defaultMenuEnvironment = __instance.transform.Find("DefaultMenuEnvironment");
            }
            if (_thunderMenuEnvironment == null && GameVersionHasMetallica)
            {
                _thunderMenuEnvironment = __instance.transform.Find("ThunderMenuEnvironment");
            }

            // doing it this way to get around the MenuSelector mod re-activating these. just ensuring we run *last*
            UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                Plugin.Log.Info("yeet");
                
                await WaitABit();
                
                _defaultMenuEnvironment.Find("Logo").gameObject.SetActive(false);
                _defaultMenuEnvironment.Find("GlowLines").gameObject.SetActive(false);
                _defaultMenuEnvironment.Find("GlowLines (1)").gameObject.SetActive(false);

                if (GameVersionHasMetallica)
                {
                    _thunderMenuEnvironment.Find("MetallicaLogo").gameObject.SetActive(false);
                    _thunderMenuEnvironment.Find("BS_Logo").gameObject.SetActive(false);
                }
            });
        }
    }
}