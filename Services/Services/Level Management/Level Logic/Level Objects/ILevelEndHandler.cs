namespace Larje.Core.Services
{
    public interface ILevelEndHandler
    {
        public void OnLevelEnded(LevelProcessor.StopData data);
    }
}