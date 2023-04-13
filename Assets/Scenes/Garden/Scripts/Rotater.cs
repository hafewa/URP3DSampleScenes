using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Figure out if this is actually used
public class Rotater : MonoBehaviour
{
    public float RevolutionTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, (360 / RevolutionTime) * Time.deltaTime);
    }
}
