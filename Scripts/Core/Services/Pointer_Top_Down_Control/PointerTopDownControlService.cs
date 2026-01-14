using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using UnityEngine;
using UnityEngine.EventSystems;

[BindService(typeof(PointerTopDownControlService))]
public class PointerTopDownControlService : Service
{
    [SerializeField, Min(1)] private int cameraMoveSmooth;
    [SerializeField] private float holdTime = 1f;
    [SerializeField] private float cameraSensitivity = 0.01f;
    [SerializeField] private LayerMask mask; 

    private bool _pointerDragged;
    private Camera _camera;
    private Tween _holdTween;
    private GameObject _playCamera;
    private RectTransformEvents _rect;
    private IPlayerInputDragTarget _dragTarget;
    private List<Vector2> _lastDelta;
        
    public event Action<PlayerInputCommandData> EventPlayerCommand;
    
    public override void Init()
    {
        _camera = Camera.main;
        _playCamera = GameObject.FindGameObjectWithTag("PlayCamera");
    }

    public void SetControlRect(RectTransformEvents rect)
    {
        if (_rect != null)
        {
            _rect.EventPointerDrag -= OnPointerDrag;
            _rect.EventPointerDown -= OnPointerDown;
            _rect.EventPointerUp -= OnPointerUp;
        }
        
        if (rect != null)
        {
            rect.EventPointerDrag += OnPointerDrag;
            rect.EventPointerDown += OnPointerDown;
            rect.EventPointerUp += OnPointerUp;
        }

        _rect = rect;
    }

    private void OnPointerDrag(PointerEventData data)
    {
        _pointerDragged = true;

        TryStopHold();

        Vector2 delta = GetCameraDelta(data.delta);
        if (_dragTarget != null)
        {
            _dragTarget.GetTransform().position += new Vector3(delta.x, 0f, delta.y) * cameraSensitivity;
        }
        else
        {
            _playCamera.transform.position += new Vector3(-delta.x, 0f, -delta.y) * cameraSensitivity;   
        }
    }

    private void OnPointerDown(PointerEventData data)
    {
        _lastDelta = new List<Vector2>();
        Ray ray = _camera.ScreenPointToRay(data.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, mask))
        {
            if (hit.collider.TryGetComponent(out IPlayerInputDragTarget dragTarget) && dragTarget.IsDraggable())
            {
                _dragTarget = dragTarget;
            }
            else if (hit.collider.TryGetComponent(out IPlayerInputHoldTarget holdTarget))
            {
                StartHold(holdTarget);
            }
        }

        _pointerDragged = false;
    }
    
    private void OnPointerUp(PointerEventData data)
    {
        _dragTarget = null;
        if (!_pointerDragged)
        {
            Ray ray = _camera.ScreenPointToRay(data.position);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, mask))
            {
                PlayerInputCommandData command =
                    hit.collider.GetComponentInParent<IPlayerInputClickTarget>()?.OnCameraClick(hit); 
                EventPlayerCommand?.Invoke(command);
                TryStopHold();
            }
        }
    }

    private void StartHold(IPlayerInputHoldTarget holdTarget)
    {
        _holdTween?.Kill();
        _holdTween = DOTween.To(() => 0f, (x) => { }, 1f, holdTime)
            .OnComplete(() =>
            {
                holdTarget.OnCameraHold();
            });
    }

    private void TryStopHold()
    {
        _holdTween?.Kill();
    }

    private Vector2 GetCameraDelta(Vector2 newDelta)
    {
        _lastDelta.Add(newDelta);
        if (_lastDelta.Count > cameraMoveSmooth)
        {
            _lastDelta.RemoveAt(0);
        }

        Vector2 result = Vector2.zero;
        _lastDelta.ForEach(x => result += x);
        result /= _lastDelta.Count;
        return result;
    }
}
