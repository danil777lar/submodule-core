using System;
using Larje.Core;
using Larje.Core.Services;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

public abstract class TaskConfig : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string taskNameKey;
    [SerializeField] private string taskDescriptionKey;
    [SerializeField] private Sprite icon;
    [Header("Reward")] 
    [SerializeField] private UltEvent rewardEvent;

    public string Type => name;
    public string TaskName => taskNameKey;
    public string TaskDescription => taskDescriptionKey.Replace("{0}", $" {TaskData.MaxProgress.ToString()} ");
    public Sprite Icon => icon;
    
    public virtual bool IsAvailable => true;
    public TaskData TaskData => DIContainer.GetService<DataService>().Data.GetTaskData(Type);

    public event Action EventRewardGiven;
    
    
    public abstract Processor CreateProcessor();

    public void ClearTaskData()
    {
        DIContainer.GetService<DataService>().Data.ClearTaskData(Type);
    }

    public void GiveReward()
    {
        if (!TaskData.RewardGiven)
        {
            TaskData.RewardGiven = true;
            rewardEvent.Invoke();
            EventRewardGiven?.Invoke();
        }
    }

    [ContextMenu("Set Localization Keys")]
    private void SetLocalizationKeys()
    {
        string formattedName = name.Replace(" ", "_").ToLower();
        taskNameKey = $"{formattedName}_name";
        taskDescriptionKey = $"{formattedName}_desc";

        GUIUtility.systemCopyBuffer = GetLocalizationBase();
    }
    
    private string GetLocalizationBase()
    {
        string result = "";
        result += $"<{taskNameKey} EN=\"Name {name}\" RU=\"...\" />\n";
        result += $"<{taskDescriptionKey} EN=\"Desc {name}\" RU=\"...\" />";
        return result;
    }

    public abstract class Processor
    {
        private TaskConfig _config;
     
        protected TaskData TaskData => _config.TaskData;
        
        public event Action EventCompleted;

        public Processor(TaskConfig config)
        {
            _config = config;
        }

        public virtual void Initialize()
        {
            if (!TaskData.Initialized)
            {
                InitializeData();   
            }
        }

        public virtual void Destroy()
        {
            
        }

        protected virtual void InitializeData()
        {
            _config.TaskData.Initialized = true;
        }

        protected void Complete()
        {
            if (!_config.TaskData.Complete)
            {
                _config.TaskData.Complete = true;
                EventCompleted?.Invoke();
            }
        }
    }
}
