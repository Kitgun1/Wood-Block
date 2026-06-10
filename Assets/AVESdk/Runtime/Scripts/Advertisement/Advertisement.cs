using Playgama;
using Playgama.Modules.Advertisement;
using System;

public static class Advertisement
{
    public static void ShowAwardedAdd(Action onAwardedCallback)
    {
        Action<RewardedState> onAwarded = null;
        onAwarded = (RewardedState state) =>
        {
            if (state == RewardedState.Rewarded)
            {
                onAwardedCallback?.Invoke();
                Bridge.advertisement.rewardedStateChanged -= onAwarded;
            }
            else if (state == RewardedState.Closed || state == RewardedState.Failed)
            {
                Bridge.advertisement.rewardedStateChanged -= onAwarded;
            }
        };

        Bridge.advertisement.rewardedStateChanged += onAwarded;
        Bridge.advertisement.ShowRewarded();
    }
}
