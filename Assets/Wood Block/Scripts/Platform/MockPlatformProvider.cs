using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MockPlatformProvider : IPlatformProvider, IAdsProvider, IStorageProvider, IPaymentsProvider
{
    public IAdsProvider Ads => this;
    public IStorageProvider Storage => this;
    public IPaymentsProvider Payments => this;

    public string Language 
    {
        get
        {
            if (Application.systemLanguage == SystemLanguage.Russian)
                return "ru";
            return "en";
        }
    }

    public bool IsSupported => false;

    #pragma warning disable 67
    public event Action<bool> OnPauseStateChanged;
    public event Action<bool> OnAudioStateChanged;
    public event Action OnAdStarted;
    public event Action OnAdCompleted;
    #pragma warning restore 67

    public async UniTask Initialize()
    {
        Debug.Log("[MockPlatform] Initializing Mock Platform Provider");
        await UniTask.CompletedTask;
    }

    public void SendGameReady()
    {
        Debug.Log("[MockPlatform] Game Ready sent!");
    }

    // Ads
    public void ShowInterstitial()
    {
        Debug.Log("[MockPlatform] Show Interstitial Ad");
        OnAdStarted?.Invoke();
        OnAdCompleted?.Invoke();
    }

    public void ShowRewarded(Action onRewarded, Action onClosedOrFailed)
    {
        Debug.Log("[MockPlatform] Show Rewarded Ad");
        OnAdStarted?.Invoke();
        onRewarded?.Invoke();
        OnAdCompleted?.Invoke();
    }

    // Storage
    public void Set(string key, string value)
    {
        Debug.Log($"[MockPlatform] Save data: {key} = {value}");
    }

    public void Delete(string key)
    {
        Debug.Log($"[MockPlatform] Delete data: {key}");
    }

    // Payments
    public void GetCatalog(Action<bool, List<Dictionary<string, string>>> callback)
    {
        callback?.Invoke(true, new List<Dictionary<string, string>>());
    }

    public void Purchase(string productId, Action<bool> callback)
    {
        Debug.Log($"[MockPlatform] Purchase product: {productId}");
        callback?.Invoke(true);
    }

    public void Consume(string productId, Action<bool> callback)
    {
        Debug.Log($"[MockPlatform] Consume product: {productId}");
        callback?.Invoke(true);
    }
}
