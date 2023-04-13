using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosingMeteor : MonoBehaviour
{
    [Range(0,1)]
    public float T;

    public float Distance;
    [Range(0, 1)] public float RotationScale; 

    private List<Vector3> m_StartPositions;
    private List<Quaternion> m_TargetRotations;
    
    void Start()
    {
        m_StartPositions = new List<Vector3>();
        m_TargetRotations = new List<Quaternion>();
        for (int i = 0; i < transform.childCount; i++)
        {
            m_StartPositions.Add(transform.GetChild(i).localPosition);
            m_TargetRotations.Add(Random.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float t = T;
        t = Mathf.Sin(t * Mathf.PI * 0.5f);
            
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform piece = transform.GetChild(i);
            piece.localPosition =
                m_StartPositions[i] + m_StartPositions[i].normalized * (1 / m_StartPositions[i].magnitude) * t * Distance;
            piece.localRotation = Quaternion.Lerp(Quaternion.identity, m_TargetRotations[i], t * RotationScale);
        }
    }
}