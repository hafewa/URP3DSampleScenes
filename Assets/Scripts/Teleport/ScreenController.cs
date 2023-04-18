using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ScreenController : MonoBehaviour
{
    public Animator screenAnimator;
    public LoadingBar controlPanel;
    private Action m_Callback;
    private MeshRenderer m_MeshRenderer;

    private void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    public void TurnScreenOn()
    {
        if(SceneTransitionManager.IsAvailable())
        {
            m_MeshRenderer.material.SetTexture("_ScreenColor", SceneTransitionManager.GetScreenRT());
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

        Shader.SetGlobalColor("_TransitionColor", m_MeshRenderer.material.GetColor("_TransitionEdgeColor"));
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

    //This is called by the animation clip when the screens are off
    public void Callback()
    {
        if(m_Callback != null)
        {
            m_Callback();
        }
    }
}
