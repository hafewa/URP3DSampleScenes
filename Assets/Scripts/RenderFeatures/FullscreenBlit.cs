using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullscreenBlit : ScriptableRendererFeature
{

    class CustomRenderPass : ScriptableRenderPass
    {
        private Material m_Material;

        public CustomRenderPass(Material material)
        {
            m_Material = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(m_Material != null)
            {
                CommandBuffer cmd = CommandBufferPool.Get("Fullscreen Quad");

                Matrix4x4 cameraProjection = renderingData.cameraData.GetProjectionMatrix();

                cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);

                if(SceneTransitionManager.IsAvailable())
                {
                    m_Material.SetTexture("_RenderTexture", SceneTransitionManager.GetScreenRT());
                }

                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_Material, 0, 0);

                cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            } 
        }
    }

    CustomRenderPass m_ScriptablePass;

    [SerializeField]
    private Material m_Material;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(m_Material);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


