using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private float m_TimeInterval;
    [SerializeField] private int m_TargetFrameRate;
    
    private TMP_Text m_Label;
    private List<float> m_Samples;

    private float m_ElapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        m_Label = GetComponent<TMP_Text>();
        m_Samples = new List<float>();
        m_Label.text = "- FPS";
        if (m_TargetFrameRate > 0)
        {
            Application.targetFrameRate = m_TargetFrameRate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ElapsedTime < m_TimeInterval)
        {
            m_Samples.Add(1.0f / Time.unscaledDeltaTime);
            m_ElapsedTime += Time.unscaledDeltaTime;
        }
        else
        {
            float frameSum = 0;
            for (int i = 0; i < m_Samples.Count; i++)
            {
                frameSum += m_Samples[i];
            }
            
            float averageFPS = frameSum / m_Samples.Count;
            m_Label.text = Mathf.Round(averageFPS) + " FPS";
            
            m_ElapsedTime = 0;
            m_Samples = new List<float>();
        }
    }
}
