using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocationEntry
{
    public int Id { get; }
    
    public Vector3 Position { get; }
    public Vector3 Direction { get; }
}
