using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[Serializable]
[CustomEditor(typeof(GameManager))]
public class PlayerSaveEditor : Editor
{
    private SerializedProperty _indices;
    private string[] _options;
    private void OnEnable()
    {
        if(CardDirectory.CardsByName.Count == 0)
            CardDirectory.LoadDirectory();
        _options = CardDirectory.CardsByName.Keys.ToArray();
        _indices = serializedObject.FindProperty("StartingIndices");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager manager = (GameManager) target;

        for(int i = 0; i < 9; i++)
        {
            _indices.GetArrayElementAtIndex(i).intValue = EditorGUILayout.Popup("Starting Card " + (i + 1), _indices.GetArrayElementAtIndex(i).intValue, _options);
            manager.StartingCards[i] = _options[_indices.GetArrayElementAtIndex(i).intValue];
        }
        
        
        if (GUILayout.Button("Reset Player Save"))
        {
            manager.ResetPlayerSave();
        }
        
        if (GUILayout.Button("Generate Card Directory"))
        {
            CardDirectory.SaveDirectory();
            _options = CardDirectory.CardsByName.Keys.ToArray();
        }

        serializedObject.ApplyModifiedProperties();
    }
    
}

#endif