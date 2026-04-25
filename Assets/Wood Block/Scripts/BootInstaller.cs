using Kimicu.YandexGames;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace WoodBlock
{
	public sealed class BootInstaller : MonoBehaviour
	{
        [SerializeField] private string _nextScene;
        [SerializeField] private MusciPlayer _musicPlayer;

		private IEnumerator Start()
		{
			yield return YandexGamesSdk.Initialize(); // Initialize SDK.
            Debug.Log("YandexGamesSdk инициализирован.");

            yield return Cloud.Initialize(); // Initialize data.
            Debug.Log("Cloud инициализирован.");

            yield return Billing.Initialize(Agava.YandexGames.ProductPictureSize.svg); // Initialize purchases.
            Debug.Log("Billing инициализирован.");

            yield return YandexCurrencyService.BillingYandexCurrencySetup();
            Debug.Log("Yandex Currency инициализирован");

            Advertisement.Initialize(); // Initialize advert.
            Debug.Log("Advertisement инициализирован.");

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
            if (!DataSaver.HasSaves(SaveKeys.BestScore))
                DataSaver.Save(SaveKeys.BestScore, 0);

            _musicPlayer.Initialize();

            PlayerInput.Initialize();
			LoadPlayerData();

            SceneLoader.LoadScene(_nextScene);
		}

		public void LoadPlayerData() => PlayerBag.LoadOrCreate();
	}
}