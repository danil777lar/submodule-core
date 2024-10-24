using UnityEngine;
using DG.Tweening;
using Larje.Core.Services;
using Lofelt.NiceVibrations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Larje.Core.Tools
{
    public class ButtonInteractionFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ButtonInteractionFeedbackConfig config;
        [InjectService] private DataService _dataService;
        [InjectService] private SoundService _soundService;
        private Selectable selectable;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (selectable != null && !selectable.interactable)
            {
                PlayInFeedback(config.NonInteractable);   
            }
            else
            {
                PlayInFeedback(config.Interactable);   
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (selectable != null && !selectable.interactable)
            {
                PlayOutFeedback(config.NonInteractable);   
            }
            else
            {
                PlayOutFeedback(config.Interactable);   
            }
        }

        private void Start()
        {
            DIContainer.InjectTo(this);
            selectable = GetComponent<Selectable>();
            if (config == null)
            {
                Debug.LogError("Button Interaction Feedback: config is null", gameObject);
            }
        }

        private void OnDestroy()
        {
            this.DOKill();
        }

        private void PlayInFeedback(ButtonInteractionFeedbackConfig.ButtonInteractionFeedbackOptions options)
        {
            this.DOKill();
            transform.DOScale(options.ScaleValue, options.InAnimDuration)
                .SetUpdate(UpdateType.Normal, true)
                .SetTarget(this)
                .SetEase(options.InAnimEase);

            if (options.UseVibration && _dataService.Data.Settings.Vibration)
            {
                HapticPatterns.PlayPreset(options.VibrationPreset);
            }

            if (options.UseSound && _dataService.Data.Settings.SoundGlobal)
            {
                //_soundService.PlayRandomFromSoundPack(options.SoundType, false);
            }
        }

        private void PlayOutFeedback(ButtonInteractionFeedbackConfig.ButtonInteractionFeedbackOptions options)
        {
            this.DOKill();
            transform.DOScale(1f, options.OutAnimDuration)
                .SetUpdate(UpdateType.Normal, true)
                .SetTarget(this)
                .SetEase(options.OutAnimEase);
        }
    }
}
