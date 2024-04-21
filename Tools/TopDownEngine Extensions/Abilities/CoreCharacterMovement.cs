#if MOREMOUNTAINS_TOPDOWNENGINE

using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Larje.Core.Tools.TopDownEngine
{
    public class CoreCharacterMovement : CharacterMovement
    {
        [SerializeField] private bool drawLimitsGizmo = true;
        [SerializeField, Min(0f)] private float minActualSpeedAnimator; 

        private bool _useLimit;
        private float _limitRange;
        private Vector3 _limitDirection;
        private Vector3 _lastPosition;

        protected const string _blendAnimationParameterName = "MoveBlend";
        protected const string _actualSpeedAnimationParameterName = "ActualSpeed";
        protected const string _modelRelativeDirectionXParameterName = "ModelRelativeDirectionX";
        protected const string _modelRelativeDirectionYParameterName = "ModelRelativeDirectionY";
        protected int _blendAnimationParameter;
        protected int _actualSpeedAnimationParameter;
        protected int _modelRelativeDirectionXParameter;
        protected int _modelRelativeDirectionYParameter;

        public float ActualSpeed { get; private set; }
        public float ActualSpeedPercent { get; private set; }
        public Vector3 ActualDirection { get; private set; }
        public Vector3 ModelRelativeDirection { get; private set; }

        public override void UpdateAnimator()
        {
            base.UpdateAnimator();

            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _blendAnimationParameter, ActualSpeedPercent,
                _character._animatorParameters, _character.RunAnimatorSanityChecks);

            float actualSpeedAnimator = Mathf.Max(ActualSpeed, minActualSpeedAnimator);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _actualSpeedAnimationParameter, actualSpeedAnimator,
                _character._animatorParameters, _character.RunAnimatorSanityChecks);

            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _modelRelativeDirectionXParameter,
                ModelRelativeDirection.x, _character._animatorParameters, _character.RunAnimatorSanityChecks);

            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _modelRelativeDirectionYParameter,
                ModelRelativeDirection.z, _character._animatorParameters, _character.RunAnimatorSanityChecks);
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

        protected void UpdateMovement(float deltaTime)
        {
            if (deltaTime > 0f)
            {
                ActualSpeed = Vector3.Distance(transform.position, _lastPosition) / deltaTime;
                ActualDirection = (transform.position - _lastPosition).normalized;
                ModelRelativeDirection = _character.CharacterModel.transform.InverseTransformDirection(ActualDirection);
                _lastPosition = transform.position;

                if (MovementSpeed > 0f)
                {
                    ActualSpeedPercent = ActualSpeed / MovementSpeed;
                }
                else
                {
                    ActualSpeedPercent = 0f;
                }
            }
        }

        protected override void InitializeAnimatorParameters()
        {
            base.InitializeAnimatorParameters();

            RegisterAnimatorParameter(_blendAnimationParameterName, AnimatorControllerParameterType.Float,
                out _blendAnimationParameter);

            RegisterAnimatorParameter(_actualSpeedAnimationParameterName, AnimatorControllerParameterType.Float,
                out _actualSpeedAnimationParameter);

            RegisterAnimatorParameter(_modelRelativeDirectionXParameterName, AnimatorControllerParameterType.Float,
                out _modelRelativeDirectionXParameter);

            RegisterAnimatorParameter(_modelRelativeDirectionYParameterName, AnimatorControllerParameterType.Float,
                out _modelRelativeDirectionYParameter);
        }

        protected override void HandleDirection()
        {
            base.HandleDirection();

            if (_useLimit && _limitDirection != Vector3.zero)
            {
                Vector3 direction = new Vector3(_horizontalMovement, 0f, _verticalMovement);
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

        private void FixedUpdate()
        {
            UpdateMovement(Time.fixedDeltaTime);
            if (!AbilityPermitted || !AbilityAuthorized)
            {
                _horizontalMovement = 0f;
                _verticalMovement = 0f;
                SetMovement();
            }
        }

        private Vector3 RotateVector(Vector3 vector, float angle)
        {
            return (Quaternion.Euler(0, angle, 0) * vector);
        }
    }
}

#endif