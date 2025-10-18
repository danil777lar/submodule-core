using System;
using System.Collections.Generic;

namespace Larje.Core.Services
{
    public partial class GameData
    {
        public List<TaskData> Tasks;
        
        public TaskData GetTaskData(string taskType)
        {
            Tasks ??= new List<TaskData>();
            
            TaskData taskData = Tasks.Find(x => x.TaskType == taskType);
            if (taskData == null)
            {
                taskData = new TaskData
                {
                    TaskType = taskType
                };
                Tasks.Add(taskData);
            }
            
            return taskData;
        }
        
        public void ClearTaskData(string taskType)
        {
            TaskData taskData = Tasks.Find(x => x.TaskType == taskType);
            if (taskData != null)
            {
                Tasks.Remove(taskData);
            }
        }
    }    
}

[Serializable]
public class TaskData
{
    public string TaskType;

    public bool Initialized;
    public bool RewardGiven;
    public TaskStatusType Status;
    
    public int MaxProgress;
    public int CurrentProgress;
}
