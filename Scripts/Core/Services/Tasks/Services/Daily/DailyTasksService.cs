using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(DailyTasksService))]
    public class DailyTasksService : TaskService
    {
        [SerializeField, Min(1)] private int tasksCount;
        [SerializeField] private List<TaskConfig> taskConfigs;

        [InjectService] private IDataService _dataService;
        
        protected List<TaskConfig.Processor> _processors;

        public virtual DateTime LastReset => DateTime.Parse(_dataService.GameData.DailyTasksServiceData.LastDailyTaskReset);
        public virtual DateTime NextReset => DateTime.Today.AddDays(1);
        public override IReadOnlyCollection<TaskConfig> Tasks => taskConfigs.FindAll(x => x.Initialized);


        public override void Init()
        {
            if (NeedToResetTasks())
            {
                CreateTasks();
            }
            else
            {
                LoadProcessors();
            }
        }

        protected virtual void CreateTasks()
        {
            ClearTasks();

            _dataService.GameData.DailyTasksServiceData.LastDailyTaskReset = DateTime.Now.ToString();

            List<TaskConfig> createdTasks = new List<TaskConfig>();
            for (int i = 0; i < tasksCount; i++)
            {
                List<TaskConfig> availableTasks = taskConfigs.FindAll(x => 
                    x.IsAvailable && !createdTasks.Contains(x));
                
                TaskConfig task = availableTasks[UnityEngine.Random.Range(0, availableTasks.Count)];
                _processors.Add(CreateProcessor(task));
                createdTasks.Add(task);
            }
        }

        protected virtual void LoadProcessors()
        {
            _processors = new List<TaskConfig.Processor>();
            foreach (TaskConfig config in Tasks)
            {
                _processors.Add(CreateProcessor(config));
            }
        }

        protected virtual void ClearTasks()
        {
            taskConfigs.ForEach(x => x.ClearTaskData());
            _processors = new List<TaskConfig.Processor>();
        }

        protected virtual bool NeedToResetTasks()
        {
            string lastResetString = _dataService.GameData.DailyTasksServiceData.LastDailyTaskReset;
            if (Tasks.Count > 0 && DateTime.TryParse(lastResetString, out DateTime lastReset))
            {
                return TimeSpan.FromTicks(lastReset.Ticks).Days != TimeSpan.FromTicks(DateTime.Now.Ticks).Days;
            }

            return true;
        }
    }
}