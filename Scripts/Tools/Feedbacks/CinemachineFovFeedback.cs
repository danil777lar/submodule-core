#if MoreMountains

using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

[FeedbackPath("Camera/Cinemachine Fov")]
public class CinemachineFovFeedback : MMF_FeedbackBase
{
    #if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.CameraColor; } }
    #endif
    
    [Space] 
    public int ListenerChannel;
    public AnimationCurve CurveIntensity;
 
    protected override void FillTargets()
    {
        
    }

    public override void Play(Vector3 position, float feedbacksIntensity = 1)
    {
        base.Play(position, feedbacksIntensity);
        CinemachineFovListener.SetFovModifier(ListenerChannel, CurveIntensity.Evaluate(feedbacksIntensity));
    }

    public override void Stop(Vector3 position, float feedbacksIntensity = 1)
    {
        base.Stop(position, feedbacksIntensity);
        CinemachineFovListener.SetFovModifier(ListenerChannel, 1f);
    }

    public override void ResetFeedback()
    {
        base.ResetFeedback();
        CinemachineFovListener.SetFovModifier(ListenerChannel, 1f);
    }
}
#endif