using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Playgama;
using Playgama.Modules.Advertisement;
using Playgama.Modules.Platform;
using Playgama.Modules.Storage;

public class PlaygamaPlatformProvider : IPlatformProvider, IAdsProvider, IStorageProvider, IPaymentsProvider
{
    public IAdsProvider Ads => this;
    public IStorageProvider Storage => this;
    public IPaymentsProvider Payments => this;

    public string Language => Bridge.platform?.language ?? "en";

    public bool IsSupported => Bridge.payments?.isSupported ?? false;

    public event Action<bool> OnPauseStateChanged;
    public event Action<bool> OnAudioStateChanged;
    public event Action OnAdStarted;
    public event Action OnAdCompleted;

    public async UniTask Initialize()
    {
        if (Bridge.platform != null)
        {
            Bridge.platform.pauseStateChanged += InvokePauseStateChanged;
            Bridge.platform.audioStateChanged += InvokeAudioStateChanged;
        }

        if (Bridge.advertisement != null)
        {
            Bridge.advertisement.interstitialStateChanged += OnPlaygamaInterstitialStateChanged;
            Bridge.advertisement.rewardedStateChanged += OnPlaygamaRewardedStateChanged;
        }

        await UniTask.CompletedTask;
    }

    private void InvokePauseStateChanged(bool isPaused) => OnPauseStateChanged?.Invoke(isPaused);
    private void InvokeAudioStateChanged(bool isAudioEnabled) => OnAudioStateChanged?.Invoke(isAudioEnabled);

    private void OnPlaygamaInterstitialStateChanged(InterstitialState state)
    {
        if (state == InterstitialState.Opened)
        {
            OnAdStarted?.Invoke();
        }
        else if (state == InterstitialState.Closed || state == InterstitialState.Failed)
        {
            OnAdCompleted?.Invoke();
        }
    }

    private void OnPlaygamaRewardedStateChanged(RewardedState state)
    {
        if (state == RewardedState.Opened)
        {
            OnAdStarted?.Invoke();
        }
        else if (state == RewardedState.Closed || state == RewardedState.Failed || state == RewardedState.Rewarded)
        {
            OnAdCompleted?.Invoke();
        }
    }

    public void SendGameReady()
    {
        Bridge.platform?.SendMessage(PlatformMessage.GameReady);
    }

    // Ads
    public void ShowInterstitial()
    {
        Bridge.advertisement?.ShowInterstitial();
    }

    public void ShowRewarded(Action onRewarded, Action onClosedOrFailed)
    {
        Action<RewardedState> onAwarded = null;
        onAwarded = (RewardedState state) =>
        {
            if (state == RewardedState.Rewarded)
            {
                onRewarded?.Invoke();
                Bridge.advertisement.rewardedStateChanged -= onAwarded;
            }
            else if (state == RewardedState.Closed || state == RewardedState.Failed)
            {
                onClosedOrFailed?.Invoke();
                Bridge.advertisement.rewardedStateChanged -= onAwarded;
            }
        };

        Bridge.advertisement.rewardedStateChanged += onAwarded;
        Bridge.advertisement.ShowRewarded();
    }

    // Storage
    public void Set(string key, string value)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.storage?.Set(key, value);
#endif
    }

    public void Delete(string key)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Bridge.storage?.Set(key, "", storageType: StorageType.PlatformInternal);
#endif
    }

    // Payments
    public void GetCatalog(Action<bool, List<Dictionary<string, string>>> callback)
    {
        Bridge.payments.GetCatalog((success, catalog) => callback?.Invoke(success, catalog));
    }

    public void Purchase(string productId, Action<bool> callback)
    {
        Bridge.payments.Purchase(productId, (success, result) => callback?.Invoke(success));
    }

    public void Consume(string productId, Action<bool> callback)
    {
        Bridge.payments.ConsumePurchase(productId, (success, result) => callback?.Invoke(success));
    }
}
