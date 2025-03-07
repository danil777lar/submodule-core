using System;

namespace Larje.Core.Services
{
    [Serializable]
    public partial class GameData
    {
        public PlayerData PlayerData;
        public Settings Settings;
        public IternalData IternalData;
    }


    [Serializable]
    public partial class PlayerData
    {
        
    }

    [Serializable]
    public partial class Settings
    {
        public bool VibrationGlobal = true;
        public bool SoundGlobal = true;
    }

    [Serializable]
    public partial class IternalData 
    {
        public bool AdsDisabled = false;
        public int SessionNum = 0;
    }
}