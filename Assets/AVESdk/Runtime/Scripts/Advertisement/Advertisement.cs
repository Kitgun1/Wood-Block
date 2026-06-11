using System;

public static class Advertisement
{
    public static void ShowAwardedAdd(Action onAwardedCallback)
    {
        PlatformSDK.ShowRewarded(onAwardedCallback, null);
    }
}
