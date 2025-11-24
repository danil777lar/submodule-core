using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Larje/Core/PostFX/Transition Effect", fileName = "LarjeFX_Transition")]
public class LarjeFXTransition : LarjePostFX
{
    [SerializeField] private Shader shader;
    
    public override LarjePostFX.Processor GetProcessor()
    {
        return new Processor(this);
    }
    
    public new class Processor : LarjePostFX.Processor
    {
        private bool _destroyed = false;
        private LarjeFXTransition _config;
        private Material _material;
        
        private List<Func<float>> _valueProviders = new List<Func<float>>();
        
        public override bool Enabled => _material != null && !_destroyed && GetValue() > 0f;
        public override Material Material => _material;

        public Processor(LarjeFXTransition config)
        {
            _config = config;
            if (_config.shader != null)
            {
                _material = new Material(_config.shader);
            }
        }

        public void AddProvider(Func<float> provider)
        {
            if (!_valueProviders.Contains(provider))
            { 
                _valueProviders.Add(provider);
            }
        }
        
        public void RemoveProvider(Func<float> provider)
        {
            if (_valueProviders.Contains(provider))
            { 
                _valueProviders.Remove(provider);
            }
        }

        public override void Update()
        {
            if (_destroyed || _material == null)
            {
                return;
            }
            
            _material.SetFloat("_Value", GetValue());
        }

        public override void Destroy()
        {
            _destroyed = true;
            if (Material != null)
            {
                GameObject.Destroy(Material);
            }
        }

        private float GetValue()
        {
            float v = 0f;
            foreach (Func<float> provider in _valueProviders)
            {
                v = Math.Max(v, provider());
            }

            return v;
        }
    }
}
