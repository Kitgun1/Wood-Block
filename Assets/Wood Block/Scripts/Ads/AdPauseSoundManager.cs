using UnityEngine;
using UnityEngine.SceneManagement;

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
            PlatformSDK.OnPauseStateChanged += OnPauseStateChanged;
            PlatformSDK.OnAudioStateChanged += OnAudioStateChanged;
            PlatformSDK.OnAdStarted += OnAdStarted;
            PlatformSDK.OnAdCompleted += OnAdCompleted;

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
                PlatformSDK.OnPauseStateChanged -= OnPauseStateChanged;
                PlatformSDK.OnAudioStateChanged -= OnAudioStateChanged;
                PlatformSDK.OnAdStarted -= OnAdStarted;
                PlatformSDK.OnAdCompleted -= OnAdCompleted;
            }
            catch (System.Exception ex)
            {
                // Игнорируем исключения при закрытии приложения, так как ресурсы Bridge уже могут быть очищены
                Debug.LogWarning($"[AdPauseSoundManager] Ignored exception during platform unsubscribe: {ex.Message}");
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

        private void OnAdStarted()
        {
            _isAdPlaying = true;
            UpdateGameState();
        }

        private void OnAdCompleted()
        {
            _isAdPlaying = false;
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
