using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInputDragTarget
{
    public bool IsDraggable();

    public Transform GetTransform();
}
