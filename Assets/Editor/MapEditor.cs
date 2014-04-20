using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(Map))]
public class MapEditor : Editor {
    SerializedProperty Tiles;
    GameObject selectedItem;
    void OnEnable()
    {
        Tiles = serializedObject.FindProperty("TileTypes");
    }
    void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Map m = (Map)target;
        foreach (var item in Tiles)
        {
            var tempItem = item as GameObject;
            if (GUILayout.Button(tempItem.name))
            {
                selectedItem = tempItem;
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUILayout.Space();
    }
}
