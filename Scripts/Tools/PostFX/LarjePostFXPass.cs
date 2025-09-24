using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_2023_1_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
#endif
using UnityEngine.Rendering.Universal;

public class LarjePostFXPass : ScriptableRenderPass
{
    private const string PASS_NAME = "LarjePostFXPass";
    
    private int _dstId = Shader.PropertyToID("_LarjePostFXDst");
    private RenderTargetIdentifier _src;
    private RenderTargetIdentifier _dst;
    
    private LarjePostFX.Processor[] _effects;

    public LarjePostFXPass(LarjePostFX.Processor[] effects, RenderPassEvent injectionPoint)
    {
        _effects = effects;
        renderPassEvent = injectionPoint;
#if UNITY_2023_1_OR_NEWER
        requiresIntermediateTexture = true;
#endif
    }

#if UNITY_2023_1_OR_NEWER
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        foreach (LarjePostFX.Processor effect in _effects)
        {
            effect.Update();
            
            if (effect.Material == null || !effect.Enabled)
            {
                continue;
            }
            
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer)
            {
                continue;
            }

            TextureHandle src = resourceData.activeColorTexture;
            TextureDesc dstDesc = renderGraph.GetTextureDesc(src);
            dstDesc.name = $"CameraColor-{PASS_NAME}";
            dstDesc.clearBuffer = false;
        
            TextureHandle dst = renderGraph.CreateTexture(dstDesc);
            RenderGraphUtils.BlitMaterialParameters blitParams = new(src, dst, effect.Material, 0);
            renderGraph.AddBlitPass(blitParams, passName: PASS_NAME);

            resourceData.cameraColor = dst;   
        }
    }
#endif

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        
    }
}