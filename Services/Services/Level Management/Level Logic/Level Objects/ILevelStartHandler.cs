namespace Larje.Core.Services.LevelManagement
{
    public interface ILevelStartHandler
    {
        public void OnLevelStarted(LevelProcessor.StartData data);
    }
}