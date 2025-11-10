using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventsRoot : MonoBehaviour, 
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
    ISelectHandler, IDeselectHandler,
    IBeginDragHandler, IEndDragHandler
{
    private bool _interactable = true;

    private Selectable _selectable;
    private List<UIAnimationBase> _animatons;

    // POINTER ---------------------------------------------
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _animatons.ForEach(a => a.PointerDown());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _animatons.ForEach(a => a.PointerUp());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animatons.ForEach(a => a.PointerEnter());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animatons.ForEach(a => a.PointerExit());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _animatons.ForEach(a => a.PointerClick());
    }


    // SELECTABLE ---------------------------------------------

    public void OnSelect(BaseEventData eventData)
    {
        _animatons.ForEach(a => a.SelectableSelect());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _animatons.ForEach(a => a.SelectableDeselect());
    }


    // DRAGGABLE ---------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        _animatons.ForEach(a => a.DraggableBeginDrag());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _animatons.ForEach(a => a.DraggableEndDrag());
    }

    private void Start()
    {
        _animatons = GetComponentsInChildren<UIAnimationBase>().ToList();

        _selectable = GetComponent<Selectable>();
        if (_selectable != null)
        {
            _interactable = _selectable.interactable;
            SendInteractableEvent(_interactable);
        }
    }

    private void Update()
    {
        if (_selectable != null && _interactable != _selectable.interactable)
        {
            _interactable = _selectable.interactable;
            SendInteractableEvent(_interactable);
        }
    }

    private void SendInteractableEvent(bool interactable)
    {
        if (interactable)
        {
            _animatons.ForEach(a => a.SelectableInteractableOn());
        }
        else
        {
            _animatons.ForEach(a => a.SelectableInteractableOff());
        }
    }
}
