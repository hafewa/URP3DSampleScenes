

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(FoliageGroup))]
public class FoliageGroupEditor : Editor
{
    private ReorderableList list;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_FoliageElements"), true, true, true, true);
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FoliageGroup foliageGroup = (FoliageGroup) target;
        
        if (GUILayout.Button("Shuffle"))
        {
            
            foliageGroup.Shuffle();
        }
    }
    
    [DrawGizmo(GizmoType.Selected)]
    static void DrawGizmo(FoliageGroup foliageGroup, GizmoType gizmoType)
    {
        Gizmos.color = Color.green;

        int res = 32;
        Vector3[] pointArray = new Vector3[res];

        Handles.DrawWireDisc(foliageGroup.transform.position, Vector3.up, foliageGroup.m_Radius);
        

        foreach (Transform child in foliageGroup.transform)
        {
            Gizmos.DrawSphere(child.transform.position, 0.1f);
        }
    }
}