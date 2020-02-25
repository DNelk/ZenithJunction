using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[CustomEditor(typeof(Card))]
public class RandomScript_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields
 
        Card script = (Card)target;
 
        // draw checkbox for the bool
        script.StatModifer = EditorGUILayout.Toggle("Modifies Stats?", script.StatModifer);
        if (script.StatModifer) // if bool is true, show other fields
        {
            var list = script.StatBoosts;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("size", list.Count)); 
            while (newCount < list.Count)
                list.RemoveAt( list.Count - 1 );
            while (newCount > list.Count)
                list.Add(null);
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Stat " + (i+1), EditorStyles.boldLabel);
                if(list[i] == null)
                    list[i] = new Stat(0,0,false, StatType.AttackUP);
                list[i].StatType = (StatType)EditorGUILayout.EnumPopup("Stat Type", list[i].StatType );
                list[i].Value = EditorGUILayout.IntField("Stat Value", list[i].Value);
                list[i].TurnsLeft = EditorGUILayout.IntField("Turns to Apply", list[i].TurnsLeft);
                list[i].IsNew = EditorGUILayout.Toggle("Don't Apply Immidiately", list[i].IsNew);
            }
        }
        else
        {
            if(script.StatBoosts.Count >= 1)
                script.StatBoosts.Clear(); 
        }
    }
}
#endif