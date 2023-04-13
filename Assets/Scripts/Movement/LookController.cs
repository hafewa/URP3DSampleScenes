using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : MonoBehaviour
{
    // Start is called before the first frame update

    private TeleportInteractor interactor;
    private LoadingBar loadingBar;

    void Start()
    {
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;

        int layerMask = 1 << LayerMask.NameToLayer("LookInteractor");

        if(Physics.Raycast(transform.position, transform.forward, out hit, 3, layerMask))
        {
            LoadingBar newLoadingBar = hit.collider.GetComponent<LoadingBar>();

            if(loadingBar == null && newLoadingBar != null)
            {
                loadingBar = newLoadingBar;
                loadingBar.StartLoading();
            }

        } else if(loadingBar != null)
        {
            loadingBar.StopLoading();
            loadingBar = null;
        }
    }
}
