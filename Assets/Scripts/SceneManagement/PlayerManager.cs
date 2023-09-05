using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// This class will enable the touch input canvas on handheld devices and will trigger the camera flythrough if the player is idle
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private bool m_FlythroughWhenIdle;
    [SerializeField] private float m_IdleTransitionTime;
    [SerializeField] private GameObject m_CrosshairCanvas;
    [SerializeField] private GameObject m_TouchInputCanvas;
    
    public PlayableDirector FlythroughDirector;
    
    private bool m_InFlythrough;
    private float m_TimeIdle;
    
    void Start()
    {
        m_InFlythrough = false;

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            m_TouchInputCanvas.SetActive(true);
        }
    }

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
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
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

    public void NotifyPlayerMoved()
    {
        m_TimeIdle = 0;
        if (m_InFlythrough)
        {
            EnableFirstPersonController();
        }
    }
}
