using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInputClickTarget
{
    public PlayerInputCommandData OnCameraClick(RaycastHit hit);
}
