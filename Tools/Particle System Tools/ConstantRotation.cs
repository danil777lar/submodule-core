using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools.EffectsTools
{
    public class ConstantRotation : MonoBehaviour
    {
        [SerializeField] private Vector3 rotation;
        [SerializeField] private Space space;
        [SerializeField] private UpdateType updateType;
        
        private void Update()
        {
            if (updateType == UpdateType.Normal)
            {
                Rotate();
            }
        }
        
        private void FixedUpdate()
        {
            if (updateType == UpdateType.Fixed)
            {
                Rotate();
            }
        }
        
        private void LateUpdate()
        {
            if (updateType == UpdateType.Late)
            {
                Rotate();
            }
        }
        
        private void Rotate()
        {
            Quaternion quaternion = Quaternion.Euler(rotation);
            
            switch (space)
            {
                case Space.World:
                    transform.rotation = quaternion;
                    break;
                case Space.Local:
                    transform.localRotation = quaternion;
                    break;
            }
        }
        
        private enum Space
        {
            World,
            Local
        }

        private enum UpdateType
        {
            Normal,
            Fixed,
            Late
        }
    }
}
