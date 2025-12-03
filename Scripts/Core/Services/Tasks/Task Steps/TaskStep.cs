using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TaskStep
{
    protected abstract bool IsCompleted { get; }
    protected abstract Data DataContent { get; set; }
    protected abstract Type DataType { get; }
    protected abstract TaskStep Next { get; }
    protected abstract TaskStep[] Children { get; }

    private string _id;

    public event Action EventCompleted;

    public TaskStep(string id)
    {
        _id = id;
    }

    public TaskStepData[] ReadData()
    {
        TaskStepData wrapper = new TaskStepData();
        wrapper.Id = _id;
        wrapper.Type = DataType.FullName;
        wrapper.Json = JsonUtility.ToJson(DataContent);

        List<TaskStepData> wrappers = new List<TaskStepData>();
        wrappers.Add(wrapper);
        foreach (TaskStep child in Children)
        {
            wrappers.AddRange(child.ReadData());
        }

        return wrappers.ToArray();
    }

    public void InjectData(TaskStepData[] stepsData)
    {
        List<TaskStepData> dataList = stepsData.ToList();
        TaskStepData wrapper = dataList.Find(x => x.Id == _id);
        dataList.Remove(wrapper);

        Type t = Type.GetType(wrapper.Type);
        Data data = JsonUtility.FromJson(wrapper.Json, t) as Data;
        DataContent = data;
        
        foreach (TaskStep child in Children)
        {
            child.InjectData(stepsData);
        }
    }

    public void Start()
    {
    }

    protected abstract class Data
    {
        public string Id;
    }
}
