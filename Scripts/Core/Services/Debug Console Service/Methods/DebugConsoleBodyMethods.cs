using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoreMountains.Tools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleBodyMethods : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private DebugConsoleMethodPanel methodPanelPrefab;
    [SerializeField] private Button groupPrefab;

    private List<DebugConsoleMethodPanel> _panels;
    private Dictionary<string, Button> _methodHeaders;
    private Dictionary<string, List<MethodInfo>> _methodsByGroup;
    
    private void OnEnable()
    {
        Rebuild();
    }
    
    private void Rebuild()
    {
        content.MMDestroyAllChildren();
        List<MethodInfo> methods = GetMethods();
        
        _panels = new List<DebugConsoleMethodPanel>();
        _methodHeaders = new Dictionary<string, Button>();
        _methodsByGroup = new Dictionary<string, List<MethodInfo>>();
        
        foreach (MethodInfo method in methods)
        {
            string groupName = method.HasAttribute(typeof(MethodGroupAttribute)) ? 
                method.GetAttribute<MethodGroupAttribute>().GroupName : "Ungrouped";
            if (!_methodsByGroup.ContainsKey(groupName))
            {
                _methodsByGroup.Add(groupName, new List<MethodInfo>());
            }
            _methodsByGroup[groupName].Add(method);
        }
        
        foreach (KeyValuePair<string, List<MethodInfo>> group in _methodsByGroup)
        {
            Button groupHeader = Instantiate(groupPrefab, content);
            groupHeader.onClick.AddListener(() => SetActiveGroup(group.Key));
            groupHeader.GetComponentInChildren<TextMeshProUGUI>().text = group.Key;
            _methodHeaders.Add(group.Key, groupHeader);

            foreach (MethodInfo method in group.Value)
            {
                DebugConsoleMethodPanel methodPanel = Instantiate(methodPanelPrefab, content);
                methodPanel.Init(method);
                methodPanel.gameObject.SetActive(false);
                _panels.Add(methodPanel);
            }
        }
        
        SetActiveGroup(_methodsByGroup.Keys.First());
    }

    private void SetActiveGroup(string group)
    {
        foreach (KeyValuePair<string, List<MethodInfo>> groups in _methodsByGroup)
        {
            foreach (MethodInfo method in groups.Value)
            {
                _panels.Find(x => x.MethodInfo == method)
                    .gameObject.SetActive(groups.Key == group);
            }
        }
        
        foreach (KeyValuePair<string, Button> header in _methodHeaders)
        {
            header.Value.GetComponent<Image>().color = header.Key == group ? Color.white : Color.white * 0.6f;
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
