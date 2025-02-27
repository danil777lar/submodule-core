using System;using UnityEngine;

public class MethodGroupAttribute : Attribute
{
    public string GroupName { get; private set; }

    public MethodGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }
}
