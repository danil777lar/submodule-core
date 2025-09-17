using UnityEngine;

[CreateAssetMenu(menuName = "Larje/Core/PostFX/Damage Effect", fileName = "LarjeFX_Damage")]
public class LarjeFXDamage : LarjePostFX
{
    [SerializeField] private Shader shader;
    
    public override LarjePostFX.Processor GetProcessor()
    {
        return new Processor(this);
    }
    
    public new class Processor : LarjePostFX.Processor
    {
        private LarjeFXDamage _config;
        private Material _material;
        
        public override Material Material => _material;
        public override bool Enabled => true;

        public Processor(LarjeFXDamage config)
        {
            _config = config;
            if (_config.shader != null)
            {
                _material = new Material(_config.shader);
            }
        }

        public void SetValue(float v)
        {
            _material.SetFloat("_Value", v);
        }
        
        public override void Destroy()
        {
            if (Material != null)
            {
                Object.Destroy(Material);
            }
        }
    }
}
