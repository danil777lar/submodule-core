using UnityEngine;

namespace Larje.Core
{
    public abstract class GameEvent
    {
        public readonly string Source;

        public GameEvent(string source)
        {
            Source = source;
        }
    }
}
