using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(PerformanceTest))]
public class PerformanceTestEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PerformanceTest performnceTest = (PerformanceTest) target;
        
        //Add current
        if (GUILayout.Button("Add current"))
        {
            PerformanceTestStage stage = new PerformanceTestStage();
            stage.SceneName = SceneManager.GetActiveScene().name;
            Transform sceneCamTransform = SceneView.lastActiveSceneView.camera.transform;
            stage.CameraPosition = sceneCamTransform.position;
            stage.CameraRotation = sceneCamTransform.rotation;
            performnceTest.m_Stages.Add(stage);
        }
    }
}
