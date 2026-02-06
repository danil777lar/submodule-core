using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectGroupActivator : MonoBehaviour, ILevelObjectDataUser
{
    [SerializeField] private List<GameObject> targets;

    public Type DataType => typeof(Data);

    public void InjectData(object data)
    {
        Data d = (Data)data;

        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].SetActive(d.isActive[i]);
        }
    }

    public object ReadData()
    {
        bool[] result = new bool[targets.Count];
        for (int i = 0; i < targets.Count; i++)
        {
            result[i] = targets[i].activeSelf;
        }
        return new Data { isActive = result };
    }

    public class Data
    {
        public bool[] isActive;
    }
}
