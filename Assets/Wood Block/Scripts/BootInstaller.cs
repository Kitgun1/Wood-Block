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
			yield return Cloud.Initialize(); // Initialize data.
			yield return Billing.Initialize(); // Initialize purchases.
			Advertisement.Initialize(); // Initialize advert.
			WebApplication.Initialize();

			PlayerInput.Initialize();
			LoadPlayerData();
			LoadScene(1);
		}

		public void LoadPlayerData() => PlayerBag.LoadOrCreate();

		public void LoadScene(int value) => SceneManager.LoadScene(value);
	}
}