using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TaskStep
{
    protected abstract Data DataContent { get; set; }
    protected abstract Type DataType { get; }
    protected abstract TaskStep Next { get; }
    protected abstract TaskStep[] Children { get; }

    protected int RawIndex => _rawIndex;
    protected string Id => _id;
    protected bool IsStarted => DataContent.Started;
    protected bool IsCompleted => DataContent.Completed;

    private int _rawIndex;
    private string _id;

    public abstract int CurrentIndex { get; }

    public event Action EventTreeCompleted;

    public TaskStep(int index, string id)
    {
        _rawIndex = index;
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
        if (data == null)
        {
            data = Activator.CreateInstance(t) as Data;
        }
        DataContent = data;
        
        foreach (TaskStep child in Children)
        {
            child.InjectData(stepsData);
        }
    }

    public void Init()
    {
        foreach (TaskStep child in Children)
        {
            child.Init();
            child.EventTreeCompleted += OnTreeCompleted;
        }

        if (DataContent.Started && !DataContent.Completed)
        {
            Start();
        }
    }

    public void Start()
    {
        DataContent.Started = true;
        OnStart();
    }

    public void Destroy()
    {
        OnDestroy();
        foreach (TaskStep child in Children)
        {
            child.Destroy();
        }
    }

    public Func<Vector3>[] GetTaskPositions()
    {
        List<Func<Vector3>> positions = new List<Func<Vector3>>();
        if (!IsCompleted)
        {
            positions.AddRange(GetStepPositions());
        }
        foreach (TaskStep child in Children)
        {
            positions.AddRange(child.GetTaskPositions());
        }

        return positions.ToArray();
    }

    protected void Complete()
    {
        if (!DataContent.Completed)
        {
            DataContent.Completed = true;
            if (Next != null)
            {
                Next.Start();
            }
            else
            {
                EventTreeCompleted?.Invoke();
            }
        }
    }

    protected virtual void OnStart()
    {
    }

    protected virtual void OnDestroy()
    {
    }

    protected virtual Func<Vector3>[] GetStepPositions()
    {
        return new Func<Vector3>[0];
    }

    private void OnTreeCompleted()
    {
        EventTreeCompleted?.Invoke();
    }

    protected abstract class Data
    {
        public bool Started;
        public bool Completed;
    }
}
