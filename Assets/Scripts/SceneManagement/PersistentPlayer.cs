using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PersistentPlayer : MonoBehaviour
{
    [SerializeField] private bool m_FlythroughWhenIdle;
    [SerializeField] private float m_IdleTransitionTime;
    public PlayableDirector FlythroughDirector;
    [SerializeField] private GameObject m_CrosshairCanvas;
    [SerializeField] private GameObject m_TouchInputCanvas;
    
    private bool m_InFlythrough;
    private float m_TimeIdle;
    
    // Start is called before the first frame update
    void Start()
    {
        m_InFlythrough = false;

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            m_TouchInputCanvas.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (m_FlythroughWhenIdle && m_TimeIdle > m_IdleTransitionTime && !m_InFlythrough)
        {
            m_TimeIdle = 0;
            EnableFlythrough();
        }

        m_TimeIdle += Time.unscaledDeltaTime;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void EnableFlythrough()
    {
        FlythroughDirector.gameObject.SetActive(true);
        
        TimelineAsset timeline = FlythroughDirector.playableAsset as TimelineAsset;
        FlythroughDirector.SetGenericBinding(timeline.GetOutputTrack(0), GetComponentInChildren<CinemachineBrain>());
        
        FlythroughDirector.time = 0;
        FlythroughDirector.Play();
        m_InFlythrough = true;
        m_CrosshairCanvas.SetActive(false);
    }

    public void EnableFirstPersonController()
    {
        m_CrosshairCanvas.SetActive(true);
        FlythroughDirector.gameObject.SetActive(false);
        m_InFlythrough = false;
    }

    public void ToggleController()
    {
        if (m_InFlythrough)
        {
            EnableFirstPersonController();
        }
        else
        {
            EnableFlythrough();
        }
        
        
    }

    public void NotifyPlayerMoved()
    {
        m_TimeIdle = 0;
        if (m_InFlythrough)
        {
            EnableFirstPersonController();
        }
    }
}
