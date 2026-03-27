using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LarjePostFXFeature : ScriptableRendererFeature
{
    public static LarjePostFXFeature Instance { get; private set; }
    
    public static bool TryGetFX<T>(out T fx) where T : LarjePostFX.Processor
    {
        fx = null;
        if (Instance == null)
        {
            Debug.LogWarning("LarjePostFXFeature instance is null");
            return false;
        }

        if (Instance._processors == null)
        {
            return false;
        }
        
        foreach (LarjePostFX.Processor processor in Instance._processors)
        {
            if (processor is T t)
            {
                fx = t;
                return true;
            }
        }

        return false;
    }

    public Settings settings = new Settings();
    
    private LarjePostFXPass _pass;
    private List<LarjePostFX.Processor> _processors = new List<LarjePostFX.Processor>();

    public override void Create()
    {
        if (Instance == this)
        {
            return;
        }
        
        if (Instance != null)
        {
            Instance.Destroy();
        }
        
        Instance = this;
        
        foreach (LarjePostFX effect in settings.effects)
        {
            if (effect != null)
            {
                LarjePostFX.Processor processor = effect.GetProcessor();
                if (processor != null)
                {
                    _processors.Add(processor);   
                }
            }
        }
        _pass = new LarjePostFXPass(_processors.ToArray(), settings.injectionPoint);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        if (!settings.runInSceneView && data.cameraData.isSceneViewCamera)
        {
            return;
        }
        if (_processors == null || _processors.Count == 0)
        {
            return;
        }
        
        renderer.EnqueuePass(_pass);
    }

    private void Destroy()
    {
        _processors.ForEach(p => p.Destroy());
    }

    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;
        public bool runInSceneView = true;
        public List<LarjePostFX> effects = new List<LarjePostFX>();
    }
}

