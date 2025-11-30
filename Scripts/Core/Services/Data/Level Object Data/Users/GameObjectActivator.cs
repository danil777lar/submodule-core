using System;
using UnityEngine;

public class GameObjectActivator : MonoBehaviour, ILevelObjectDataUser
{
    [SerializeField] private GameObject target;

    public Type DataType => typeof(Data);

    public void InjectData(object data)
    {
        Data d = (Data)data;
        target.SetActive(d.isActive);
    }

    public object ReadData()
    {
        return new Data { isActive = target.activeSelf };
    }

    public class Data
    {
        public bool isActive;
    }
}
