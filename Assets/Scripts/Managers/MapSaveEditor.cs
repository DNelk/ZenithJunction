using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(MapManager))]
public class MapSaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Save Map"))
        {
            MapManager manager = (MapManager) target;
            manager.SaveMap();
        }
    }
    
}

#endif