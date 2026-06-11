using Lean.Localization;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LocalizationDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    private List<string> _languages = new List<string>();

    private void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();

        // Fetch all registered languages from LeanLocalization
        _languages.Clear();
        foreach (var langName in LeanLocalization.CurrentLanguages.Keys)
        {
            _languages.Add(langName);
        }

        // Fallback to defaults if no languages are loaded/registered yet
        if (_languages.Count == 0)
        {
            _languages.Add("English");
            _languages.Add("Russian");
        }

        // Populate the dropdown options
        _dropdown.ClearOptions();
        _dropdown.AddOptions(_languages);

        // Adjust template window height to fit the number of items
        AdjustTemplateHeight();

        // Get the currently selected language, fallback to PlayerPrefs to enforce it across scenes
        string currentLanguage = PlayerPrefs.GetString("LeanLocalization.CurrentLanguage");
        if (string.IsNullOrEmpty(currentLanguage))
        {
            currentLanguage = LeanLocalization.GetFirstCurrentLanguage();
        }
        else
        {
            LeanLocalization.SetCurrentLanguageAll(currentLanguage);
        }
        int currentIndex = _languages.IndexOf(currentLanguage);

        // Set the active value in the dropdown without triggering onValueChanged immediately
        _dropdown.SetValueWithoutNotify(currentIndex >= 0 ? currentIndex : 0);

        _dropdown.onValueChanged.AddListener(ChangeLang);
    }
    
    public void ChangeLang(int langIndex)
    {
        if (langIndex >= 0 && langIndex < _languages.Count)
        {
            LeanLocalization.SetCurrentLanguageAll(_languages[langIndex]);
            PlayerPrefs.SetString("LeanLocalization.CurrentLanguage", _languages[langIndex]);
            PlayerPrefs.Save();
        }
    }

    private void AdjustTemplateHeight()
    {
        if (_dropdown == null || _dropdown.template == null) return;

        RectTransform templateRect = _dropdown.template;
        RectTransform itemRect = templateRect.Find("Viewport/Content/Item")?.GetComponent<RectTransform>();

        if (itemRect != null)
        {
            float itemHeight = itemRect.rect.height;
            int count = _languages.Count;

            // Calculate exact height of the dropdown window
            float padding = 2f; // Small padding for borders/spacers
            float calculatedHeight = (itemHeight * count) + padding;

            // Cap the dropdown height (e.g. show scrollbar if there are more than 6 options)
            float maxHeight = (itemHeight * 6) + padding;
            if (calculatedHeight > maxHeight)
            {
                calculatedHeight = maxHeight;
            }

            // Apply new height to the Dropdown Template container
            Vector2 size = templateRect.sizeDelta;
            size.y = calculatedHeight;
            templateRect.sizeDelta = size;
        }
    }
}
