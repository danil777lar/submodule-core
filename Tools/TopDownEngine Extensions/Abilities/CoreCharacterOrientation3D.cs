#if MOREMOUNTAINS_TOPDOWNENGINE

using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Core.Tools.TopDownEngine
{
    public class CoreCharacterOrientation3D : CharacterAbility
    {
        public Transform forceTarget;
        public Transform forceLookTarget;
        
        [Space(40f)]
        [SerializeField] private Vector3 directionMultiplier = Vector3.one;
        [Header("Main")] [SerializeField] private float minRotationSpeed;
        [SerializeField] private float maxRotationSpeed;
        [Header("Look")] 
        [SerializeField, Range(0f, 360f)] private float lookAngle;
        [SerializeField] private float lookSpeed;
        [SerializeField] private Transform modelLook;

        private Vector3 _lastPosition;
        private Vector3 _currentDirection;
        private Vector3 _currentLookDirection;
        private CoreCharacterMovement _coreMovement;

        public Vector3 Direction => _currentDirection;
        public Vector3 LookDirection => modelLook.forward;
        public bool PermitLook { get; private set; }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal || 
                !AbilityAuthorized || !AbilityPermitted)
            {
                PermitLook = false;
                return;
            }

            PermitLook = true;

            CatchDirection();
            CatchLookDirection();

            ApplyLimit();
            RotateLook();
            Rotate();
        }

        protected override void Initialization()
        {
            base.Initialization();
            _coreMovement = _character.FindAbility<CoreCharacterMovement>();

            _lastPosition = transform.position;
            _currentDirection = _character.CharacterModel.transform.forward;
            _currentLookDirection = _currentDirection;
        }

        private void CatchDirection()
        {
            Vector3 newDirection = forceTarget ? 
                (forceTarget.position - transform.position).normalized : 
                (transform.position - _lastPosition).normalized;

            newDirection = Vector3.Scale(newDirection, directionMultiplier);
            _currentDirection = newDirection != Vector3.zero ? newDirection : _currentDirection;
            
            _lastPosition = transform.position;
        }

        private void CatchLookDirection()
        {
            Vector3 newDirection = forceLookTarget ? 
                forceLookTarget.position - transform.position : 
                _currentDirection;
            
            newDirection = Vector3.Scale(newDirection, directionMultiplier);
            _currentLookDirection = newDirection != Vector3.zero ? newDirection : _currentLookDirection;
        }

        private void ApplyLimit()
        {
            Vector3 limitDirection = -LookDirection;

            float limit = 360f - lookAngle; 
            float angle = Vector3.Angle(limitDirection, _currentDirection);
            float signedAngle = Vector3.SignedAngle(limitDirection, _currentDirection, Vector3.up);

            if (angle < limit * 0.5f)
            {
                float rotateAngle = limit * 0.5f - angle;
                rotateAngle *= signedAngle < 0f ? -1 : 1f;
                _currentDirection = Quaternion.Euler(0, rotateAngle, 0) * _currentDirection;
            }
        }
        
        private void Rotate()
        {
            if (_currentDirection != Vector3.zero)
            {
                float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed,
                    _coreMovement.ActualSpeedPercent) * Time.deltaTime;
                
                Quaternion rotation = Quaternion.LookRotation(_currentDirection);
                _character.CharacterModel.transform.rotation = Quaternion.RotateTowards(
                    _character.CharacterModel.transform.rotation, rotation, rotationSpeed);
            }
        }

        private void RotateLook()
        {
            if (modelLook != null && _currentLookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(_currentLookDirection);
                modelLook.rotation = Quaternion.RotateTowards(modelLook.rotation, rotation, 
                    lookSpeed * Time.deltaTime);
            }
        }
    }
}

#endif