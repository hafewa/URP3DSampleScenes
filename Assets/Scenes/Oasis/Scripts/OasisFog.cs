using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//TODO: Manage the material from script instead of inspector

[ExecuteAlways]
public class OasisFog : MonoBehaviour
{
    private OasisFogPass _pass;

    public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingTransparents;
    public int injectionPointOffset = 0;
    public Material material;
    public ScriptableRenderPassInput inputRequirements;
    
    private void OnEnable()
    {
        _pass = new OasisFogPass();
        
        // setup callback
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
    }

    private void OnBeginCamera(ScriptableRenderContext context, Camera cam)
    {
        // injection point
        _pass.renderPassEvent = injectionPoint + injectionPointOffset;
        _pass.passMaterial = material;
        _pass.inputReq = inputRequirements;


        
        cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(_pass);
    }
    
    private class OasisFogPass : ScriptableRenderPass
    {
        public Material passMaterial;
        private LocalKeyword keyword;
        public ScriptableRenderPassInput inputReq = ScriptableRenderPassInput.None;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (passMaterial != null)
            {
                keyword = new LocalKeyword(passMaterial.shader, "_FLIPY");
            }
            ConfigureInput(inputReq);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            OasisFogVolumeComponent volumeComponent = VolumeManager.instance.stack.GetComponent<OasisFogVolumeComponent>();
            
            float fogDensity = volumeComponent.Density.value;
            if (passMaterial == null || fogDensity < Mathf.Epsilon) return;

            var cmd = CommandBufferPool.Get("CameraFullscreenQuad");

            float fogStartDistance = volumeComponent.StartDistance.value;
            Color fogTint = volumeComponent.Tint.value;
            float fogSunScatteringIntensity = volumeComponent.SunScatteringIntensity.value;
            Vector2 fogHeightRange = volumeComponent.HeightRange.value;

            passMaterial.SetColor("_Tint", fogTint);
            passMaterial.SetFloat("_Density", fogDensity);
            passMaterial.SetFloat("_StartDistance", fogStartDistance);
            passMaterial.SetFloat("_SunScatteringIntensity", fogSunScatteringIntensity);
            passMaterial.SetVector("_Height_Range", fogHeightRange);

            var flipY = renderingData.cameraData.IsRenderTargetProjectionMatrixFlipped(renderingData.cameraData.renderer.cameraColorTargetHandle);
            passMaterial.SetKeyword(keyword, flipY);
            var cam = renderingData.cameraData.camera;
            passMaterial.SetMatrix("_InverseViewProjection", (GL.GetGPUProjectionMatrix(cam.projectionMatrix, false) * cam.worldToCameraMatrix).inverse);
            CoreUtils.DrawFullScreen(cmd, passMaterial);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }
}
