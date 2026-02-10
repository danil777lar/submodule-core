using System;
using Larje.Core;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(IGameStateService))]
    public class GameStateService : Service, IGameStateService
    {
        private GameState _currentState;

        public GameState CurrentState 
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    GameState previousState = _currentState;
                    _currentState = value;
                    EventGameStateChanged?.Invoke(previousState, _currentState);
                }
            }
        }

        public event Action<GameState, GameState> EventGameStateChanged;

        public override void Init()
        {
        }

        public void SetGameState(GameState newState, Action onStateChanged = null)
        {
            CurrentState = newState;
            onStateChanged?.Invoke();
        }
    }
}
