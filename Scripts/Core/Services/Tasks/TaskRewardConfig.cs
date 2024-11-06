using UnityEngine;

namespace Larje.Core.Services
{
    public abstract class TaskRewardConfig : ScriptableObject
    {
        public abstract void GiveReward(int multiplier);
    }
}