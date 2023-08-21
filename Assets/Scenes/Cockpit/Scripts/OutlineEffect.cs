using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class OutlineEffect : MonoBehaviour
{
    private OutlinePass pass;

    public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingTransparents;
    public Material material;
    public ScriptableRenderPassInput inputRequirements;
    
    private void OnEnable()
    {
        pass ??= new OutlinePass();
        
        // setup callback
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
    }

    private void OnBeginCamera(ScriptableRenderContext context, Camera cam)
    {
        if (pass == null) return;

        if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView) return;
        
        // injection point
        pass.renderPassEvent = injectionPoint;
        pass.passMaterial = material;
        pass.inputReq = inputRequirements;
        
        // inject pass
        cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(pass);
    }

    private class OutlinePass : ScriptableRenderPass
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
            OutlineVolumeComponent volumeComponent = VolumeManager.instance.stack.GetComponent<OutlineVolumeComponent>();
            
            
            if (passMaterial == null || !volumeComponent.Enabled.value) return;
            // do render

            var cmd = CommandBufferPool.Get("OutlineEffect");
            
            var flipY = renderingData.cameraData.IsRenderTargetProjectionMatrixFlipped(renderingData.cameraData.renderer.cameraColorTargetHandle);
            passMaterial.SetKeyword(keyword, flipY);
            CoreUtils.DrawFullScreen(cmd, passMaterial);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }
}
