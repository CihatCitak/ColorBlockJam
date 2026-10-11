using UnityEngine;

namespace Game.App
{
    internal static class FrameRateSettings
    {
        private const int MobileTargetFrameRate = 60;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Apply()
        {
            if (!Application.isMobilePlatform)
                return;

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = MobileTargetFrameRate;
        }
    }
}
