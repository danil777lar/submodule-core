using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleBodyLog : MonoBehaviour
    {
        [SerializeField] private LogSource logSource;
        [Space] [SerializeField] private Transform logsRoot;
        [SerializeField] private DebugConsoleLog logPrefab;

        private int _instantiatedLogs;
        private DebugConsoleService _debugConsoleService;

        private void Awake()
        {
            _debugConsoleService = GetComponentInParent<DebugConsoleService>();
        }

        private void Update()
        {
            UpdateLogs();
        }

        private void UpdateLogs()
        {
            List<DebugConsoleService.Log> logs = GetLogs();
            while (logs.Count > _instantiatedLogs)
            {
                DrawLog(logs.ElementAt(_instantiatedLogs));
            }
        }

        private void DrawLog(DebugConsoleService.Log log)
        {
            _instantiatedLogs++;
            Instantiate(logPrefab, logsRoot).Init(log, 36, RebuildLogsLayout);
        }

        private void RebuildLogsLayout()
        {
            StartCoroutine(RebuildLogsLayoutCo());
        }

        private IEnumerator RebuildLogsLayoutCo()
        {
            RectTransform rectTransform = logsRoot as RectTransform;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            yield return null;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private List<DebugConsoleService.Log> GetLogs()
        {
            return logSource switch
            {
                LogSource.Unity => _debugConsoleService.UnityLogs.ToList(),
                LogSource.JS => _debugConsoleService.JSLogs.ToList(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private enum LogSource
        {
            Unity,
            JS
        }
    }
}