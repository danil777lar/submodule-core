using System;
using UnityEngine;

namespace Larje.Core.Services
{
    [Serializable]
    public class SystemData
    {
        public Settings Settings = new Settings();
        public IternalData IternalData = new IternalData();
    }
    
    [Serializable]
    public partial class Settings
    {
        public bool VibrationGlobal = true;
        public bool SoundGlobal = true;
        public GraphicsSettings Graphics;
    }

    [Serializable]
    public partial class GraphicsSettings
    {
    }

    [Serializable]
    public partial class IternalData 
    {
        public bool AdsDisabled = false;
        public int SessionNum = 0;
    }
}
