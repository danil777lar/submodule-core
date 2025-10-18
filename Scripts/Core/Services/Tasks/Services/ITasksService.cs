using System;
using System.Collections.Generic;

namespace Larje.Core.Services
{
    public interface ITasksService
    {
        public IReadOnlyCollection<TaskConfig> Tasks { get; }

        public event Action<TaskConfig, TaskStatusType> EventTaskStatusChanged;
        public event Action<TaskConfig> EventTaskRewardGiven;
    }
}