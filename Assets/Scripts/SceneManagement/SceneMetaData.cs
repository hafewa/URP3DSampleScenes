using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// This script has metadata needed for multi scene rendering and teleporting
/// It also registers the scene in the scene transition manager
/// </summary>
public class SceneMetaData : MonoBehaviour
{
    //TODO: Rewrite to use properties with correct getters and setters? Not sure if this is possible while exposing them in the editor. Maybe we need a custom editor.
    public GameObject mainLight = null;
    public Material skybox = null;
    public Cubemap reflection = null;
    public Transform CameraLockTransform = null;
    public Transform SpawnTransform;
    public PlayableDirector Director;
    public float DirectorStartTime = 0;
    public GameObject Root;
    public Scene Scene;
    public GameObject HubLoader;
    public bool FogEnabled;
    public bool StartActive;
    
    void Start()
    {
        if(SceneTransitionManager.IsAvailable())
        {
            SetUp();
        }
    }

    private void SetUp()
    {
        Scene = gameObject.scene;

        //Disable objects that shouldn't be used in a multi scene setup
        foreach (var go in Scene.GetRootGameObjects())
        {
            if (go != gameObject && !(go == Root && StartActive))
            {
                go.SetActive(false);
            }
        }
        
        //Register scene
        SceneTransitionManager.RegisterScene(Scene.name, this);
    }
}
