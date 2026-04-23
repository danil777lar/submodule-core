using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Larje.Core.Services
{
    public partial class GameData 
    {
        public DailyTasksServiceData DailyTasksServiceData;
    }
}

[Serializable]
public class DailyTasksServiceData
{
    public string LastDailyTaskReset;
    public List<string> SelectedTaskIds = new List<string>();
    public List<string> ClaimedTaskIds = new List<string>();
}
