using UnityEngine;
using UnityEngine.SceneManagement;

namespace WoodBlock
{
    public sealed class GamePause : MonoBehaviour
    {
        public void StartPause()
        {
            LifeTime.Instance.enabled = false;
        }

        public void StopPause()
        {
            LifeTime.Instance.enabled = true;
        }
    }
}