using Lean.Localization;
using UnityEngine;
using UnityEngine.Events;

public class LanguageBoot : MonoBehaviour
{
    [SerializeField] private UnityEvent _eventsAfterBoot;
    private void Start()
    {
        string language = PlayerPrefs.GetString("LeanLocalization.CurrentLanguage");
        if (string.IsNullOrEmpty(language))
        {
            language = PlatformSDK.Language == "en" ? "English" : "Russian";
        }
        LeanLocalization.SetCurrentLanguageAll(language);
        _eventsAfterBoot?.Invoke();
    }
}
