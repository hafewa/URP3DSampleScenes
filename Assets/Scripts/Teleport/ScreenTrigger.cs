using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Will trigger screen animations when entering trigger
/// </summary>
public class ScreenTrigger : MonoBehaviour
{
    public Animator screenAnimator;
    public bool startOn;
    public TeleportInteractor Interactor;
    // Start is called before the first frame update
    void Start()
    {
        screenAnimator.SetBool("ScreenOn", SceneTransitionManager.HasNotLoadedScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Turn on screen!");
        if(screenAnimator != null)
        {
            screenAnimator.SetBool("ScreenOn", true);
        }
        Interactor.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Turn off screen!");
        if(screenAnimator != null)
        {
            screenAnimator.SetBool("ScreenOn", false);
        }
        Interactor.enabled = false;
    }
}
