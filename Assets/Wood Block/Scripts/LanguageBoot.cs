using Kimicu.YandexGames;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Events;

public class LanguageBoot : MonoBehaviour
{
    [SerializeField] private UnityEvent _eventsAfterBoot;
    private void Start()
    {
        string language = YandexGamesSdk.Environment.i18n.lang == "en" ? "English" : "Russian";
        LeanLocalization.SetCurrentLanguageAll(language);
        _eventsAfterBoot?.Invoke();
    }
}
