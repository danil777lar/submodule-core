namespace Larje.Core.Services.LevelManagement
{
    public interface ILevelEndHandler
    {
        public void OnLevelEnded(LevelProcessor.StopData data);
    }
}