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

        public void LoadScene(int value) => SceneManager.LoadScene(value);
    }
}