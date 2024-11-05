using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[BindService(typeof(AchievementsService))]
public class AchievementsService : Service, ITasksService
{
    [SerializeField] private List<TaskConfig> achievements;

    [InjectService] private UIService _uiService;
    
    private List<TaskConfig.Processor> _processors;
    
    public IReadOnlyCollection<TaskConfig> Tasks => achievements;
    
    public event Action EventRewardGiven;

    public override void Init()
    {
        achievements.ForEach(x => x.EventRewardGiven += () => EventRewardGiven?.Invoke());
        
        _processors = new List<TaskConfig.Processor>();
        foreach (TaskConfig config in achievements)
        {
            TaskConfig.Processor processor = config.CreateProcessor();
            processor.Initialize();
            processor.EventCompleted += () => OnAchievementCompleted(config);
            _processors.Add(processor);
        }
    }

    private void OnAchievementCompleted(TaskConfig config)
    {
        Debug.Log($"Complete achievement {config.Type}");
    }
}
