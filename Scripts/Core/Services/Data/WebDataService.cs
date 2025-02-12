using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

[BindService(typeof(DataService))]
public class WebDataService : DataService
{
    [SerializeField] private string getDataMethod = "get_data";
    [SerializeField] private string setDataMethod = "set_data";
    
    [InjectService] private BackendBridgeService _backendBridgeService;
    
    private string _lastLog = "";
    
    public string UserId { get; private set; }
    public string UserFirstName { get; private set; }

    public void SetUserId(string id)
    {
        UserId = id;
        Load();
    }

    public void SetFirstName(string nick)
    {
        UserFirstName = nick;
    }

    public override void DeleteSave()
    {
        
    }

    public override void Save()
    {
        if (UserId != null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("user_id", UserId);
            data.Add("user_data", JsonUtility.ToJson(_data));

            _backendBridgeService.SendRequest(setDataMethod, data, response =>
            {
                Debug.Log($"Web Data Service: Save Data result => {response["result"]}");
            });
        }
    }

    protected override void Load()
    {
        if (UserId != null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("user_id", UserId);

            _backendBridgeService.SendRequest(getDataMethod, data, response =>
            {
                if (response.TryGetValue("user_data", out string jsonData))
                {
                    _data = JsonUtility.FromJson<GameData>(jsonData);
                }
                Debug.Log($"Web Data Service: Load Data result => {response["result"]}");
            });
        }
    }

    private IEnumerator SendRequest(UnityWebRequest request, Action<UnityWebRequest> onComplete)
    {
        yield return request.SendWebRequest();
        
        onComplete?.Invoke(request);
        request.Dispose();
    }
}
