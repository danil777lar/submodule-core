using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Tools.SwipeProcessor
{
    [RequireComponent(typeof(RectTransformEvents))]
    public class SwipeProcessor : MonoBehaviour
    {
        [SerializeField] private SwipeProcessorConfig config;
        
        private bool _isActive = true;
        private bool _pointerDown;
        private Vector2 _pointerDownPoint;
        private RectTransformEvents _rect;

        private Dictionary<Vector2, Action> _directions;

        public void Activate(bool isActive)
        {
            _isActive = isActive;
        }

        public void BindDirection(Vector2 direction, Action onSwipe)
        {
            _directions ??= new Dictionary<Vector2, Action>();
            if (!_directions.ContainsKey(direction))
            {
                _directions.Add(direction, () => { });
            }
            _directions[direction] += onSwipe;
        }

        public void Clear()
        {
            _directions?.Clear();
        }

        private void Start()
        {
            _rect = GetComponent<RectTransformEvents>();
            _rect.EventPointerDown += OnPointerDown;
            _rect.EventPointerUp += OnPointerUp;
        }

        private void Update()
        {
            if (_pointerDown && SwipeLength() >= config.MaxLength)
            {
                Swipe();    
            }   
        }

        private void OnPointerDown(PointerEventData data)
        {
            if (_isActive && config.Enabled)
            {
                _pointerDown = true;
                _pointerDownPoint = data.position;
            }
        }

        private void OnPointerUp(PointerEventData data)
        {
            if (_isActive && config.Enabled && _pointerDown)
            {
                if (SwipeLength() >= config.MinLength)
                {
                    Swipe();
                }
                _pointerDown = false;
            }
        }

        private void Swipe()
        {
            if (_pointerDown)
            {
                _pointerDown = false;
                Vector2 currentDirection = ((Vector2)Input.mousePosition - _pointerDownPoint).normalized;
                Vector2 targetDirection = _directions.Keys
                    .OrderBy(x => Vector2.Distance(x.normalized, currentDirection)).First();
                if (Vector2.Angle(currentDirection, targetDirection) <= config.MaxAngle)
                {
                    _directions[targetDirection].Invoke();
                }
            }
        }

        private float SwipeLength()
        {
            return Vector2.Distance(_pointerDownPoint, Input.mousePosition) / Screen.width;
        }
    }
}