#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace WoodBlock.Editor
{
    public static class DisableRaycastTargetEditor
    {
        [MenuItem("Tools/Disable Raycast on Bomb Button For Add")]
        public static void DisableRaycastOnBombButtonForAdd()
        {
            string scenesFolder = "Assets/Wood Block/Scenes";
            if (!Directory.Exists(scenesFolder))
            {
                Debug.LogError($"Directory {scenesFolder} not found!");
                return;
            }

            string[] sceneFiles = Directory.GetFiles(scenesFolder, "*.unity", SearchOption.AllDirectories);
            if (sceneFiles.Length == 0)
            {
                Debug.LogWarning("No scene files found in " + scenesFolder);
                return;
            }

            int updatedCount = 0;
            int totalChecked = 0;

            foreach (string scenePath in sceneFiles)
            {
                totalChecked++;
                try
                {
                    Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    bool sceneModified = false;
                    
                    GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                    foreach (var obj in allObjects)
                    {
                        if (obj.scene == scene && MatchesPath(obj.transform))
                        {
                            TextMeshProUGUI[] tmps = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
                            foreach (var tmp in tmps)
                            {
                                if (tmp.raycastTarget)
                                {
                                    tmp.raycastTarget = false;
                                    EditorUtility.SetDirty(tmp);
                                    sceneModified = true;
                                    Debug.Log($"[{scene.name}] Disabled RaycastTarget on TMPro: {tmp.name}");
                                }
                            }
                        }
                    }

                    if (sceneModified)
                    {
                        EditorSceneManager.MarkSceneDirty(scene);
                        EditorSceneManager.SaveScene(scene);
                        updatedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error processing scene '{scenePath}': {ex.Message}\n{ex.StackTrace}");
                }
            }

            EditorUtility.DisplayDialog("Disable Raycast Target", 
                $"Finished processing {totalChecked} scenes.\n\n" +
                $"Scenes updated: {updatedCount}", 
                "OK");
        }

        private static bool MatchesPath(Transform trans)
        {
            if (trans == null || trans.name != "For Add") return false;
            
            Transform p1 = trans.parent;
            if (p1 == null || p1.name != "BombButton") return false;
            
            return true;
        }
    }
}
#endif
