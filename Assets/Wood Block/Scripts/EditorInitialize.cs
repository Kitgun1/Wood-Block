namespace WoodBlock
{
    public class EditorInitialize : UnityEngine.MonoBehaviour
    {
        private void Awake()
        {
            PlayerInput.Initialize();
        }
    }
}