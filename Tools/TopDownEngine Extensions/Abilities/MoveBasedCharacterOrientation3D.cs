#if MOREMOUNTAINS_TOPDOWNENGINE

using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Larje.Core.Tools.TopDownEngine
{
    public class MoveBasedCharacterOrientation3D : CharacterAbility
    {
        public Transform forceTarget;
        public Transform forceLookTarget;
        
        [SerializeField] private float minRotationSpeed;
        [SerializeField] private float maxRotationSpeed;
        [Space] 
        [SerializeField] private Transform model;
        [SerializeField] private Transform modelLook;

        private float _lastRotateTime;
        private Vector3 _lastPosition;
        private Vector3 _currentDirection;
        private Vector3 _currentLookDirection;
        private ActualSpeedCharacterMovement _actualSpeedMovement;
        
        public Vector3 LookDirection => _currentLookDirection;

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                return;
            }

            if (model == null)
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
            _actualSpeedMovement = _character.FindAbility<ActualSpeedCharacterMovement>();
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
                _currentLookDirection = _currentDirection;
            }
        }

        private void Rotate()
        {
            float timeDelta = Time.time - _lastRotateTime; 
            if (_currentDirection != Vector3.zero)
            {
                float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed,
                    _actualSpeedMovement.ActualSpeedPercent) * timeDelta;
                Quaternion rotation = Quaternion.LookRotation(_currentDirection);
                model.rotation = Quaternion.RotateTowards(model.rotation, rotation, rotationSpeed);
            }
            _lastRotateTime = Time.time;
        }

        private void RotateLook()
        {
            if (modelLook != null && _currentLookDirection != Vector3.zero)
            {
                modelLook.rotation = Quaternion.LookRotation(_currentLookDirection);
            }
        }
    }
}

#endif