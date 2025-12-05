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
    public bool Inited;
    public bool IsTrackinig;
    public string TaskType;
    public TaskStatusType Status;
    public TaskStepData[] StepsData;
}

[Serializable]
public class TaskStepData
{
    public string Id;
    public string Type;
    public string Json;
}
