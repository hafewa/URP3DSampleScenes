using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MemoryCounter : MonoBehaviour
{
    private Text m_GuiText;
    
    // Start is called before the first frame update
    void Start()
    {
        m_GuiText = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Mono used size" + Profiler.GetMonoUsedSizeLong()/1000000 + "Bytes");
        //m_GuiText.text = "" + megabytes;
    }
}
