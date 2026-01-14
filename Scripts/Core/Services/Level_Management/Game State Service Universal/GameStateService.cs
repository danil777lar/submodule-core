using System;
using Larje.Core;
using UnityEngine;

[BindService(typeof(GameStateService), typeof(IGameStateService))]
public class GameStateService : Service, IGameStateService
{
    [InjectService] private LocationService _locationService;

    private GameStateType _currentState;

    public GameStateType CurrentState 
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                GameStateType previousState = _currentState;
                _currentState = value;
                EventGameStateChanged?.Invoke(previousState, _currentState);
            }
        }
    }

    public event Action<GameStateType, GameStateType> EventGameStateChanged;

    public override void Init()
    {
        _locationService.EventStartLoadLocation += () => SetGameState(GameStateType.Menu);
        _locationService.EventLocationEntered += (_, _) => SetGameState(GameStateType.Playing);
    }

    public void SetGameState(GameStateType newState, Action onStateChanged = null)
    {
        CurrentState = newState;
        onStateChanged?.Invoke();
    }
}
