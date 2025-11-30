using System;
using UnityEngine;

public interface ILevelObjectDataUser
{
    public Type DataType { get; }
    public void InjectData(object data);
    public object ReadData();
}