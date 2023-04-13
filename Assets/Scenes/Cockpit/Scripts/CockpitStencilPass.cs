using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class CockpitStencilPass : MonoBehaviour
{
    private CockpitStencil stencilPass;
    
    public LayerMask cockpitMaskLayer = 4;
    public Shader simpleOverrideShader;
    public int stencilValue = 1;
    public bool debugStencil;

    private void OnEnable()
    {
        stencilPass ??= new CockpitStencil();
        if(simpleOverrideShader)
            stencilPass.shader = simpleOverrideShader;
        stencilPass.mask = cockpitMaskLayer;
        stencilPass.stencilValue = stencilValue;
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
    }

    private void OnBeginCamera(ScriptableRenderContext context, Camera cam)
    {
        if (stencilPass == null) return;
        stencilPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        stencilPass.debug = debugStencil;
        // inject pass
        cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(stencilPass);
    }

    class CockpitStencil : ScriptableRenderPass
    {
        private DrawingSettings stencilDrawingSettings;
        private RenderStateBlock stencilRenderStateBlock;
        
        private DrawingSettings drawingSettings;
        private RenderStateBlock renderStateBlock;
        
        private FilteringSettings filteringSettings;
        
        public Shader shader;
        public LayerMask mask;
        public int stencilValue;
        public bool debug;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, mask);

            stencilRenderStateBlock = StencilRenderStateBlock();

            renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            drawingSettings = CreateDrawingSettings(new ShaderTagId("SRPDefaultUnlit"), ref renderingData,
                SortingCriteria.CommonOpaque);
            
            stencilDrawingSettings = CreateDrawingSettings(new ShaderTagId("SRPDefaultUnlit"), ref renderingData,
                SortingCriteria.CommonOpaque);
            if (debug)
            {
                stencilDrawingSettings.overrideShader = shader;
                stencilDrawingSettings.overrideShaderPassIndex = 0;
            }

            // Stencil Cockpit
            context.DrawRenderers(renderingData.cullResults, ref stencilDrawingSettings, ref filteringSettings, ref stencilRenderStateBlock);
            
            // Draw Cockpit normally
            //if(!debug)
                //context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);

            //context.ExecuteCommandBuffer(cmd);
            //cmd.Release();
        }

        private RenderStateBlock StencilRenderStateBlock()
        {
            var _RSB = new RenderStateBlock();

            if (debug)
            {
                var depthstate = new DepthState(false, CompareFunction.Always);
                _RSB.depthState = depthstate;
                _RSB.mask |= RenderStateMask.Depth;
            }

            var stencilState = StencilState.defaultValue;
            stencilState.enabled = true;
            stencilState.SetCompareFunction(CompareFunction.Always);
            stencilState.SetPassOperation(StencilOp.Replace);
            stencilState.SetFailOperation(StencilOp.Keep);
            stencilState.SetZFailOperation(0);

            _RSB.mask |= RenderStateMask.Stencil;
            _RSB.stencilReference = stencilValue;
            _RSB.stencilState = stencilState;

            return _RSB;
        }
    }
}


