using UnityEngine;

namespace Larje.Core.Services
{
    public abstract class TaskRewardConfig : ScriptableObject
    {
        public virtual int Amount => 0;
        public abstract void GiveReward(int multiplier);
    }
}