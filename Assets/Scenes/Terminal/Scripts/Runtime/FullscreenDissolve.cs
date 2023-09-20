using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullscreenDissolve : MonoBehaviour
{
    private DissolvePass pass;

    public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingTransparents+1;
    public Material material;
    //public ScriptableRenderPassInput inputRequirements;
    
    private void OnEnable()
    {
        pass ??= new DissolvePass();
        
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

        if (!SceneTransitionManager.DissolveNeeded() || cam.CompareTag("ScreenCamera"))
        {
            return;
        }
        
        // injection point
        pass.renderPassEvent = injectionPoint;
        pass.passMaterial = material;
        
        // inject pass
        cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(pass);
    }

    private class DissolvePass : ScriptableRenderPass
    {
        public Material passMaterial;
        private LocalKeyword keyword;
        //public ScriptableRenderPassInput inputReq = ScriptableRenderPassInput.None;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (passMaterial != null)
            {
                //keyword = new LocalKeyword(passMaterial.shader, "_FLIPY");
            }
            //ConfigureInput(inputReq);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (passMaterial == null) return;
            // do render

            var cmd = CommandBufferPool.Get("DissolveEffect");

            //passMaterial.SetKeyword(keyword, renderingData.cameraData.targetTexture == null);
            CoreUtils.DrawFullScreen(cmd, passMaterial);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }
}
