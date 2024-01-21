using System;
using System.Linq;
using UnityEngine;

namespace Larje.Core.Services
{
    [CreateAssetMenu(menuName = "Configs/Time Scale Config", fileName = "Time Scale Config")]
    public class TimeScaleServiceConfig : ScriptableObject
    {
        private static TimeScaleServiceConfig _instance;
        public static TimeScaleServiceConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<TimeScaleServiceConfig>("Time Scale Config");
                }

                return _instance;
            }
        }
        
        [field: SerializeField] public float MinTimescale { get; private set; }
        [field: SerializeField] public TimeScaleAnimation[] TimeScaleAnimations { get; private set; }


        private void OnValidate()
        {
            if (TimeScaleAnimations != null)
            {
                TimeScaleAnimations.ToList().ForEach(x => x.Validate());
            }
        }
    }
}