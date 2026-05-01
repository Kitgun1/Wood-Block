using Playgama;
using Playgama.Modules.Advertisement;
using System;

public static class Advertisement
{
    public static void ShowAwardedAdd(Action onAwardedCallback)
    {
        Action<RewardedState> onAwarded  = delegate(RewardedState state)
        {
            if (state == RewardedState.Rewarded)
            {
                onAwardedCallback?.Invoke();
            }
        };

        Bridge.advertisement.rewardedStateChanged += onAwarded;
        Bridge.advertisement.ShowRewarded();
        Bridge.advertisement.rewardedStateChanged -= onAwarded;
    }
}
