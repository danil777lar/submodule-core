using System;
using System.Collections.Generic;

namespace Larje.Core.Services
{
    public interface ITasksService
    {
        public IReadOnlyCollection<TaskConfig> Tasks { get; }

        public event Action EventTaskActivated;
        public event Action EventTaskCompleted;
        public event Action EventTaskRewardGiven;
    }
}