using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[VolumeComponentMenu("Sample Scene/Oasis Fog")]
public class OasisFogVolumeComponent : VolumeComponent
{
    public FloatParameter Intensity = new(0);
}
