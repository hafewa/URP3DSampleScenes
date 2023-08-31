using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[VolumeComponentMenu("Sample Scene/Oasis Fog")]
public class OasisFogVolumeComponent : VolumeComponent
{
    public ClampedFloatParameter Density = new ClampedFloatParameter(0, 0, 0.05f);
    public Vector2Parameter HeightRange = new Vector2Parameter(new Vector2(0.0f, 50.0f));
    public ColorParameter Tint = new ColorParameter(UnityEngine.Color.white, true, false, false);
    public FloatParameter SunScatteringIntensity = new FloatParameter (2.0f);
}