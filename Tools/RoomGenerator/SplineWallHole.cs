#if DREAMTECK_SPLINES

using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(SplineProjector))]
    public class SplineWallHole : MonoBehaviour
    {
        [SerializeField] private float height;
        [SerializeField] private Vector2 size;
        
        private SplineProjector _projector;
        private SplineProjector Projector
        {
            get
            {
                if (_projector == null)
                {
                    _projector = GetComponent<SplineProjector>();
                }

                return _projector;
            }
        }

        private void OnValidate()
        {
            size.x = Mathf.Max(0f, size.x);
            size.y = Mathf.Max(0f, size.y);
            height = Mathf.Max(0f, height);
        }

        public Data GetData()
        {
            Data data = new Data()
            {
                height = height,
                distance = Projector.CalculateLength(0f, Projector.GetPercent()),
                size = size,
                position = transform.position
            };
            
            return data;
        }

        [Serializable]
        public class Data
        {
            public float height;
            public float distance;
            public Vector2 size;
            public Vector3 position;
        }
    }
}

#endif