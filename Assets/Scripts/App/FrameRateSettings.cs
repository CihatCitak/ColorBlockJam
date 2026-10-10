using UnityEngine;

public static class FrameRateSettings
{
    private const int MobileTargetFrameRate = 60;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Apply()
    {
        if (!Application.isMobilePlatform)
            return;

        // Mobile ignores vSyncCount; frame rate is driven by targetFrameRate (default is 30).
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = MobileTargetFrameRate;
    }
}
