using System;
using UnityEngine;

public interface IGameStateService
{
    public GameStateType CurrentState { get; }

    public event Action<GameStateType, GameStateType> EventGameStateChanged;
}

public enum GameStateType
{
    Menu,
    Playing,
    Paused,
    Dialogue,
    Cutscene,
    Win,
    Fail
}
