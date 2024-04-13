#if MOREMOUNTAINS_TOPDOWNENGINE

using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Larje.Core.Tools.TopDownEngine
{
    public class CoreCharacterOrientation3D : CharacterAbility
    {
        public Transform forceTarget;
        public Transform forceLookTarget;
        
        [SerializeField] private Vector3 rotationMultiplier = Vector3.one;
        [SerializeField] private float minRotationSpeed;
        [SerializeField] private float maxRotationSpeed;
        [Space] 
        [SerializeField] private Transform modelLook;

        private float _lastRotateTime;
        private Vector3 _lastPosition;
        private Vector3 _currentDirection;
        private Vector3 _currentLookDirection;
        private CoreCharacterMovement _coreMovement;
        
        public Vector3 Direction => _currentDirection;
        public Vector3 LookDirection => _currentLookDirection;

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                return;
            }

            if (!AbilityAuthorized)
            {
                return;
            }

            CatchDirection();
            Rotate();
            
            CatchLookDirection();
            RotateLook();
        }

        protected override void Initialization()
        {
            base.Initialization();
            _coreMovement = _character.FindAbility<CoreCharacterMovement>();
        }

        private void CatchDirection()
        {
            if (forceTarget)
            {
                _currentDirection = (forceTarget.position - transform.position).normalized;
            }
            else
            {
                _currentDirection = (transform.position - _lastPosition).normalized;   
            }
            _lastPosition = transform.position;
        }

        private void CatchLookDirection()
        {
            if (forceLookTarget)
            {
                _currentLookDirection = (forceLookTarget.position - transform.position).normalized.XZ();
            }
            else
            {
                float angle = Mathf.Sin(Time.time) * 45f;
                _currentLookDirection = Quaternion.Euler(0f, angle, 0f) * _currentDirection;
            }
        }

        private void Rotate()
        {
            float timeDelta = Time.time - _lastRotateTime; 
            if (_currentDirection != Vector3.zero)
            {
                float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed,
                    _coreMovement.ActualSpeedPercent) * timeDelta;
                Quaternion rotation = Quaternion.LookRotation(_currentDirection);
                rotation = Quaternion.Euler(Vector3.Scale(rotation.eulerAngles, rotationMultiplier));
                _character.CharacterModel.transform.rotation = Quaternion.RotateTowards(
                    _character.CharacterModel.transform.rotation, rotation, rotationSpeed);
            }
            _lastRotateTime = Time.time;
        }

        private void RotateLook()
        {
            if (modelLook != null && _currentLookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(_currentLookDirection);
                modelLook.rotation = Quaternion.Lerp(modelLook.rotation, rotation, Time.fixedDeltaTime * 10f);
            }
        }
    }
}

#endif