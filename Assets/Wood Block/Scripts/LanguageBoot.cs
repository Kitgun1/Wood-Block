using Lean.Localization;
using Playgama;
using UnityEngine;
using UnityEngine.Events;

public class LanguageBoot : MonoBehaviour
{
    [SerializeField] private UnityEvent _eventsAfterBoot;
    private void Start()
    {
        string language = Bridge.platform.language == "en" ? "English" : "Russian";
        LeanLocalization.SetCurrentLanguageAll(language);
        _eventsAfterBoot?.Invoke();
    }
}
