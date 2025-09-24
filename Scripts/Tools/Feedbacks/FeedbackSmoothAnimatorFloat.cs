#if MoreMountains

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

[FeedbackPath("Animation/Smooth Animator Float")]
public class FeedbackSmoothAnimatorFloat : MMF_FeedbackBase
{
#if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.AnimationColor; } }
#endif

    [Space] 
    public Animator TargetAnimator;
    public float TargetValue = 1f;
    public string ParameterName;
 
    protected override void FillTargets()
    {
        
    }

    public override void Play(Vector3 position, float feedbacksIntensity = 1)
    {
        base.Play(position, feedbacksIntensity);

        DOTween.Kill(this);
        DOTween.To(() => TargetAnimator.GetFloat(ParameterName),
            x => TargetAnimator.SetFloat(ParameterName, x),
            TargetValue, Duration)
            .SetTarget(this);
    }

    public override void Stop(Vector3 position, float feedbacksIntensity = 1)
    {
        base.Stop(position, feedbacksIntensity);
        
        DOTween.Kill(this);
    }

    public override void ResetFeedback()
    {
        base.ResetFeedback();
        
        DOTween.Kill(this);
    }
}
#endif