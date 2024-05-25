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
        
        public Data GetData()
        {
            double percent = Projector.spline.Project(transform.position).percent;
            double halfLengthPercent = Projector.spline.Travel(0f, size.x * 0.5f);

            double xFrom = PrettyPercent(percent - halfLengthPercent);
            double xTo = PrettyPercent(percent + halfLengthPercent);
            double yFrom = yPos;
            double yTo = yPos + size.y;
            Data data = new Data(xFrom, xTo, yFrom, yTo);
            
            return data;
        }

        private void OnValidate()
        {
            size.x = Mathf.Max(0f, size.x);
            size.y = Mathf.Max(0f, size.y);
            yPos = Mathf.Max(0f, yPos);
            Projector.targetObject = gameObject;
            if (Projector.spline == null)
            {
                Projector.spline = GetComponentInParent<SplineComputer>();
            }            
        }

        private double PrettyPercent(double percent)
        {
            if (Projector.spline.isClosed)
            {
                percent %= 1.0;
                if (percent < 0.0)
                {
                    percent += 1.0;
                }
            }
            else
            {
                percent = Math.Clamp(percent, 0f, 1f);
            }

            return percent;
        }

        [Serializable]
        public struct Data
        {
            public readonly double XFrom;
            public readonly double XTo;
            public readonly double YFrom;
            public readonly double YTo;
            
            public Data(double xFrom, double xTo, double yFrom, double yTo)
            {
                XFrom = xFrom;
                XTo = xTo;
                YFrom = yFrom;
                YTo = yTo;
            }
        }
    }
}

#endif