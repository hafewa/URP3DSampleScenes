using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//TODO: Remove this script beforesample release
/*
[CustomEditor(typeof(PrefabReplacer))]
public class PrefabReplaceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrefabReplacer replacer = (PrefabReplacer) target;
        if (GUILayout.Button("Replace"))
        {
            replacer.Replace();
        }
    }
}
*/

[ExecuteAlways]
public class PrefabReplacer : MonoBehaviour
{
    public List<GameObject> NewPrefabs;


/*
    public void Replace()
    {
        //I will be adding at the end of the list so I cache the length
        int childCount = transform.childCount;

        List<GameObject> toDelete = new List<GameObject>();

        for (int i = 0; i < childCount; i++)
        {
            Transform current = transform.GetChild(i);

            string prefabName = current.name.Split('(')[0].Trim();
            
            Debug.Log("Replacing: " + current.name + ", Prefab: " + prefabName);

            GameObject newPrefab = FindPrefabFromName(prefabName);

            if (newPrefab != null)
            {
                Transform newInstance = ((GameObject)PrefabUtility.InstantiatePrefab(newPrefab)).transform;

                newInstance.position = current.transform.position;
                newInstance.rotation = current.transform.rotation;
                newInstance.parent = transform;

                toDelete.Add(current.gameObject);
            }
            else
            {
                Debug.LogWarning("Could not find replacement for " + prefabName);
            }
        }

        foreach (var gameObject in toDelete)
        {
            DestroyImmediate(gameObject);
        }

    }

    public GameObject FindPrefabFromName(string name)
    {
        foreach (var prefab in NewPrefabs)
        {
            if (prefab.name == name) return prefab;
        }
        
        return null;
    }
    */
}
