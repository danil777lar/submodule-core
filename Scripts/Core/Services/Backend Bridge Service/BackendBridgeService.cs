using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Larje.Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

[BindService(typeof(BackendBridgeService))]
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
        Debug.Log($"BackendBridgeService | process request '{rd.method}'");
        
        rd.data.Add("app", appName);

        string jsonData = JsonConvert.SerializeObject(rd.data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest($"{_url}/{rd.method}", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        
        request.SendWebRequest().completed += operation =>
        {
            Debug.Log($"BackendBridgeService | request '{rd.method}' completed \n {request.downloadHandler.text}");
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
