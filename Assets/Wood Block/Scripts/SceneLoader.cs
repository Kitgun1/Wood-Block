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
        public static void LoadScene(int buildIndex)
        {
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        }

        public static void LoadCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
        }

        public static void LoadNextScene()
        {
            var scene = SceneManager.GetActiveScene();
            var index = scene.buildIndex + 1;
            try
            {
                SceneManager.LoadScene(index);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[SceneLoader] Failed to load next scene at index {index}, loading scene 1: {ex.Message}");
                SceneManager.LoadScene(1);
            }
        }
    }
}