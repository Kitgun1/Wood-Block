using Playgama;
using Playgama.Modules.Platform;
using UnityEngine;

namespace WoodBlock
{
	public sealed class BootInstaller : MonoBehaviour
	{
        [SerializeField] private string _nextScene;
        [SerializeField] private MusicPlayer _musicPlayer;

		private async void Start()
		{

            await Billings.Initialize(true);
            Debug.Log("Billings was initialized!");

            await DataSaver.Initialize();

            _musicPlayer.Initialize();

            PlayerInput.Initialize();

            Bridge.platform.SendMessage(PlatformMessage.GameReady);

            SceneLoader.LoadScene(_nextScene);
		}
	}
}