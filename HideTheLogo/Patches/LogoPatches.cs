using System.Threading.Tasks;
using HarmonyLib;
using IPA.Utilities.Async;
using UnityEngine;

namespace HideTheLogo.Patches
{
    [HarmonyPatch]
    internal class LogoPatches
    {
        private static Transform _defaultMenuEnvironment;
        private static Transform _thunderMenuEnvironment;

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
            if (_thunderMenuEnvironment == null)
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

                _thunderMenuEnvironment.Find("MetallicaLogo").gameObject.SetActive(false);
                _thunderMenuEnvironment.Find("BS_Logo").gameObject.SetActive(false);
            });
        }
    }
}