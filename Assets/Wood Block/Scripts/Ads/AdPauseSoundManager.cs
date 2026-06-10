using UnityEngine;
using UnityEngine.SceneManagement;
using Playgama;
using Playgama.Modules.Advertisement;

namespace WoodBlock
{
    public sealed class AdPauseSoundManager : MonoBehaviour
    {
        private static AdPauseSoundManager _instance;
        private bool _isPlatformPaused = false;
        private bool _isPlatformAudioMuted = false;
        private bool _isAdPlaying = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_instance != null) return;

            GameObject go = new GameObject("AdPauseSoundManager");
            _instance = go.AddComponent<AdPauseSoundManager>();
            DontDestroyOnLoad(go);
        }

        private void Start()
        {
            // Subscribe to platform pause and audio events
            if (Bridge.platform != null)
            {
                Bridge.platform.pauseStateChanged += OnPauseStateChanged;
                Bridge.platform.audioStateChanged += OnAudioStateChanged;
            }

            // Subscribe to advertisement events
            if (Bridge.advertisement != null)
            {
                Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;
                Bridge.advertisement.rewardedStateChanged += OnRewardedStateChanged;
            }

            UpdateGameState();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnDestroy()
        {
            try
            {
                if (Bridge.platform != null)
                {
                    Bridge.platform.pauseStateChanged -= OnPauseStateChanged;
                    Bridge.platform.audioStateChanged -= OnAudioStateChanged;
                }
            }
            catch (System.Exception ex)
            {
                // Игнорируем исключения при закрытии приложения, так как ресурсы Bridge уже могут быть очищены
                Debug.LogWarning($"[AdPauseSoundManager] Ignored exception during platform unsubscribe: {ex.Message}");
            }

            try
            {
                if (Bridge.advertisement != null)
                {
                    Bridge.advertisement.interstitialStateChanged -= OnInterstitialStateChanged;
                    Bridge.advertisement.rewardedStateChanged -= OnRewardedStateChanged;
                }
            }
            catch (System.Exception ex)
            {
                // Игнорируем исключения при закрытии приложения
                Debug.LogWarning($"[AdPauseSoundManager] Ignored exception during ad unsubscribe: {ex.Message}");
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Re-apply state when a new scene is loaded (in case new LifeTime is instantiated)
            UpdateGameState();
        }

        private void OnPauseStateChanged(bool isPaused)
        {
            _isPlatformPaused = isPaused;
            UpdateGameState();
        }

        private void OnAudioStateChanged(bool isAudioEnabled)
        {
            _isPlatformAudioMuted = !isAudioEnabled;
            UpdateGameState();
        }

        private void OnInterstitialStateChanged(InterstitialState state)
        {
            if (state == InterstitialState.Opened)
            {
                _isAdPlaying = true;
            }
            else if (state == InterstitialState.Closed || state == InterstitialState.Failed)
            {
                _isAdPlaying = false;
            }
            UpdateGameState();
        }

        private void OnRewardedStateChanged(RewardedState state)
        {
            if (state == RewardedState.Opened)
            {
                _isAdPlaying = true;
            }
            else if (state == RewardedState.Closed || state == RewardedState.Failed || state == RewardedState.Rewarded)
            {
                _isAdPlaying = false;
            }
            UpdateGameState();
        }

        private void UpdateGameState()
        {
            bool shouldPause = _isPlatformPaused || _isAdPlaying;
            bool shouldMute = _isPlatformAudioMuted || _isAdPlaying;

            // Apply Game Pause
            Time.timeScale = shouldPause ? 0f : 1f;
            if (LifeTime.Instance != null)
            {
                LifeTime.Instance.enabled = !shouldPause;
            }

            // Apply Game Mute
            AudioListener.pause = shouldMute;
            AudioListener.volume = shouldMute ? 0f : 1f;
        }
    }
}
