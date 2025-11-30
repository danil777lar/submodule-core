using System;
using System.Collections.Generic;
using System.Linq;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using Unity.VisualScripting;
using UnityEngine;

public class LevelObjectDataController : MonoBehaviour
{
    [InjectService] private IDataService _dataService;
    [InjectService] private LocationService _locationService;
    
    private string _guid;
    private GUIDHolder _guidHolder;
    private LevelObjectData _data;
    private List<ILevelObjectDataUser> _dataUsers = new List<ILevelObjectDataUser>();

    private void Awake()
    {
        DIContainer.InjectTo(this);
        
        _guidHolder = GetComponent<GUIDHolder>();
        _dataUsers = GetComponentsInChildren<ILevelObjectDataUser>().ToList();
        
        _dataService.EventPreSave += SaveData;

        if (string.IsNullOrEmpty(_guidHolder.GUID))
        {
            Debug.LogWarning($"GUID is null or empty on {gameObject.name}", this);
            return;
        }

        string locationName = _locationService.CurrentLocation.ToString();
        string locationArgs = "";
        foreach (LocationArgType arg in _locationService.CurrentArguments)
        {
            locationArgs += $"({arg.ToString()})";
        }
        _guid = $"{locationName}_{locationArgs}_{_guidHolder.GUID}";
        
        GrabData();
        InjectData();
    }

    private void OnDestroy()
    {
        _dataService.EventPreSave -= SaveData;
    }

    private void GrabData()
    {
        if (string.IsNullOrEmpty(_guid))
        {
            return;
        }

        _data = _dataService.GameData.LevelObjectsDatabase.Objects.Find(x => x.guid == _guid);
        if (_data == null)
        {
            _data = new LevelObjectData
            {
                guid = _guid,
                dataList = new List<LevelObjectDataWrapper>()
            };
            _dataService.GameData.LevelObjectsDatabase.Objects.Add(_data);
        }
    }

    private void InjectData()
    {
        if (string.IsNullOrEmpty(_guid))
        {
            return;
        }

        foreach (ILevelObjectDataUser user in _dataUsers)
        {
            LevelObjectDataWrapper wrapper = _data.dataList.Find(x => x.type == user.DataType.FullName);

            if (wrapper != null)
            {
                object data = JsonUtility.FromJson(wrapper.json, user.DataType);
                user.InjectData(data);
            }
        }
    }

    private void SaveData()
    {
        if (string.IsNullOrEmpty(_guid))
        {
            return;
        }

        foreach (ILevelObjectDataUser user in _dataUsers)
        {
            LevelObjectDataWrapper wrapper = _data.dataList.
                Find(x => x.type == user.DataType.FullName);

            if (wrapper == null)
            {
                wrapper = new LevelObjectDataWrapper
                {
                    type = user.DataType.FullName,
                    json = string.Empty
                };
                _data.dataList.Add(wrapper);
            }

            wrapper.json = JsonUtility.ToJson(user.ReadData(), false);
        }
    }
}
