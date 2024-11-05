using System;
using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Larje.Core.Services
{
    public partial class GameData
    {
        public DailyTasksRewardData dailyTasksRewardData;
    }    
}

[Serializable]
public class DailyTasksRewardData
{
    public bool RewardClaimed;
}
