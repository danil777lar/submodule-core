using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

public class GameEventTaskStatusChanged : GameEvent
{
    public readonly TaskConfig TaskConfig;
    public readonly TaskStatusType TaskStatus;

    public override bool IsValid => TaskConfig != null;

    public GameEventTaskStatusChanged(TaskConfig taskConfig, TaskStatusType taskStatus, string source) : base(source)
    {
        TaskConfig = taskConfig;
        TaskStatus = taskStatus;
    }
}
