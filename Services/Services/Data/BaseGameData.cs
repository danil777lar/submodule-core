using System;

namespace Larje.Core.Services.DataService
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
        public bool Vibration = true;
        public float SoundValue = 1f;
        public float MusicValue = 1f;
    }

    [Serializable]
    public partial class IternalData 
    {
        public bool AdsDisabled = false;
        public int SessionNum = 0;
    }
}