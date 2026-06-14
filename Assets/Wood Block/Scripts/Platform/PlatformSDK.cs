using System;
using Cysharp.Threading.Tasks;

public static class PlatformSDK
{
    private static IPlatformProvider _provider;

    public static void InitializeProvider(IPlatformProvider provider)
    {
        if (_provider != null)
        {
            _provider.OnPauseStateChanged -= ForwardPauseStateChanged;
            _provider.OnAudioStateChanged -= ForwardAudioStateChanged;
            if (_provider.Ads != null)
            {
                _provider.Ads.OnAdStarted -= ForwardAdStarted;
                _provider.Ads.OnAdCompleted -= ForwardAdCompleted;
            }
        }

        _provider = provider;

        if (_provider != null)
        {
            _provider.OnPauseStateChanged += ForwardPauseStateChanged;
            _provider.OnAudioStateChanged += ForwardAudioStateChanged;
            if (_provider.Ads != null)
            {
                _provider.Ads.OnAdStarted += ForwardAdStarted;
                _provider.Ads.OnAdCompleted += ForwardAdCompleted;
            }
        }
    }

    public static IPlatformProvider Provider => _provider;

    public static UniTask Initialize() => _provider != null ? _provider.Initialize() : UniTask.CompletedTask;
    public static void SendGameReady() => _provider?.SendGameReady();
    public static string Language => _provider?.Language ?? "en";

    public static event Action<bool> OnPauseStateChanged;
    public static event Action<bool> OnAudioStateChanged;

    public static void ShowInterstitial() => _provider?.Ads?.ShowInterstitial();
    
    public static void ShowRewarded(Action onRewarded, Action onClosedOrFailed)
    {
        _provider?.Ads?.ShowRewarded(onRewarded, onClosedOrFailed);
    }

    public static event Action OnAdStarted;
    public static event Action OnAdCompleted;

    private static void ForwardPauseStateChanged(bool isPaused) => OnPauseStateChanged?.Invoke(isPaused);
    private static void ForwardAudioStateChanged(bool isAudioEnabled) => OnAudioStateChanged?.Invoke(isAudioEnabled);
    private static void ForwardAdStarted() => OnAdStarted?.Invoke();
    private static void ForwardAdCompleted() => OnAdCompleted?.Invoke();

    public static IStorageProvider Storage => _provider?.Storage;
    public static IPaymentsProvider Payments => _provider?.Payments;
}
