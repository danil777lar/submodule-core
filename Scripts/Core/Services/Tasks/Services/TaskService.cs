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
        
        public event Action EventTaskActivated;
        public event Action EventTaskCompleted;
        public event Action EventTaskRewardGiven;
        
        protected virtual TaskConfig.Processor CreateProcessor(TaskConfig config)
        {
            TaskConfig.Processor processor = config.CreateProcessor();
            
            processor.EventActivated += () => OnTaskActivated(config);
            processor.EventCompleted += () => OnTaskCompleted(config);
            processor.EventRewarded += () => OnTaskRewardGiven(config);

            processor.Initialize();
            return processor;
        }
        
        protected virtual void OnTaskActivated(TaskConfig config)
        {
            Debug.Log($"Activate task {config.Type}");
            EventTaskActivated?.Invoke();
        }

        protected virtual void OnTaskCompleted(TaskConfig config)
        {
            Debug.Log($"Complete task {config.Type}");
            EventTaskCompleted?.Invoke();
        }
        
        protected virtual void OnTaskRewardGiven(TaskConfig config)
        {
            Debug.Log($"Reward task {config.Type}");
            EventTaskRewardGiven?.Invoke();
        }
    }
}