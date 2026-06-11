using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IPlatformProvider
{
    IAdsProvider Ads { get; }
    IStorageProvider Storage { get; }
    IPaymentsProvider Payments { get; }

    UniTask Initialize();
    void SendGameReady();
    string Language { get; }

    event Action<bool> OnPauseStateChanged;
    event Action<bool> OnAudioStateChanged;
}

public interface IAdsProvider
{
    event Action OnAdStarted;
    event Action OnAdCompleted;

    void ShowInterstitial();
    void ShowRewarded(Action onRewarded, Action onClosedOrFailed);
}

public interface IStorageProvider
{
    void Set(string key, string value);
    void Delete(string key);
}

public interface IPaymentsProvider
{
    bool IsSupported { get; }
    void GetCatalog(Action<bool, List<Dictionary<string, string>>> callback);
    void Purchase(string productId, Action<bool> callback);
    void Consume(string productId, Action<bool> callback);
}
