using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class PlayerSaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Reset Player Save"))
        {
            GameManager manager = (GameManager) target;
            manager.ResetPlayerSave();
        }
    }
    
}

#endif