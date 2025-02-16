using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[BindService(typeof(DataService))]
public class WebDataService : DataService
{
    [Space(20f)] 
    [SerializeField] private string aesKey;
    [SerializeField] private string aesIV;
    [Space]
    [SerializeField] private string getDataMethod = "get_data";
    [SerializeField] private string setDataMethod = "set_data";

    [InjectService] private BackendBridgeService _backend;
    
    private bool _loaded;

    public string UserId { get; private set; } = "0";
    public string UserFirstName { get; private set; }

    public void SetUserId(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            UserId = id;
            Load();
        }
    }

    public void SetFirstName(string nick)
    {
        UserFirstName = nick;
    }

    public float GetLoadPercent()
    {
        return _loaded ? 1f : 0f;
    }

    public override void DeleteSave()
    {
    }

    public override void Save()
    {
        if (UserId != null)
        {
            Debug.Log("Web Data Service: Try Save!");
            
            string json = JsonUtility.ToJson(_data);
            
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("user_id", UserId);
            data.Add("user_data", AESUtility.Encrypt(json, aesKey, aesIV));

            _backend.SendRequest(setDataMethod, data, response =>
            {
                if (response["result"] == "success")
                {
                    Debug.Log("Web Data Service: Save Complete!");
                    Debug.Log($"SavedData:\n{JToken.Parse(json).ToString(Formatting.Indented)}");
                }
                else
                {
                    Debug.Log("Web Data Service: Save Error");
                }
            });
        }
        else
        {
            Debug.Log("Web Data Service: Save Error: User ID is null");
        }
    }

    protected override void Load()
    {
        if (UserId != null)
        {
            Debug.Log("Web Data Service: Try Load!");
            
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("user_id", UserId);

            _backend.SendRequest(setDataMethod, data, response =>
            {
                if (response["result"] == "success")
                {
                    string rawText = response["user_data"];
                    if (string.IsNullOrEmpty(rawText) || string.IsNullOrWhiteSpace(rawText))
                    {
                        _data = _defaultProfile.profileData;
                    }
                    else
                    {
                        string json = AESUtility.Decrypt(rawText, aesKey, aesIV);
                        _data = JsonUtility.FromJson<GameData>(json);
                    }

                    OnLoaded();
                    Debug.Log("Web Data Service: Load Complete!");
                }
                else
                {
                    Debug.Log("Web Data Service: Load Error");
                }
            });
        }
        else
        {
            Debug.Log("Web Data Service: Load Error: User ID is null");
        }
    }

    private void OnLoaded()
    {
        if (!_loaded)
        {
            _data.IternalData.SessionNum++;
            Save();
        }
        _loaded = true;
    }
    
    [ContextMenu("Generate Key")]
    private void GenerateKey()
    {
        Debug.Log(AESUtility.GenerateKey());
    }
    
    [ContextMenu("Generate IV")]
    private void GenerateIV()
    {
        Debug.Log(AESUtility.GenerateIV());
    }
}
