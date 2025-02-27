using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoreMountains.Tools;
using UnityEngine;

public class DebugConsoleBodyMethods : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private DebugConsoleMethodPanel methodPanelPrefab;

    private void OnEnable()
    {
        Rebuild();
    }
    
    private void Rebuild()
    {
        content.MMDestroyAllChildren();
        foreach (MethodInfo method in GetMethods())
        {
            DebugConsoleMethodPanel methodPanel = Instantiate(methodPanelPrefab, content);
            methodPanel.Init(method);
        }
    }
    
    private List<MethodInfo> GetMethods()
    {
        List<MethodInfo> methods = new List<MethodInfo>();
        methods = typeof(DebugConsoleMethods)
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ToList();
        return methods;
    }
}
