using System;
using Larje.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Tools
{
    public class CorePlayerJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Visual")] 
        [SerializeField] private RectTransform joystick;
        [SerializeField] private RectTransform joystickBack;

        [Header("Options")] 
        [SerializeField] private float maxDistance;
        
        [InjectService] private InputService _inputService;

        private bool _isTouching;
        private bool _uiTouch;
        private Vector2 _initialJoystickPosition;
        private Vector2 _startTouchPosition;
        private Vector2 _currentTouchPosition;

        public event Action EventPointerDown;
        public event Action EventPointerUp;

        public Vector2 GetNormalizedValue()
        {
            Vector2 direction = (_currentTouchPosition - _startTouchPosition).normalized;
            float magnitude = (_currentTouchPosition - _startTouchPosition).magnitude / maxDistance;
            return direction * (_isTouching ? magnitude : 0);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _uiTouch = true;
            EventPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _uiTouch = false;
            EventPointerUp?.Invoke();
        }
        
        private void Start()
        {
            DIContainer.InjectTo(this);
            _initialJoystickPosition = joystick.transform.position;
        }

        protected void Update()
        {
            UpdateTouch();
            UpdateJoystick();
        }

        private void UpdateTouch()
        {
            Vector3 pointerValue = _inputService.PlayerPointer.ReadValue<Vector3>();
            _currentTouchPosition = pointerValue.XY();
            if (pointerValue.z > 0 && _uiTouch)
            {
                if (!_isTouching)
                {
                    _startTouchPosition = _currentTouchPosition;
                    _isTouching = true;
                }
            }
            else
            {
                _isTouching = false;
            }
        }

        private void UpdateJoystick()
        {
            if (_isTouching)
            {
                joystick.position = _currentTouchPosition;
                joystickBack.position = _startTouchPosition;
            }
            else
            {
                joystick.position = _initialJoystickPosition;
                joystickBack.position = _initialJoystickPosition;
            }

            Vector2 direction = joystick.position - joystickBack.position;
            if (direction.magnitude > maxDistance)
            {
                _startTouchPosition = joystick.position - (Vector3)(direction.normalized * maxDistance);
            }
        }
    }
}