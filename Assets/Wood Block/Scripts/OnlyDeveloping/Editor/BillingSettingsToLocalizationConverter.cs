using UnityEngine;
using UnityEditor;
using Lean.Localization;
using AYellowpaper.SerializedCollections;

public class BillingSettingsToLocalizationConverter : EditorWindow
{
    private BillingsSettings _settings;
    private LeanLocalization _localization;

    [MenuItem("Tools/Convert Billings Settings to LeanLocalization")]
    public static void ShowWindow()
    {
        GetWindow<BillingSettingsToLocalizationConverter>("Billings to LeanLoc");
    }

    private void OnEnable()
    {
        // Try to load default BillingsSettings
        _settings = Resources.Load<BillingsSettings>("BillingSettings");
        // Try to find LeanLocalization in the active scene
        _localization = FindObjectOfType<LeanLocalization>();
    }

    private void OnGUI()
    {
        GUILayout.Label("Convert Billings Settings to LeanLocalization Phrases", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _settings = (BillingsSettings)EditorGUILayout.ObjectField("Billing Settings", _settings, typeof(BillingsSettings), false);
        _localization = (LeanLocalization)EditorGUILayout.ObjectField("Lean Localization", _localization, typeof(LeanLocalization), true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Convert and Apply Translations", GUILayout.Height(30)))
        {
            if (_settings == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a BillingsSettings asset.", "OK");
                return;
            }

            if (_localization == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a LeanLocalization component or prefab.", "OK");
                return;
            }

            ConvertSettingsToLocalization();
        }
    }

    private void ConvertSettingsToLocalization()
    {
        SerializedDictionary<string, ProductSetting> catalog = null;
        _settings.GetAllCatalog(c => catalog = c, err => {
            Debug.LogError($"[Converter] Failed to load catalog: {err}");
        });

        if (catalog == null || catalog.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "Catalog is empty or failed to load.", "OK");
            return;
        }

        int addedCount = 0;
        int updatedCount = 0;

        // Record Undo for the LeanLocalization GameObject
        Undo.RegisterCompleteObjectUndo(_localization.gameObject, "Convert Billings Settings to LeanLocalization");

        foreach (var pair in catalog)
        {
            string id = pair.Key;
            ProductSetting product = pair.Value;

            if (product == null) continue;

            // Determine the localization key
            string key = !string.IsNullOrEmpty(product.TitleLocalizationKey) ? product.TitleLocalizationKey : id;

            if (string.IsNullOrEmpty(key)) continue;

            // Find if a child GameObject with this key name already exists under LeanLocalization
            Transform childTransform = _localization.transform.Find(key);
            LeanPhrase phrase = null;

            if (childTransform != null)
            {
                phrase = childTransform.GetComponent<LeanPhrase>();
                if (phrase == null)
                {
                    phrase = Undo.AddComponent<LeanPhrase>(childTransform.gameObject);
                }
                updatedCount++;
            }
            else
            {
                // Create a new phrase GameObject under the localization GameObject
                GameObject root = new GameObject(key);
                Undo.RegisterCreatedObjectUndo(root, "Create Phrase GameObject");
                
                phrase = root.AddComponent<LeanPhrase>();
                phrase.Data = LeanPhrase.DataType.Text;
                
                root.transform.SetParent(_localization.transform, false);
                addedCount++;
            }

            // Set phrase data to Text
            phrase.Data = LeanPhrase.DataType.Text;

            // Record Undo for the phrase itself
            Undo.RecordObject(phrase, "Update Phrase Translations");

            // Add or update Russian translation
            if (!string.IsNullOrEmpty(product.RuTitle))
            {
                phrase.AddEntry("Russian", product.RuTitle);
            }

            // Add or update English translation
            if (!string.IsNullOrEmpty(product.EnTitle))
            {
                phrase.AddEntry("English", product.EnTitle);
            }

            EditorUtility.SetDirty(phrase);
        }

        EditorUtility.SetDirty(_localization.gameObject);
        
        // If the localization is part of a prefab, apply prefab changes
        if (PrefabUtility.IsPartOfPrefabAsset(_localization.gameObject))
        {
            PrefabUtility.SavePrefabAsset(_localization.gameObject);
            Debug.Log("[Converter] Saved changes directly to LeanLocalization Prefab.");
        }
        else if (PrefabUtility.IsPartOfPrefabInstance(_localization.gameObject))
        {
            PrefabUtility.ApplyPrefabInstance(_localization.gameObject, InteractionMode.UserAction);
            Debug.Log("[Converter] Applied changes to LeanLocalization Prefab Instance.");
        }

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Success", $"Conversion complete!\nAdded: {addedCount} new phrases.\nUpdated: {updatedCount} existing phrases.", "OK");
        Debug.Log($"[Converter] Conversion complete! Added: {addedCount}, Updated: {updatedCount}.");
    }
}
