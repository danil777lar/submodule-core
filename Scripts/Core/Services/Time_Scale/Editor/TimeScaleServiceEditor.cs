using System;
using ProjectConstants;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Services
{
    [CustomEditor(typeof(TimeScaleService))]
    public class TimeScaleServiceEditor : Editor
    {
        private TimeScaleService _timeScaleService;

        private void OnEnable()
        {
            _timeScaleService = target as TimeScaleService;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                EditorUtility.SetDirty(target);

                if (_timeScaleService)
                {
                    GUILayout.Space(15);
                    DrawSlider("Total", _timeScaleService.GetTotalTimeScale());
                    GUILayout.Space(15);
                    foreach (TimeScaleLayerType layer in Enum.GetValues(typeof(TimeScaleLayerType)))
                    {
                        DrawSlider(layer.ToString(), _timeScaleService.GetTimeScale(layer));
                        GUILayout.Space(15);
                    }
                }
            }
        }

        private void DrawSlider(string label, float value)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(label, GUILayout.Width(100));
            GUILayout.HorizontalSlider(value, 0f, 1f);

            GUILayout.EndHorizontal();
        }
    }
}