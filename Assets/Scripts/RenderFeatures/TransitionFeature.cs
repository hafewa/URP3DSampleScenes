using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TransitionFeature : ScriptableRendererFeature
{

    class TransitionPass : ScriptableRenderPass
    {
        private float m_Transition;

        public TransitionPass(float transition)
        {
            m_Transition = transition;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Shader.SetGlobalFloat("_TransitionAmount", m_Transition);
        }
    }

    TransitionPass m_ScriptablePass;

    [Range(0, 1)]
    public float transition;

    

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new TransitionPass(transition);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


