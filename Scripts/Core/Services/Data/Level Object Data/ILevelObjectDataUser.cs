using System;
using UnityEngine;

public interface ILevelObjectDataUser
{
    public GameObject GameObject { get; }
    public Type DataType { get; }
    public void InjectData(object data);
    public object ReadData();
}
