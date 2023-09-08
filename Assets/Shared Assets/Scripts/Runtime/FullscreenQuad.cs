using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways, DefaultExecutionOrder(600)]
public class FullscreenQuad : MonoBehaviour
{
    private CameraFullscreenQuad _pass;

    public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingTransparents;
    public int injectionPointOffset = 0;
    public Material material;
    public ScriptableRenderPassInput inputRequirements;
    
    private void OnEnable()
    {
        _pass = new CameraFullscreenQuad();
        
        // setup callback
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
    }

    private void OnBeginCamera(ScriptableRenderContext context, Camera cam)
    {
        //if (_pass == null) return;
        
        // injection point
        _pass.renderPassEvent = injectionPoint + injectionPointOffset;
        _pass.passMaterial = material;
        _pass.inputReq = inputRequirements;
        
        // inject pass
        cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(_pass);
    }

    private class CameraFullscreenQuad : ScriptableRenderPass
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
            //if (passMaterial == null) return;
            // do render
            
            Debug.Log("injecting pass");

            var cmd = CommandBufferPool.Get("CameraFullscreenQuad");
            
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


