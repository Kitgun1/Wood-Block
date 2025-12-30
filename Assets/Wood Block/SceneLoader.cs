using UnityEngine;
using UnityEngine.SceneManagement;

namespace WoodBlock
{
    public sealed class SceneLoader : MonoBehaviour
    {
        public void MoveToScene(string sceneName) => LoadScene(sceneName);

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}