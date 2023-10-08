using System.Collections;
using Agava.YandexGames;
using KiYandexSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using Billing = KiYandexSDK.Billing;

namespace WoodBlock
{
    public class BootInstaller : MonoBehaviour
    {
        private IEnumerator Start()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            yield return YandexGamesSdk.Initialize(); // Initialize Agava SDK.
            yield return YandexData.Initialize(); // Initialize data.
            yield return Billing.Initialize(); // Initialize purchases.
            AdvertSDK.AdvertInitialize();  // Initialize advert.
            WebGL.Initialize();  // Initialize WebGL.
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