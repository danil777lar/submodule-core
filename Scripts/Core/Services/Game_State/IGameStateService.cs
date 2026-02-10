using System;
using UnityEngine;

namespace Larje.Core.Services
{
    public interface IGameStateService
    {
        public GameState CurrentState { get; }

        public event Action<GameState, GameState> EventGameStateChanged;

        public void SetGameState(GameState newState, Action onStateChanged = null);
    }
}
