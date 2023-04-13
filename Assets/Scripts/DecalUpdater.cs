using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
[RequireComponent(typeof(DecalProjector))]
public class DecalUpdater : MonoBehaviour
{
    public float opacity;

    private DecalProjector _decalProjector;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_decalProjector == null) _decalProjector = GetComponent<DecalProjector>();
        _decalProjector.fadeFactor = opacity;
    }
}
