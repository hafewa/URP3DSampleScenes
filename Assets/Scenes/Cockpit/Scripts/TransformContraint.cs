using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class TransformContraint : MonoBehaviour
{
    public Transform pivot;
    [Header("Constraints")]
    public Transform constraintA;
    public Transform constraintB;

    [Range(0f, 1f)]
    public float blendContraints = 0f;
    [Header("Blend Controls")]
    [Range(0f, 1f)]
    public float blendTotal = 0f;
    [Range(0f, 1f)]
    public float blendPosition = 0f;
    [Range(0f, 1f)]
    public float blendRotation = 0f;
    [Range(0f, 1f)]
    public float blendScale = 0f;
    
    private Vector3 positionWS;
    private Quaternion rotation;
    private Vector3 scale = Vector3.one;

    public float dampening = 0.1f;
    private Vector3 posVel;
    private Quaternion rotVel;
    
    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += BeginFrame;
        RenderPipelineManager.endCameraRendering += EndFrame;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= BeginFrame;
        RenderPipelineManager.endCameraRendering -= EndFrame;
    }

    private void LateUpdate()
    {
        if(Application.isPlaying)
            UpdateConstraint();
    }

    private void BeginFrame(ScriptableRenderContext arg1, Camera arg2)
    {
        if (!Application.isPlaying)
        {
            var transform1 = transform;
            positionWS = transform1.position;
            rotation = transform1.rotation;
            scale = transform1.localScale;

            UpdateConstraint();
        }
    }
    
    private void EndFrame(ScriptableRenderContext arg1, Camera arg2)
    {
        if (!Application.isPlaying)
        {
            pivot.SetPositionAndRotation(positionWS, rotation);
            pivot.localScale = scale;
        }
    }

    private void UpdateConstraint()
    {
        if (!constraintA || !constraintB) return;

        var targetPosition = Vector3.Lerp(constraintA.position, constraintB.position, blendContraints);
        var targetRotation = Quaternion.Lerp(constraintA.rotation, constraintB.rotation, blendContraints);
        var targetScale = Vector3.Lerp(constraintA.lossyScale, constraintB.lossyScale, blendContraints);

        var pos = Vector3.Lerp(transform.position, targetPosition, blendTotal * blendPosition);
        //pos = Vector3.SmoothDamp(pivot.position, pos, ref posVel, dampening);
        var rot = Quaternion.Lerp(transform.rotation, targetRotation, blendTotal * blendRotation);
        
        pivot.SetPositionAndRotation(
            pos,
            rot
        );
        pivot.localScale = Vector3.Lerp(transform.localScale, targetScale, blendTotal * blendScale);
    }
}
