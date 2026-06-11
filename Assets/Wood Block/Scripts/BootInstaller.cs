using UnityEngine;

namespace WoodBlock
{
	public sealed class BootInstaller : MonoBehaviour
	{
        public enum PlatformProviderType
        {
            Playgama,
            MockEditor
        }

        [SerializeField] private PlatformProviderType _providerType;
        [SerializeField] private string _nextScene;
        [SerializeField] private MusicPlayer _musicPlayer;
		private async void Start()
		{
            SelectProvider();

            await PlatformSDK.Initialize();

            await Billings.Initialize(true);

            await DataSaver.Initialize();

            _musicPlayer?.Initialize();

            PlayerInput.Initialize();

            PlatformSDK.SendGameReady();

            SceneLoader.LoadScene(_nextScene);
		}

        private void SelectProvider()
        {
            switch (_providerType)
            {
                case PlatformProviderType.Playgama:
                    PlatformSDK.InitializeProvider(new PlaygamaPlatformProvider());
                    break;
                case PlatformProviderType.MockEditor:
                    PlatformSDK.InitializeProvider(new MockPlatformProvider());
                    break;
            }

        }
	}
}