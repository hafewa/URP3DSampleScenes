using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component ensures that there can be only one CameraSingleton among the scenes
/// </summary>
public class CameraSingleton : MonoBehaviour
{
    private static CameraSingleton instance = null;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);

        foreach (var childTransform in GetComponentsInChildren<Transform>())
        {
            if (childTransform.parent == transform) //How do I avoid this and just loop over immediate children
            {
                Debug.Log("Keeping " + childTransform.name + " alive");
                childTransform.parent = null;
                DontDestroyOnLoad(childTransform);
            }

        }
        
        instance = this;
    }
}
