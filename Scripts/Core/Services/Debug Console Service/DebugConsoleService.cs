using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Larje.Core;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleService : Service
{
    [SerializeField] private bool enableConsole;
    [SerializeField] private bool showConsoleButton;
    [Space]
    [SerializeField] private GameObject consoleRoot;
    [SerializeField] private GameObject buttonRoot;
    
    [InjectService] private InputService _inputService;

    private bool _consoleOpened;
    private List<Log> _unityLogs = new List<Log>();
    private List<Log> _jsLogs = new List<Log>();

    public IReadOnlyCollection<Log> UnityLogs => _unityLogs;
    public IReadOnlyCollection<Log> JSLogs => _jsLogs;
    
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void RegisterLogCallback(string className, string methodName);
    #endif
    
    public override void Init()
    {
        if (enableConsole)
        {
            CloseConsole();
            
            Application.logMessageReceived += HandleUnityLog;
            #if UNITY_WEBGL && !UNITY_EDITOR
            RegisterLogCallback(gameObject.name, "HandleJSLog");
            #endif
            
            _inputService.UIDebug.performed += ctx => ToggleConsole();

            InputService.Condition condition = new InputService.Condition(
                () => !_consoleOpened, InputService.ConditionOperation.And, "Debug Console");
            _inputService.AddCondition<InputSystem_Actions.PlayerActions>(condition);
        }
        else
        {
            consoleRoot.SetActive(false);
            buttonRoot.SetActive(false);
        }
    }

    public void OpenConsole()
    {
        _consoleOpened = true;
        consoleRoot.SetActive(true);
        buttonRoot.SetActive(false);
    }

    public void CloseConsole()
    {
        _consoleOpened = false;
        consoleRoot.SetActive(false);
        buttonRoot.SetActive(true && showConsoleButton);
    }
    
    public void ToggleConsole()
    {
        if (_consoleOpened)
        {
            CloseConsole();
        }
        else
        {
            OpenConsole();
        }
    }
    
    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        Log log = new Log();
        log.text = logString;
        log.stackTrace = stackTrace;
        log.type = type;
        
        _unityLogs.Add(log);
    }
    
    public void HandleJSLog(string message)
    {
        Log log = new Log();
        
        log.text = message;
        log.stackTrace = string.Empty;
        log.type = LogType.Log;
        
        _jsLogs.Add(log);
    }

    [Serializable]
    public class Log
    {
        public string text;
        public string stackTrace;
        public LogType type;
    }
}
