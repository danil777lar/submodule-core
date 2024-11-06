using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    public abstract class TaskConfig : ScriptableObject
    {
        [Header("Info")] 
        [SerializeField] protected string taskNameKey;
        [SerializeField] protected string taskDescriptionKey;
        [SerializeField] protected Sprite icon;
        
        [Header("Logic")]
        [SerializeField] protected bool activateOnInitialize;
        
        [Header("Reward")] 
        [SerializeField] protected List<TaskRewardConfig> rewards;

        public virtual string Type => name;
        public virtual string TaskName => taskNameKey;
        public virtual string TaskDescription => taskDescriptionKey;
        public virtual Sprite Icon => icon;
        
        public virtual bool IsAvailable => true;
        public virtual bool Initialized => GetData().Initialized;
        public virtual bool IsActive => GetData().IsActive;
        public virtual bool Complete => GetData().Complete;
        public virtual bool RewardGiven => GetData().RewardGiven;
        

        public abstract Processor CreateProcessor();

        public virtual void ClearTaskData()
        {
            DIContainer.GetService<DataService>().Data.ClearTaskData(Type);
        }

        [ContextMenu("Set Localization Keys")]
        protected virtual void SetLocalizationKeys()
        {
            string formattedName = name.Replace(" ", "_").ToLower();
            taskNameKey = $"{formattedName}_name";
            taskDescriptionKey = $"{formattedName}_desc";

            GUIUtility.systemCopyBuffer = GetLocalizationBase();
        }

        protected virtual string GetLocalizationBase()
        {
            string result = "";
            result += $"<{taskNameKey} EN=\"Name {name}\" RU=\"...\" />\n";
            result += $"<{taskDescriptionKey} EN=\"Desc {name}\" RU=\"...\" />";
            return result;
        }

        protected virtual TaskData GetData()
        {
            return DIContainer.GetService<DataService>().Data.GetTaskData(Type);
        }

        public abstract class Processor
        {
            private TaskConfig _config;

            protected TaskData Data => _config.GetData();

            public event Action EventActivated;
            public event Action EventCompleted;
            public event Action EventRewarded;

            public Processor(TaskConfig config)
            {
                _config = config;
            }

            public virtual void Initialize()
            {
                if (!Data.Initialized)
                {
                    InitializeData();
                    if (_config.activateOnInitialize)
                    {
                        Activate();
                    }
                }
            }

            public virtual void Destroy()
            {

            }

            protected virtual void InitializeData()
            {
                Data.Initialized = true;
            }

            protected virtual void Activate()
            {
                if (!Data.IsActive)
                {
                    Data.IsActive = true;
                    EventActivated?.Invoke();
                }
            }

            protected virtual void Complete()
            {
                if (!Data.Complete)
                {
                    Data.Complete = true;
                    EventCompleted?.Invoke();
                }
            }

            protected virtual void GiveReward(int multiplier = 1)
            {
                if (!Data.Complete)
                {
                    Data.Complete = true;
                    _config.rewards.ForEach(x => x.GiveReward(multiplier));
                    EventRewarded?.Invoke();
                }
            }
        }
    }
}