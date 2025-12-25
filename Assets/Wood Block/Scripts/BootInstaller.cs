using System.Collections;
using Kimicu.YandexGames;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WoodBlock
{
	public class BootInstaller : MonoBehaviour
	{
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
			LoadScene(1);
		}

		public void LoadPlayerData() => PlayerBag.LoadOrCreate();

		public void LoadScene(int value) => SceneManager.LoadScene(value);
	}
}