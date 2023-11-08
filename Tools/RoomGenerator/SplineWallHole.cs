#if DREAMTECK_SPLINES

using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Core.Tools.RoomGenerator
{
    [RequireComponent(typeof(SplineProjector))]
    public class SplineWallHole : MonoBehaviour
    {
        [SerializeField] private float yPos;
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
            yPos = Mathf.Max(0f, yPos);
        }

        public Data GetData()
        {
            Data data = new Data()
            {
                yPos = yPos,
                distance = Projector.CalculateLength(0f, Projector.GetPercent()),
                size = size,
                position = transform.position
            };
            
            return data;
        }

        [Serializable]
        public struct Data
        {
            public float yPos;
            public float distance;
            public Vector2 size;
            public Vector3 position;
        }
    }
}

#endif