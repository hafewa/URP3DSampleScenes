using System;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public Animator screenAnimator;
    public LoadingBar controlPanel;
    private Action m_Callback;


    void Start()
    {
    }

    public void TurnScreenOn()
    {
        if(SceneTransitionManager.IsAvailable() && GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().material.SetTexture("_ScreenColor", SceneTransitionManager.GetScreenRT());
        }
        
        if (screenAnimator != null)
        {
            screenAnimator.SetBool("ScreenOn", true);
            screenAnimator.SetTrigger("EnableScreen");
        }

        if(controlPanel != null)
        {
            controlPanel.TurnOn();
        }

        if(GetComponent<MeshRenderer>() != null)
        {
            Shader.SetGlobalColor("_TransitionColor", GetComponent<MeshRenderer>().material.GetColor("_TransitionEdgeColor"));
        }
    }

    public void TurnScreenOff(Action callback)
    {
        m_Callback = callback;
        if (screenAnimator != null)
        {
            screenAnimator.SetBool("ScreenOn", false);
            screenAnimator.SetTrigger("DisableScreen");
        }

        if(controlPanel != null)
        {
            controlPanel.TurnOff();
        }
    }

    public void Callback()
    {
        if(m_Callback != null)
        {
            m_Callback();
        }
    }
}
