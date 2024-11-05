using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITasksService
{
    public IReadOnlyCollection<TaskConfig> Tasks { get; }
    
    public event Action EventRewardGiven;
}
