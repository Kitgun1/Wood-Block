using System;
using System.Linq;
using KiUtility;
using UnityEngine;

namespace WoodBlock
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private DictionaryWindows _windows = new();

        private void Awake()
        {
            OpenWindow(new[] { WindowType.StartMenu });
        }

        public void OpenWindow(WindowType[] windowType)
        {
            foreach (var pair in _windows)
            {
                pair.Value.Active(windowType.Any(w => w == pair.Key));
            }
        }
    }
}