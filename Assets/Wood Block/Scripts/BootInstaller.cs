using Kimicu.YandexGames;
using Lean.Localization;
using System.Collections;
using System.Threading.Tasks;
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

            yield return YandexCurrencyService.Initialize();
            Debug.Log("YandexCurrenceService инициализирован.");

            WebApplication.Initialize();
            Debug.Log("WebApplication инициализирован.");

            YandexGamesSdk.GameReady();

            if(!DataSaver.HasSaves(SaveKeys.MusicVolume))
                DataSaver.Save(SaveKeys.MusicVolume,1f);
            if(!DataSaver.HasSaves(SaveKeys.SoundsVolume))
                DataSaver.Save(SaveKeys.SoundsVolume, 1f);
            if (!DataSaver.HasSaves(SaveKeys.CurrentLevel))
                DataSaver.Save(SaveKeys.CurrentLevel,1);
            if (!DataSaver.HasSaves(SaveKeys.SelectedBackgroundId))
                DataSaver.Save(SaveKeys.SelectedBackgroundId, "base_bg");
            if (!DataSaver.HasSaves(SaveKeys.SelectedSkinId))
                DataSaver.Save(SaveKeys.SelectedSkinId, "base_skin");


            PlayerInput.Initialize();
			LoadPlayerData();

            SceneLoader.LoadScene(_nextScene);
		}

		public void LoadPlayerData() => PlayerBag.LoadOrCreate();
	}
}