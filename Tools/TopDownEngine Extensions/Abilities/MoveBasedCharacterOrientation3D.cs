#if MOREMOUNTAINS_TOPDOWNENGINE

using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Larje.Core.Tools.TopDownEngine
{
    public class MoveBasedCharacterOrientation3D : CharacterAbility
    {
        public Transform forceTarget;
        
        [SerializeField] private float minRotationSpeed;
        [SerializeField] private float maxRotationSpeed;
        [Space] 
        [SerializeField] private Transform model;

        private Vector3 _lastPosition;
        private float _lastRotateTime;
        private Vector3 _currentDirection;
        private ActualSpeedCharacterMovement _actualSpeedMovement;

        protected override void Initialization()
        {
            base.Initialization();
            _actualSpeedMovement = _character.FindAbility<ActualSpeedCharacterMovement>();
        }

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
    }
}

#endif