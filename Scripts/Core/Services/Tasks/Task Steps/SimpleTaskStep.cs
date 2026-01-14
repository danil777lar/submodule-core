using System;
using UnityEngine;

public abstract class SimpleTaskStep : TaskStep
{
    private TaskStep _next;

    public override int CurrentIndex => IsCompleted && _next != null ? _next.CurrentIndex : RawIndex;

    protected override TaskStep Next => _next;
    protected override TaskStep[] Children => _next == null ? new TaskStep[]{} : new TaskStep[] { _next };

    public SimpleTaskStep(int index, string id) : base(index, id)
    {
        
    }

    public SimpleTaskStep SetNext(TaskStep next)
    {
        _next = next;
        return this;
    }
}
