using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Larje.Core.Services;
using Lofelt.NiceVibrations;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Larje.Core.Tools
{
    public class ButtonInteractionFeedback : MonoBehaviour, 
        IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ButtonInteractionFeedbackConfig config;
        
        [InjectService] private DataService _dataService;
        [InjectService] private SoundService _soundService;
        
        private Material _material;
        private Selectable _selectable;
        private ButtonInteractionConnector _connector;
        
        private Dictionary<ButtonInteractionStateType, Dictionary<string, string>> _states;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            SetActiveState(ButtonInteractionStateType.Pressed, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetActiveState(ButtonInteractionStateType.Pressed, false);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            SetActiveState(ButtonInteractionStateType.Hovered, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetActiveState(ButtonInteractionStateType.Hovered, false);
        }

        private void Start()
        {
            DIContainer.InjectTo(this);
            
            _selectable = GetComponent<Selectable>();
            if (config == null)
            {
                Debug.LogError("Button Interaction Feedback: config is null", gameObject);
                return;
            }
            
            InitMaterial();
            
            _states = new Dictionary<ButtonInteractionStateType, Dictionary<string, string>>();

            ButtonProperties properties = new ButtonProperties(transform, _material);
            _connector = new ButtonInteractionConnector(properties);
            
            SetActiveState(ButtonInteractionStateType.Default, true);
        }

        private void Update()
        {
            SetActiveState(ButtonInteractionStateType.NoInteractable, !_selectable.interactable);
            
            _states ??= new Dictionary<ButtonInteractionStateType, Dictionary<string, string>>();
            
            _connector.Clear();
            Dictionary<string, List<object>> values = new Dictionary<string, List<object>>();
            foreach (ButtonInteractionState state in config.States)
            {
                if (!_states.ContainsKey(state.stateType))
                {
                    _states[state.stateType] = new Dictionary<string, string>();
                }
                
                _states[state.stateType][ButtonInteractionEffect.KEY_DELTA_TIME] = Time.deltaTime.ToString();
                state.Evaluate(_connector, _states[state.stateType]);
            }
            
            _connector.Apply();
        }

        private void InitMaterial()
        {
            if (config.Material != null)
            {
                _material = Instantiate(config.Material);
                foreach (Image image in GetComponentsInChildren<Image>())
                {
                    image.material = _material;
                }
            }
        }

        private void OnDisable()
        {
            this.DOKill();
            transform.localScale = Vector3.one;
        }

        private void OnDestroy()
        {
            this.DOKill();
            if (_material != null)
            {
                Destroy(_material);
            }
        }

        private void SetActiveState(ButtonInteractionStateType state, bool isActive)
        {
            _states ??= new Dictionary<ButtonInteractionStateType, Dictionary<string, string>>();
            if (!_states.ContainsKey(state))
            {
                _states[state] = new Dictionary<string, string>();
            }
            _states[state][ButtonInteractionEffect.KEY_IS_ACTIVE] = isActive.ToString();
        }

        private ButtonInteractionState GetState(ButtonInteractionStateType type)
        {
            return config.States.ToList().Find(x => x.stateType == type);
        }
    }
}
