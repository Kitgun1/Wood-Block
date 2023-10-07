using UnityEditor;

public static class EditorUtils
{
#if UNITY_EDITOR
    public static bool IsHierarchyFocused
    {
        get
        {
            return EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.titleContent.text == "Hierarchy";
        }
    }
#endif
}