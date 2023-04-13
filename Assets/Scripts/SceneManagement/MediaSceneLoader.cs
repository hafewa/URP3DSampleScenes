using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaSceneLoader : MonoBehaviour
{
    public SceneLoader GardenSceneLoader;
    public SceneLoader CockpitSceneLoader;
    private SceneLoader HubSceneLoader;
    
    private

    // Start is called before the first frame update
    void Start()
    {
        HubSceneLoader = GetComponent<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EnableGarden()
    {
        GardenSceneLoader.EnableScene();
    }

    public void DisableGarden()
    {
        GardenSceneLoader.DisableScene();
    }

    public void EnableCockpit()
    {
        CockpitSceneLoader.EnableScene();
    }

    public void DisableCockpit()
    {
        CockpitSceneLoader.DisableScene();
    }

    public void EnableHub()
    {
        HubSceneLoader.EnableScene();
    }


    public void Transition()
    {
        SceneTransitionManager.StartTransition(this);
    }

    public SceneLoader GetHubSceneLoader()
    {
        return HubSceneLoader;
    }
}
