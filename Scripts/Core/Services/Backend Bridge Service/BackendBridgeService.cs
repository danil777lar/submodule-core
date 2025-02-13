using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class BackendBridgeService : Service
{
    [SerializeField] private bool useOnlyHostUrl;
    [SerializeField] private string appName;
    [SerializeField] private string additionalPath;

    private string _url;
    private List<RequestData> _requests = new List<RequestData>();
    
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void GetCurrentLocation(string goName, string methodName);
    #endif
    
    public override void Init()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        GetCurrentLocation(gameObject.name, "CatchLocation");
        #endif
    }
    
    public void CatchLocation(string path)
    {
        if (useOnlyHostUrl)
        {
            Uri uri = new Uri(path);
            _url = uri.Host;
        }
        else
        {
            _url = path;
        }
        _url += additionalPath;
        
        Debug.Log($"BackendBridgeService | location catch: {_url}");
    }
    
    public void SendRequest(string method, Dictionary<string, string> data, Action<Dictionary<string, string>> callback)
    {
        RequestData rd = new RequestData
        {
            method = method,
            data = data,
            callback = callback
        };
        _requests.Add(rd);
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(_url))
        {
            while (_requests.Count > 0)
            {
                ProcessRequest(_requests[0]);
            }
        }
    }
    
    private void ProcessRequest(RequestData rd)
    {
        rd.data.Add("app_name", appName);
        
        UnityWebRequest request = UnityWebRequest.Post($"{_url}/{rd.method}", rd.data);
        request.SendWebRequest().completed += operation =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Dictionary<string, string> responseDict = 
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                rd.callback?.Invoke(responseDict);
            }
            else
            {
                Debug.LogError($"BackendBridgeService | request failed: {request.error}");
            }
        };
        
        _requests.Remove(rd);
    }

    private class RequestData
    {
        public string method;
        public Dictionary<string, string> data;
        public Action<Dictionary<string, string>> callback;
    }
}
