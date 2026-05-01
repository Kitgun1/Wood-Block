using Playgama;
using Playgama.Modules.Platform;
using UnityEngine;

namespace WoodBlock
{
	public sealed class BootInstaller : MonoBehaviour
	{
        [SerializeField] private string _nextScene;
        [SerializeField] private MusciPlayer _musicPlayer;

		private async void Start()
		{

            await Billings.Initialize(true);
            Debug.Log("Billings was initialized!");

            if (!DataSaver.HasSaves(SaveKeys.MusicVolume))
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

            Bridge.platform.SendMessage(PlatformMessage.GameReady);

            SceneLoader.LoadScene(_nextScene);
		}
	}
}