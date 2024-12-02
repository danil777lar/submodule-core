using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Networking;

[BindService(typeof(DataService))]
public class WebDataService : DataService
{
    [SerializeField] private string getDataUrl = "get_data.php";
    [SerializeField] private string setDataUrl = "set_data.php";
    
    private string _lastLog = "";
    
    public string URL { get; private set; }
    public string UserId { get; private set; }
    public string UserFirstName { get; private set; }
    
    public void SetLink(string link)
    {
        URL = link;
    }

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
            Dictionary<string, string> users = new Dictionary<string, string>();
            users.Add("user_id", UserId);
            users.Add("user_data", JsonUtility.ToJson(_data));

            UnityWebRequest request = UnityWebRequest.Post(URL + setDataUrl, users);
            StartCoroutine(SendRequest(request, (result) =>
            {
                if (result.result == UnityWebRequest.Result.Success)
                {
                    Log("Web Data Service: Save Complete!", false);
                }
                else
                {
                    Log("Web Data Service: Save Error - " + result.error, true);
                }
            }));
        }
        else
        {
            Log("Web Data Service: Save Error: User ID is null", true);
        }
    }

    protected override void Load()
    {
        if (UserId != null)
        {
            Dictionary<string, string> users = new Dictionary<string, string>();
            users.Add("user_id", UserId);

            UnityWebRequest request = UnityWebRequest.Post(URL + getDataUrl, users);
            StartCoroutine(SendRequest(request, (result) => 
            {
                if (result.result == UnityWebRequest.Result.Success)
                {
                    string json = result.downloadHandler.text;
                    _data = JsonUtility.FromJson<GameData>(json);
                    
                    Log("Web Data Service: Load Complete!", false);
                }
                else
                {
                    Log("Web Data Service: Load Error - " + result.error, true);
                }
            }));
        }
        else
        {
            Log("Web Data Service: Load Error: User ID is null", true);
        }
    }

    private void Update()
    {
        MMDebug.DebugOnScreen(_lastLog);
    }
    
    private void Log(string text, bool error)
    {
        _lastLog = $"[{DateTime.Now.ToString("HH:mm:ss")}] {text}";
        if (error)
        {
            Debug.LogWarning(text);
        }
        else
        {
            Debug.Log(text);
        }
    }

    private IEnumerator SendRequest(UnityWebRequest request, Action<UnityWebRequest> onComplete)
    {
        yield return request.SendWebRequest();
        
        onComplete?.Invoke(request);
        request.Dispose();
    }
}
