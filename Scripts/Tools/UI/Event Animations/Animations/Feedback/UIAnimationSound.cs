using UnityEngine;

public class UIAnimationSound : UIAnimationBase
{
    [SerializeField] private SoundSettings soundForward;
    [SerializeField] private SoundSettings soundBackwards;
    
    protected override float OnEvent(bool forward)
    {
        if (forward)
        {
            soundForward.Play();
        }
        else
        {
            soundBackwards.Play();
        }
        
        return 0f;
    }
}
