using UnityEngine;

public abstract class LarjePostFX : ScriptableObject
{
    public abstract Processor GetProcessor();
    
    public abstract class Processor
    {
        public abstract Material Material { get; }
        public abstract bool Enabled { get; }

        public abstract void Destroy();
    }
}