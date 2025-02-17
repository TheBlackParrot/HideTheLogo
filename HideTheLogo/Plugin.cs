using System.Reflection;
using HarmonyLib;
using IPA;
using JetBrains.Annotations;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace HideTheLogo
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    [UsedImplicitly]
    public class Plugin
    {
        internal static IPALogger Log { get; private set; }
        private static Harmony _harmony;

        [Init]
        [UsedImplicitly]
        public void Init(Zenjector zenjector, IPALogger logger)
        {
            Log = logger;

            zenjector.UseLogger(logger);
            zenjector.UseMetadataBinder<Plugin>();
        }
        
        [OnEnable]
        [UsedImplicitly]
        public void OnEnable()
        {
            _harmony = new Harmony("TheBlackParrot.HideTheLogo");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        [UsedImplicitly]
        public void OnDisable()
        {
            _harmony.UnpatchSelf();
        }
    }
}