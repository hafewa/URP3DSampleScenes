using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class ScreenCamera : MonoBehaviour
{
    [SerializeField] private float m_RenderTextureScale = 1;
    private Camera cam;

    public Material screenMat;

    //On awake because it needs to happen before the screen materials are set
    void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateTarget();

        //screenMat.SetTexture("_ScreenColor", cam.targetTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateTarget()
    {
        cam.targetTexture = new RenderTexture((int)(cam.scaledPixelWidth * m_RenderTextureScale), (int)(cam.scaledPixelHeight * m_RenderTextureScale), GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormat.D24_UNorm_S8_UInt);
    }
}