using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.Tools;
using UnityEngine;

[BindService(typeof(DailyTasksService))]
public class DailyTasksService : Service, ITasksService
{
    [SerializeField, Min(1)] private int tasksCount;
    [SerializeField] private List<TaskConfig> taskConfigs;
    
    [InjectService] private DataService _dataService;
    [InjectService] private UIService _uiService;
    
    private bool _needToShowPopup;
    private List<TaskConfig.Processor> _processors;
    
    public DateTime LastReset => DateTime.Parse(_dataService.Data.DailyTasksServiceData.LastDailyTaskReset);
    public DateTime NextReset => DateTime.Today.AddDays(1);
    public IReadOnlyCollection<TaskConfig> Tasks => taskConfigs.FindAll(x => x.TaskData.Initialized);
    public event Action EventRewardGiven;

    public override void Init()
    {
        taskConfigs.ForEach(x => x.EventRewardGiven += () => EventRewardGiven?.Invoke());
        
        if (NeedToResetTasks())
        {
            CreateTasks();
        }
        else
        {
            LoadProcessors();
        }
    }

    private void LoadProcessors()
    {
        _processors = new List<TaskConfig.Processor>();
        foreach (TaskConfig config in Tasks)
        {
            TaskConfig.Processor processor = config.CreateProcessor();
            processor.Initialize();
            processor.EventCompleted += () => OnTaskCompleted(config);
            _processors.Add(processor);
        }
    }

    private void CreateTasks()
    {
        ClearTasks();   
        
        _dataService.Data.DailyTasksServiceData.LastDailyTaskReset = DateTime.Now.ToString();
        
        List<TaskConfig> tasksToCreate = new List<TaskConfig>();
        for (int i = 0; i < tasksCount; i++)
        {
            List<TaskConfig> availableTasks = taskConfigs.FindAll(x => x.IsAvailable && !tasksToCreate.Contains(x));
            TaskConfig task = availableTasks[UnityEngine.Random.Range(0, availableTasks.Count)];
            
            TaskConfig.Processor processor = task.CreateProcessor();
            processor.Initialize();
            processor.EventCompleted += () => OnTaskCompleted(task);
            _processors.Add(processor);
            
            tasksToCreate.Add(task);
        }
    }
    
    private void ClearTasks()
    {
        taskConfigs.ForEach(x => x.ClearTaskData());
        _processors = new List<TaskConfig.Processor>();
    }
    
    private bool NeedToResetTasks()
    {
        string lastResetString = _dataService.Data.DailyTasksServiceData.LastDailyTaskReset; 
        if (Tasks.Count > 0 && DateTime.TryParse(lastResetString, out DateTime lastReset))
        {
            return TimeSpan.FromTicks(lastReset.Ticks).Days != TimeSpan.FromTicks(DateTime.Now.Ticks).Days;
        }
        
        return true;
    }
    
    private void OnTaskCompleted(TaskConfig config)
    {
        Debug.Log($"Complete task {config.Type}");
        if (!_needToShowPopup)
        {
            this.DOKill();
            DOVirtual.DelayedCall(1f, () => _needToShowPopup = true)
                .SetTarget(this);
        }
    }
}
