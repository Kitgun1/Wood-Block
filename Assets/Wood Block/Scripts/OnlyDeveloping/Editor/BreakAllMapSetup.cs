#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Code by Gemini 
namespace WoodBlock.Editor
{
    public static class BreakAllMapSetup
    {
        [MenuItem("Tools/Setup BreakAllMap PriceTexts")]
        public static void SetupBreakAllMapPriceTexts()
        {
            string folderPath = "Assets/Wood Block/Scenes/Levels";
            if (!Directory.Exists(folderPath))
            {
                Debug.LogError($"Directory {folderPath} not found!");
                return;
            }

            string[] sceneFiles = Directory.GetFiles(folderPath, "*.unity");
            if (sceneFiles.Length == 0)
            {
                Debug.LogWarning("No scene files found in " + folderPath);
                return;
            }

            int updatedCount = 0;
            int notFoundCount = 0;

            foreach (string scenePath in sceneFiles)
            {
                try
                {
                    Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    
                    GameObject breakAllMapObj = null;
                    var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                    foreach (var obj in allObjects)
                    {
                        if (obj.scene == scene && obj.name == "BreakAllMap")
                        {
                            breakAllMapObj = obj;
                            break;
                        }
                    }

                    if (breakAllMapObj != null)
                    {
                        // 2. Destroy all old PriceText child objects
                        for (int i = breakAllMapObj.transform.childCount - 1; i >= 0; i--)
                        {
                            var child = breakAllMapObj.transform.GetChild(i);
                            if (child.name == "PriceText")
                            {
                                UnityEngine.Object.DestroyImmediate(child.gameObject);
                            }
                        }

                        // 1. Create PriceText (TMPro - Text (UI))
                        GameObject priceTextObj = new GameObject("PriceText");
                        priceTextObj.transform.SetParent(breakAllMapObj.transform);
                        
                        // Apply position, rotation, scale first
                        priceTextObj.transform.position = new Vector3(6.042481422424316f, 8.659934997558594f, 0.0f);
                        priceTextObj.transform.rotation = Quaternion.identity;
                        priceTextObj.transform.localScale = Vector3.one;

                        // Add RectTransform and apply placement relative to parent anchors
                        var rectTrans = priceTextObj.AddComponent<RectTransform>();
                        rectTrans.anchorMin = new Vector2(0f, 0f);
                        rectTrans.anchorMax = new Vector2(1f, 1f);
                        rectTrans.offsetMin = new Vector2(25f, 9f);      // Left: 25, Bottom: 9
                        rectTrans.offsetMax = new Vector2(-25f, -56f);   // Right: 25, Top: 56
                        
                        // 3. TextMeshProUGUI with Auto Size min 18, max 73 and font ofont
                        var tmp = priceTextObj.AddComponent<TMPro.TextMeshProUGUI>();
                        tmp.enableAutoSizing = true;
                        tmp.fontSizeMin = 18f;
                        tmp.fontSizeMax = 73f;
                        tmp.alignment = TMPro.TextAlignmentOptions.Center;
                        var fontAsset = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/Plugins/TextMesh Pro/Fonts/ofont.asset");
                        if (fontAsset != null)
                        {
                            tmp.font = fontAsset;
                        }
                        else
                        {
                            Debug.LogWarning("ofont font asset not found at Assets/Plugins/TextMesh Pro/Fonts/ofont.asset");
                        }
                        tmp.text = "";

                        var mapBlowUp = breakAllMapObj.GetComponent<MapBlowUp>();
                        if (mapBlowUp != null)
                        {
                            var field = typeof(MapBlowUp).GetField("_priceText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (field != null)
                            {
                                field.SetValue(mapBlowUp, tmp);
                                EditorUtility.SetDirty(mapBlowUp);
                            }
                        }

                        // 4. Find child "Text (TMP)" and change placement
                        Transform textTmpChild = null;
                        for (int i = 0; i < breakAllMapObj.transform.childCount; i++)
                        {
                            var child = breakAllMapObj.transform.GetChild(i);
                            if (child.name.Contains("Text (TMP)"))
                            {
                                textTmpChild = child;
                                break;
                            }
                        }

                        if (textTmpChild != null)
                        {
                            var childRect = textTmpChild.GetComponent<RectTransform>();
                            if (childRect == null)
                            {
                                childRect = textTmpChild.gameObject.AddComponent<RectTransform>();
                            }
                            childRect.anchorMin = new Vector2(0f, 0f);
                            childRect.anchorMax = new Vector2(1f, 1f);
                            childRect.offsetMin = new Vector2(25f, 30f);     // Left: 25, Bottom: 30
                            childRect.offsetMax = new Vector2(-25f, -30f);    // Right: 25, Top: 30
                            EditorUtility.SetDirty(textTmpChild.gameObject);
                        }

                        EditorSceneManager.MarkSceneDirty(scene);
                        EditorSceneManager.SaveScene(scene);
                        updatedCount++;
                    }
                    else
                    {
                        notFoundCount++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[BreakAllMapSetup] Error processing BreakAllMap on '{scenePath}': {ex.Message}");
                }
            }

            EditorUtility.DisplayDialog("Setup BreakAllMap PriceTexts", 
                $"Finished setup on {sceneFiles.Length} scenes.\n\n" +
                $"Setup PriceText: {updatedCount}\n" +
                $"BreakAllMap not found: {notFoundCount}", 
                "OK");
        }
        
          [MenuItem("Tools/Disable BreakAllMap PriceText Wrapping")]
        public static void DisableBreakAllMapPriceTextWrapping()
        {
            string folderPath = "Assets/Wood Block/Scenes/Levels";
            if (!Directory.Exists(folderPath))
            {
                Debug.LogError($"Directory {folderPath} not found!");
                return;
            }
            string[] sceneFiles = Directory.GetFiles(folderPath, "*.unity");
            if (sceneFiles.Length == 0)
            {
                Debug.LogWarning("No scene files found in " + folderPath);
                return;
            }
            int updatedCount = 0;
            int notFoundCount = 0;
            foreach (string scenePath in sceneFiles)
            {
                try
                {
                    Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    
                    GameObject breakAllMapObj = null;
                    var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                    foreach (var obj in allObjects)
                    {
                        if (obj.scene == scene && obj.name == "BreakAllMap")
                        {
                            breakAllMapObj = obj;
                            break;
                        }
                    }
                    if (breakAllMapObj != null)
                    {
                        Transform priceTextChild = breakAllMapObj.transform.Find("PriceText");
                        if (priceTextChild != null)
                        {
                            var tmp = priceTextChild.GetComponent<TMPro.TMP_Text>();
                            if (tmp != null)
                            {
                                tmp.fontSizeMin = 12;
                                tmp.enableWordWrapping = false;
                                EditorUtility.SetDirty(tmp);
                                EditorSceneManager.MarkSceneDirty(scene);
                                EditorSceneManager.SaveScene(scene);
                                updatedCount++;
                                continue;
                            }
                        }
                    }
                    notFoundCount++;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[BreakAllMapSetup] Error processing PriceText on '{scenePath}': {ex.Message}");
                }
            }
            EditorUtility.DisplayDialog("Disable PriceText Wrapping", 
                $"Finished processing {sceneFiles.Length} scenes.\n\n" +
                $"Updated PriceText wrapping: {updatedCount}\n" +
                $"Not found/skipped: {notFoundCount}", 
                "OK");
        }
    }
    
}
#endif
