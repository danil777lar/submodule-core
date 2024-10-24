using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Animancer;

namespace Larje.Core.Tools.GunController
{
    public class GunController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        
        private float _ikWeight;
        private Gun _gun;


        private void Start()
        {
            _gun = GetComponentInChildren<Gun>();
        }


        private void OnAnimatorIK(int layerIndex)
        {
            _ikWeight = 1f;

            if (_gun && _gun.RightArmTarget) 
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _ikWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _ikWeight);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, _gun.RightArmTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, _gun.RightArmTarget.rotation);
            }
            if (_gun && _gun.LeftArmTarget) 
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _ikWeight);
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, _gun.LeftArmTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, _gun.LeftArmTarget.rotation);
            }
        }

        [ContextMenu("Enable Weapon")]
        public void EnableWeapon() 
        {
            _gun?.ChangeState(true, 0.5f);
        }

        [ContextMenu("Disable Weapon")]
        public void DisableWeapon()
        {
            _gun?.ChangeState(false, 1f);
        }

        public void SetAimTarget(Transform target) 
        {
            if (_gun)
                _gun.aimTarget = target;
        }

        [ContextMenu("Shoot")]
        public void Shoot() 
        {
            _gun?.Shoot();
        }

        public void Drop() 
        {
            _gun?.Drop();
            _gun = null;
        }
    }
}