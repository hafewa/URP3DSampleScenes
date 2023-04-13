using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class OverlayPosition : MonoBehaviour
{
    public Camera[] baseCam;
    private Vector3 offset;
    
    void LateUpdate()
    {
        
    }
    
    

    public void ToggleOffset()
    {
        offset = -offset;
        
    }

    public void SetOffst(Vector3 offset)
    {
        this.offset = offset;
    }

    public Vector3 GetOffset()
    {
        return offset;
    }

    public void UpdateWithOffset()
    {
        Camera activeCamera = GetActiveCamera();

        transform.rotation = activeCamera.transform.rotation;
        transform.position = activeCamera.transform.position + offset;
        GetComponent<Camera>().fieldOfView = activeCamera.fieldOfView;
    }

    private Camera GetActiveCamera()
    {
        //TODO: THIS SHOULDNT BE A LIST! Base it on tags
        for (int i = 0; i < baseCam.Length; i++)
        {
            if (baseCam[i].gameObject.activeInHierarchy) return baseCam[i];
        }

        throw new Exception("No base cam is currently active");
    }
}
