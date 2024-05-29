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
        [Header("Main")] 
        [SerializeField] private float minRotationSpeed;
        [SerializeField] private float maxRotationSpeed;
        [Header("Look")] 
        [SerializeField, Range(0f, 360f)] private float lookAngle;
        [SerializeField] private float lookSpeed;
        [SerializeField] private Transform modelLook;

        private Vector3 _lastPosition;
        
        private Vector3 _targetDirection;
        private Vector3 _currentDirection;
        
        private Vector3 _currentLookDirection;
        
        private CoreCharacterMovement _coreMovement;

        public Vector3 TargetDirection => _targetDirection;
        public Vector3 CurrentDirection => _currentDirection;
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
            _targetDirection = _character.CharacterModel.transform.forward;
            _currentLookDirection = _targetDirection;
        }

        private void CatchDirection()
        {
            Vector3 newDirection = forceTarget ? 
                (forceTarget.position - transform.position).normalized : 
                (transform.position - _lastPosition).normalized;

            newDirection = Vector3.Scale(newDirection, directionMultiplier);
            _targetDirection = newDirection != Vector3.zero ? newDirection : _targetDirection;
            
            _lastPosition = transform.position;
        }

        private void CatchLookDirection()
        {
            Vector3 newDirection = forceLookTarget ? 
                forceLookTarget.position - transform.position : 
                _targetDirection;
            
            newDirection = Vector3.Scale(newDirection, directionMultiplier);
            _currentLookDirection = newDirection != Vector3.zero ? newDirection : _currentLookDirection;
        }

        private void ApplyLimit()
        {
            Vector3 limitDirection = -LookDirection;

            float limit = 360f - lookAngle; 
            float angle = Vector3.Angle(limitDirection, _targetDirection);
            float signedAngle = Vector3.SignedAngle(limitDirection, _targetDirection, Vector3.up);

            if (angle < limit * 0.5f)
            {
                float rotateAngle = limit * 0.5f - angle;
                rotateAngle *= signedAngle < 0f ? -1 : 1f;
                _targetDirection = Quaternion.Euler(0, rotateAngle, 0) * _targetDirection;
            }
        }
        
        private void Rotate()
        {
            if (_targetDirection != Vector3.zero)
            {
                float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed,
                    _coreMovement.ActualSpeedPercent) * Time.deltaTime;
                
                Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
                Quaternion currentRotation = Quaternion.RotateTowards(_character.CharacterModel.transform.rotation,
                    targetRotation, rotationSpeed);
                
                _character.CharacterModel.transform.rotation = currentRotation;
                _currentDirection = _character.CharacterModel.transform.forward;
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