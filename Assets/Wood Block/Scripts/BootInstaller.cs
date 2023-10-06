using UnityEngine;
using UnityEngine.SceneManagement;

namespace WoodBlock
{
    public class BootInstaller : MonoBehaviour
    {
        private void Awake()
        {
            PlayerInput.Initialize();
        }

        public void LoadPlayerData() => PlayerBag.LoadOrCreate();

        public void LoadScene(int value) => SceneManager.LoadScene(value);
    }
}