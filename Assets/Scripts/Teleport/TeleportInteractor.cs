using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Do something else

[ExecuteAlways]
public class TeleportInteractor : MonoBehaviour
{
    public float WarmUpTime;

    public float TeleportProgress;
    public bool LookedAt;
    public bool Active;


    // Start is called before the first frame update
    void Start()
    {
        LookedAt = false;
        Active = false;
        Shader.SetGlobalFloat("_TriplanarTransition", 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(LookedAt && Active)
        {
            TeleportProgress += Time.deltaTime;

            if(TeleportProgress > WarmUpTime)
            {
                TeleportProgress = WarmUpTime;
                SceneTransitionManager.Teleport();
            }
        } else if (Active)
        {
            TeleportProgress -= Time.deltaTime;
            TeleportProgress = Mathf.Max(TeleportProgress, 0);
        }

        if(LookedAt || Active)
        {
            float t = TeleportProgress / WarmUpTime;

            Shader.SetGlobalFloat("_TriplanarTransition", t);
        }
        
    }

    public void Focus()
    {
        LookedAt = true;
    }

    public void DeFocus()
    {
        LookedAt = false;
    }

    public void Activate()
    {
        Active = true;
    }

    public void Deactivate()
    {
        Active = false;
    }
}
