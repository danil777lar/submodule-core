using System;
using System.Collections.Generic;
using System.Linq;
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

        public virtual string TaskId => name;
        public virtual string TaskName => taskNameKey;
        public virtual string TaskDescription => taskDescriptionKey;
        public virtual Sprite Icon => icon;
        
        public virtual bool IsAvailable => true;
        public virtual TaskStatusType Status => GetData().Status;
        

        public abstract Processor CreateProcessor();

        public virtual void ClearTaskData()
        {
            DIContainer.GetService<IDataService>().GameData.ClearTaskData(TaskId);
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
            return DIContainer.GetService<IDataService>().GameData.GetTaskData(TaskId);
        }

        public abstract class Processor
        {
            protected bool _destroyed;
            
            private TaskConfig _config;
            private IDataService _dataService;

            protected TaskData Data => _config.GetData();
            protected abstract TaskStep RootStep { get; }

            public event Action<TaskStatusType> EventStatusChanged;

            public Processor(TaskConfig config)
            {
                _config = config;

                _dataService = DIContainer.GetService<IDataService>();
                _dataService.EventPreSave += OnSave;

                CreateTree();

                if (!Data.Inited)
                {
                    Data.Inited = true;
                    TaskStepData[] stepsData = RootStep.ReadData();
                    Data.StepsData = stepsData;
                }

                RootStep.InjectData(Data.StepsData.ToArray());
                RootStep.Init();
            }

            public void Start()
            {
                if (Data.Status == TaskStatusType.NotStarted)
                {
                    ChangeStatus(TaskStatusType.Started);
                    RootStep.Start();
                }
            }

            public void Destroy()
            {
                if (!_destroyed)
                {
                    if (_dataService != null)
                    {
                        _dataService.EventPreSave -= OnSave;
                    }

                    _destroyed = true;
                    RootStep.Destroy();
                }
            }

            public bool IsChildOf(TaskConfig config)
            {
                return config == _config;
            }

            private void OnSave()
            {
                Data.StepsData = RootStep.ReadData();
            }

            private void ChangeStatus(TaskStatusType status)
            {
                Data.Status = status;
                EventStatusChanged?.Invoke(status);
            }

            protected abstract void CreateTree();
        }
    }
}
