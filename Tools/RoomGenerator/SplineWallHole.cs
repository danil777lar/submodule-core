#if DREAMTECK_SPLINES

using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(SplinePositioner))]
    public class SplineWallHole : MonoBehaviour
    {
        [SerializeField] private float height;
        [SerializeField] private Vector2 size;

        private SplinePositioner _positioner;
        private SplinePositioner Positioner
        {
            get
            {
                if (_positioner == null)
                {
                    _positioner = GetComponentInParent<SplinePositioner>();
                }

                return _positioner;
            }
        }

        public Data GetData()
        {
            return new Data()
            {
                height = height,
                size = size,
                position = transform.position
            };
        }

        public class Data
        {
            public float height;
            public Vector2 size;
            public Vector3 position;
        }
    }
}

#endif