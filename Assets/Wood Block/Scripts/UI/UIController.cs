using System.Linq;
using KimicuUtility;
using UnityEngine;

namespace WoodBlock
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private WindowType _startingWindow;
        [SerializeField] private DictionaryWindows _windows = new();

        private void Awake()
        {
            OpenWindow(new[] { _startingWindow });
        }

        public void OpenWindow(WindowType[] windowType)
        {
            foreach (var pair in _windows)
            {
                pair.Value.Active(windowType.Any(w => w == pair.Key));
            }

            LayoutsUpdater.UpdateAllLayouts();
        }
    }
}