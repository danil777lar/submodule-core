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
    private List<UIAnimationBase> _animations;

    private List<UIAnimationBase> Animations
    {
        get
        {
            if (_animations == null)
            {
                _animations = GetComponentsInChildren<UIAnimationBase>().ToList();
            }
            
            return _animations;
        }
    }

    // POINTER ---------------------------------------------
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Animations.ForEach(a => a.PointerDown());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Animations.ForEach(a => a.PointerUp());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Animations.ForEach(a => a.PointerEnter());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animations.ForEach(a => a.PointerExit());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Animations.ForEach(a => a.PointerClick());
    }


    // SELECTABLE ---------------------------------------------

    public void OnSelect(BaseEventData eventData)
    {
        Animations.ForEach(a => a.SelectableSelect());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Animations.ForEach(a => a.SelectableDeselect());
    }


    // DRAGGABLE ---------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        Animations.ForEach(a => a.DraggableBeginDrag());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Animations.ForEach(a => a.DraggableEndDrag());
    }

    private void Start()
    {

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
            Animations.ForEach(a => a.SelectableInteractableOn());
        }
        else
        {
            Animations.ForEach(a => a.SelectableInteractableOff());
        }
    }
}
