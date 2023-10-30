using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR && UNITY_WEBGL
using Kimicu.YandexGames;
using Agava.YandexGames;
using Billing = Kimicu.YandexGames.Billing;
#endif

namespace WoodBlock
{
    public class BootInstaller : MonoBehaviour
    {
        private IEnumerator Start()
        {
#if UNITY_EDITOR && UNITY_WEBGL
            yield return YandexGamesSdk.Initialize(); // Initialize Agava SDK.
            yield return YandexData.Initialize(); // Initialize data.
            yield return Billing.Initialize(); // Initialize purchases.
            Advert.AdvertInitialize();  // Initialize advert.
            WebGL.InitializeListener();  // Initialize WebGL.
#else
            yield return null;
#endif
            PlayerInput.Initialize();
            LoadPlayerData();
            LoadScene(1);
        }

        public void LoadPlayerData() => PlayerBag.LoadOrCreate();

        public void LoadScene(int value) => SceneManager.LoadScene(value);
    }
}