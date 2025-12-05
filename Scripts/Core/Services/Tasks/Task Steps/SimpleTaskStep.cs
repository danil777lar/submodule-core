using System;
using UnityEngine;

public abstract class SimpleTaskStep : TaskStep
{
    private TaskStep _next;

    protected override bool IsCompleted => false;
    protected override TaskStep Next => _next;
    protected override TaskStep[] Children => _next == null ? new TaskStep[]{} : new TaskStep[] { _next };

    public SimpleTaskStep(string id) : base(id)
    {
        
    }

    public SimpleTaskStep SetNext(TaskStep next)
    {
        _next = next;
        return this;
    }
}
