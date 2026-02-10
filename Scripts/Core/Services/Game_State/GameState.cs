using UnityEngine;

namespace Larje.Core.Services
{
    [CreateAssetMenu(fileName = "GameState", menuName = "Larje/Game State", order = 0)]
    public class GameState : ScriptableObject
    {  
    }

    public static partial class GameStates
    {
        public static GameState Splash => Load("GS_Splash");
        public static GameState Loading => Load("GS_Loading");
        public static GameState Menu => Load("GS_Menu");
        public static GameState Playing => Load("GS_Playing");
        public static GameState Paused => Load("GS_Paused");
        public static GameState Win => Load("GS_Win");
        public static GameState Fail => Load("GS_Fail");
        public static GameState Cutscene => Load("GS_Cutscene");
        public static GameState Dialogue => Load("GS_Dialogue");

        private static GameState Load(string name)
        {
            GameState id = Resources.Load<GameState>($"{name}");
            if (id == null)
            {
                Debug.LogError($"Missing GameStateId Resources asset: {name}");
            }

            return id;
        }
    }
}
