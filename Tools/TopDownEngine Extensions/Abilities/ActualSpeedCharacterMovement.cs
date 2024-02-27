#if MOREMOUNTAINS_TOPDOWNENGINE

using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Larje.Core.Tools.TopDownEngine
{
    public class ActualSpeedCharacterMovement : CharacterMovement
    {
        [SerializeField] private bool drawLimitsGizmo = true;
        
        private bool _useLimit;
        private float _limitRange;
        private Vector3 _limitDirection;
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

        public void SetLimit(Vector3 direction, float range)
        {
            _useLimit = true;
            _limitDirection = direction;
            _limitRange = range;
        }
        
        public void RemoveLimit()
        {
            _useLimit = false;
        }

        protected void FixedUpdate()
        {
            ActualSpeed = Vector3.Distance(transform.position, _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = transform.position;
            ActualSpeedPercent = ActualSpeed / MovementSpeed;
        }

        protected override void HandleDirection()
        {
            base.HandleDirection();

            if (_useLimit && _limitDirection != Vector3.zero)
            {
                Vector3 direction = new Vector3(_horizontalMovement,0f, _verticalMovement);
                if (direction != Vector3.zero)
                {
                    float angle = Vector3.Angle(_limitDirection, direction);
                    float signedAngle = Vector3.SignedAngle(_limitDirection, direction, Vector3.up);

                    if (angle < _limitRange * 0.5f)
                    {
                        float rotateAngle = _limitRange * 0.5f - angle;
                        rotateAngle *= signedAngle < 0f ? -1 : 1f;
                        direction = RotateVector(direction, rotateAngle);
                        _horizontalMovement = direction.x;
                        _verticalMovement = direction.z;
                    }
                }

                if (drawLimitsGizmo)
                {
                    Debug.DrawRay(transform.position, direction.normalized * 2.2f, Color.blue);
                    Debug.DrawRay(transform.position, _limitDirection.normalized * 2f, Color.red);
                    Debug.DrawRay(transform.position, RotateVector(_limitDirection.normalized, _limitRange * 0.5f) * 2f,
                        Color.yellow);
                    Debug.DrawRay(transform.position,
                        RotateVector(_limitDirection.normalized, _limitRange * -0.5f) * 2f, Color.yellow);
                }
            }
        }

        private Vector3 RotateVector(Vector3 vector, float angle)
        {
            return (Quaternion.Euler(0, angle, 0) * vector);
        }
    }
}

#endif