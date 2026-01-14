namespace Larje.Core.Services
{
    public interface ILevelStartHandler
    {
        public void OnLevelStarted(LevelProcessor.StartData data);
    }
}