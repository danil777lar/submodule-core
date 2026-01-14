using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleBodyMethods : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private DebugConsoleMethodPanel methodPanelPrefab;
        [SerializeField] private DebugConsoleMethodGroupPanel groupPrefab;

        private List<DebugConsoleMethodPanel> _panels;
        private Dictionary<string, DebugConsoleMethodGroupPanel> _methodGroups;
        private Dictionary<string, List<MethodInfo>> _methodsByGroup;

        private void OnEnable()
        {
            Rebuild();
        }

        private void Rebuild()
        {
            content.DestroyAllChildren();
            List<MethodInfo> methods = GetMethods();

            _panels = new List<DebugConsoleMethodPanel>();
            _methodGroups = new Dictionary<string, DebugConsoleMethodGroupPanel>();
            _methodsByGroup = new Dictionary<string, List<MethodInfo>>();

            foreach (MethodInfo method in methods)
            {
                string groupName = method.HasAttribute(typeof(MethodGroupAttribute))
                    ? method.GetAttribute<MethodGroupAttribute>().GroupName
                    : "Ungrouped";
                if (!_methodsByGroup.ContainsKey(groupName))
                {
                    _methodsByGroup.Add(groupName, new List<MethodInfo>());
                }

                _methodsByGroup[groupName].Add(method);
            }

            int groupIndex = 0;
            foreach (KeyValuePair<string, List<MethodInfo>> group in _methodsByGroup)
            {
                DebugConsoleMethodGroupPanel groupHeader = Instantiate(groupPrefab, content);
                groupHeader.Init(groupIndex, group.Key, () => OnGroupClicked(group.Key), OnGroupUpdate);
                _methodGroups.Add(group.Key, groupHeader);

                foreach (MethodInfo method in group.Value)
                {
                    DebugConsoleMethodPanel methodPanel = Instantiate(methodPanelPrefab);
                    methodPanel.Init(method);
                    groupHeader.AddChild(methodPanel.transform);
                    _panels.Add(methodPanel);
                }
                
                groupIndex++;
            }

            _methodGroups.First().Value.SetState(true);
        }

        private void OnGroupClicked(string groupName)
        {
            foreach (KeyValuePair<string, DebugConsoleMethodGroupPanel> group in _methodGroups)
            {
                group.Value.SetState(group.Key == groupName);
            }
        }

        private void OnGroupUpdate()
        {
            RectTransform rectTransform = content as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private List<MethodInfo> GetMethods()
        {
            List<MethodInfo> methods = new List<MethodInfo>();
            methods = typeof(DebugConsoleMethods)
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance |
                            BindingFlags.DeclaredOnly)
                .ToList();
            return methods;
        }
    }
}