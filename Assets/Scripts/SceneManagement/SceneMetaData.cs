using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// This script sets the scene up correctly if it is loaded from the hub
/// </summary>
public class SceneMetaData : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        if(SceneTransitionManager.IsAvailable())
        {
            SetUp();
        }
    }

    private void SetUp()
    {
        Scene scene = gameObject.scene;

        foreach (var go in scene.GetRootGameObjects())
        {
            if (go != gameObject && !(go == Root && StartActive))
            {
                go.SetActive(false);
            }
        }
        
        SceneTransitionManager.RegisterScene(scene.name, this);
        Scene = scene;
    }
}
