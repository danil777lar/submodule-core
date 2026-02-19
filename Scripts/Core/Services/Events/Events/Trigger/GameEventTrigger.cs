using Larje.Core;
using UnityEngine;

namespace Larje.Core
{
    public class GameEventTrigger : GameEvent
    {
        public override bool IsValid => _trigger != null;

        private TriggerConstant _trigger;
        private float _value;

        public TriggerConstant Trigger => _trigger;
        public float ValueFloat => _value;
        public bool ValueBool => _value >= 1f;

        public GameEventTrigger(TriggerConstant trigger, float value, string source) : base(source)
        {
            _trigger = trigger;
            _value = value;
        }
    }
}
