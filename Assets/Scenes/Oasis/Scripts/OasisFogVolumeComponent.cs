using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[VolumeComponentMenu("Sample Scene/Oasis Fog")]
public class OasisFogVolumeComponent : VolumeComponent
{
    public ClampedFloatParameter Density = new ClampedFloatParameter(0, 0, 0.05f);
    public ColorParameter Tint = new ColorParameter(UnityEngine.Color.white, true, false, false);
}