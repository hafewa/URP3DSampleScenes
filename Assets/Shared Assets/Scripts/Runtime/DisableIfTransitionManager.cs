using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableIfTransitionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneTransitionManager.IsAvailable())
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
