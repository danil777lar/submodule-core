using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputCommandData
{
    public float minDistance;
    public float executeDuration;
    public Vector3 targetPosition;
    public Action<CharacterPointerControlAbility> execute;
}
