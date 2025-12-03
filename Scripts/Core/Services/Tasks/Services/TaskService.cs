using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    public abstract class TaskService : Service, ITasksService
    {
        public abstract override void Init();

        public abstract IReadOnlyCollection<TaskConfig> Tasks { get; }
        
        public event Action<TaskConfig, TaskStatusType> EventTaskStatusChanged;
        public event Action<TaskConfig> EventTaskRewardGiven;
        
        protected virtual TaskConfig.Processor CreateProcessor(TaskConfig config)
        {
            TaskConfig.Processor processor = config.CreateProcessor();
            processor.EventStatusChanged += (status) => OnTaskStatusChanged(config, status);

            return processor;
        }
        
        protected virtual void OnTaskStatusChanged(TaskConfig config, TaskStatusType taskStatusType)
        {
            EventTaskStatusChanged?.Invoke(config, taskStatusType);
        }
        
        protected virtual void OnTaskRewardGiven(TaskConfig config)
        {
            EventTaskRewardGiven?.Invoke(config);
        }
    }
}
