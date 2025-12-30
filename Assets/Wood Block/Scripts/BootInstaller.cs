using Kimicu.YandexGames;
using System.Collections;
using UnityEngine;

namespace WoodBlock
{
	public sealed class BootInstaller : MonoBehaviour
	{
        [SerializeField] private string _nextScene;

		private IEnumerator Start()
		{
			yield return YandexGamesSdk.Initialize(); // Initialize SDK.
            Debug.Log("YandexGamesSdk инициализирован.");

            yield return Cloud.Initialize(); // Initialize data.
            Debug.Log("Cloud инициализирован.");

            yield return Billing.Initialize(); // Initialize purchases.
            Debug.Log("Billing инициализирован.");

            Advertisement.Initialize(); // Initialize advert.
            Debug.Log("Advertisement инициализирован.");

            WebApplication.Initialize();
            Debug.Log("WebApplication инициализирован.");

            PlayerInput.Initialize();
			LoadPlayerData();

            SceneLoader.LoadScene(_nextScene);
		}

		public void LoadPlayerData() => PlayerBag.LoadOrCreate();
	}
}