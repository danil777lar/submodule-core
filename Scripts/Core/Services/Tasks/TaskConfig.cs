using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        [SerializeField] protected bool startOnInitialize;
        
        [Header("Reward")] 
        [SerializeField] protected List<TaskRewardConfig> rewards;

        public virtual string Type => name;
        public virtual string TaskName => taskNameKey;
        public virtual string TaskDescription => taskDescriptionKey;
        public virtual Sprite Icon => icon;
        
        public virtual bool IsAvailable => true;
        public virtual bool Initialized => GetData().Initialized;
        public virtual bool RewardGiven => GetData().RewardGiven;
        public virtual TaskStatusType Status => GetData().Status;
        

        public abstract Processor CreateProcessor();

        public virtual void ClearTaskData()
        {
            DIContainer.GetService<IDataService>().GameData.ClearTaskData(Type);
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
            return DIContainer.GetService<IDataService>().GameData.GetTaskData(Type);
        }

        public abstract class Processor
        {
            protected bool _destroyed;
            
            private TaskConfig _config;

            protected TaskData Data => _config.GetData();

            public event Action<TaskStatusType> EventStatusChanged;
            public event Action EventRewarded;

            public Processor(TaskConfig config)
            {
                _config = config;
            }

            public bool IsChildOf(TaskConfig config)
            {
                return config == _config;
            }

            public void Initialize()
            {
                OnInitialize();
                
                if (!Data.Initialized)
                {
                    InitializeData();
                }
                
                if (_config.startOnInitialize)
                {
                    Start();
                }
            }

            public void Start()
            {
                if (Data.Status == TaskStatusType.NotStarted)
                {
                    ChangeStatus(TaskStatusType.Started);
                }
            }

            public void GiveReward(int multiplier = 1)
            {
                if ((Data.Status == TaskStatusType.Completed || Data.Status == TaskStatusType.Failed) && !Data.RewardGiven)
                {
                    _config.rewards.ForEach(x => x.GiveReward(multiplier));
                    EventRewarded?.Invoke();
                }
            }

            public void Destroy()
            {
                if (!_destroyed)
                {
                    _destroyed = true;
                    OnDestroy();
                }
            }
            
            protected void InitializeData()
            {
                Data.Initialized = true;
                OnInitializeData();
            }

            protected void Complete()
            {
                if (Data.Status == TaskStatusType.Started)
                {
                    ChangeStatus(TaskStatusType.Completed);
                }
            }

            protected void Fail()
            {
                if (Data.Status == TaskStatusType.Started)
                {
                    ChangeStatus(TaskStatusType.Failed);
                }
            }

            private void ChangeStatus(TaskStatusType status)
            {
                Data.Status = status;
                
                switch (status)
                {
                    case TaskStatusType.Started:
                        OnStarted();
                        break;
                    case TaskStatusType.Completed:
                        OnCompleted();
                        break;
                    case TaskStatusType.Failed:
                        OnFailed();
                        break;
                }
                
                EventStatusChanged?.Invoke(status);
            }

            protected virtual void OnInitialize() { }
            protected virtual void OnInitializeData() { }
            protected virtual void OnStarted() { }
            protected virtual void OnCompleted() { }
            protected virtual void OnFailed() { }
            protected virtual void OnDestroy() { }
        }
    }
}