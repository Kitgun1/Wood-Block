using System;
using Cysharp.Threading.Tasks;

public static class PlatformSDK
{
    private static IPlatformProvider _provider;

    public static void InitializeProvider(IPlatformProvider provider)
    {
        _provider = provider;
    }

    public static IPlatformProvider Provider => _provider;

    public static UniTask Initialize() => _provider != null ? _provider.Initialize() : UniTask.CompletedTask;
    public static void SendGameReady() => _provider?.SendGameReady();
    public static string Language => _provider?.Language ?? "en";

    public static event Action<bool> OnPauseStateChanged
    {
        add { if (_provider != null) _provider.OnPauseStateChanged += value; }
        remove { if (_provider != null) _provider.OnPauseStateChanged -= value; }
    }

    public static event Action<bool> OnAudioStateChanged
    {
        add { if (_provider != null) _provider.OnAudioStateChanged += value; }
        remove { if (_provider != null) _provider.OnAudioStateChanged -= value; }
    }

    public static void ShowInterstitial() => _provider?.Ads?.ShowInterstitial();
    
    public static void ShowRewarded(Action onRewarded, Action onClosedOrFailed)
    {
        _provider?.Ads?.ShowRewarded(onRewarded, onClosedOrFailed);
    }

    public static event Action OnAdStarted
    {
        add { if (_provider?.Ads != null) _provider.Ads.OnAdStarted += value; }
        remove { if (_provider?.Ads != null) _provider.Ads.OnAdStarted -= value; }
    }

    public static event Action OnAdCompleted
    {
        add { if (_provider?.Ads != null) _provider.Ads.OnAdCompleted += value; }
        remove { if (_provider?.Ads != null) _provider.Ads.OnAdCompleted -= value; }
    }

    public static IStorageProvider Storage => _provider?.Storage;
    public static IPaymentsProvider Payments => _provider?.Payments;
}
