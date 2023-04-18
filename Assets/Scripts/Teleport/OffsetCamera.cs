using UnityEngine;

public class OffsetCamera : MonoBehaviour
{
    [Tooltip("The camera which this camera will follow with an offset")]
    [SerializeField] private Camera m_MainCamera;
    
    private Vector3 m_Offset;
    private Camera m_Camera;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    public void ToggleOffset()
    {
        m_Offset = -m_Offset;
    }

    public void SetOffset(Vector3 offset)
    {
        m_Offset = offset;
    }

    public Vector3 GetOffset()
    {
        return m_Offset;
    }

    public void UpdateWithOffset()
    {
        Transform thisTransform = transform;
        Transform mainCamTransform = m_MainCamera.transform;
        
        thisTransform.rotation = mainCamTransform.rotation;
        thisTransform.position = mainCamTransform.position + m_Offset;
        m_Camera.fieldOfView = m_MainCamera.fieldOfView;
    }
}
