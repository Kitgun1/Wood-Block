using Kimicu.YandexGames;
using Lean.Localization;
using UnityEngine;

public class LanguageBoot : MonoBehaviour
{
    private void Awake()
    {
        string language = YandexGamesSdk.Environment.i18n.lang == "en" ? "English" : "Russian";
        LeanLocalization.SetCurrentLanguageAll(language);
    }
}
