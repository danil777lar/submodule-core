#if MOREMOUNTAINS_TOPDOWNENGINE

using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Larje.Core.Tools.TopDownEngine
{
    public class ActualSpeedCharacterMovement : CharacterMovement
    {
        private Vector3 _lastPosition;

        protected const string _blendAnimationParameterName = "MoveBlend";
        protected const string _actualSpeedAnimationParameterName = "ActualSpeed";
        protected int _blendAnimationParameter;
        protected int _actualSpeedAnimationParameter;

        public float ActualSpeed { get; private set; }
        public float ActualSpeedPercent { get; private set; }

        protected override void InitializeAnimatorParameters()
        {
            base.InitializeAnimatorParameters();
            RegisterAnimatorParameter(_blendAnimationParameterName, AnimatorControllerParameterType.Float,
                out _blendAnimationParameter);
            RegisterAnimatorParameter(_actualSpeedAnimationParameterName, AnimatorControllerParameterType.Float,
                out _actualSpeedAnimationParameter);
        }

        public override void UpdateAnimator()
        {
            base.UpdateAnimator();
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _blendAnimationParameter, ActualSpeedPercent,
                _character._animatorParameters, _character.RunAnimatorSanityChecks);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _actualSpeedAnimationParameter, ActualSpeed,
                _character._animatorParameters, _character.RunAnimatorSanityChecks);
        }

        protected void FixedUpdate()
        {
            ActualSpeed = Vector3.Distance(transform.position, _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = transform.position;
            ActualSpeedPercent = ActualSpeed / MovementSpeed;
        }
    }
}

#endif