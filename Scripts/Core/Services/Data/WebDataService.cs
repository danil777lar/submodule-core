using System.Collections;
using System.Collections.Generic;
using Larje.Core;
using Larje.Core.Services;
using UnityEngine;

[BindService(typeof(DataService))]
public class WebDataService : DataService
{
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
    }

    public void SetFirstName(string nick)
    {
        UserFirstName = nick;
    }
}
