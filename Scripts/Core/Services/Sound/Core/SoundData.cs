using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    public partial class GameData
    {
        
    }   
    
    public partial class Settings
    {
        public SoundData SoundData;
    }
}

[Serializable]
public class SoundData
{
    public List<SoundChannelData> Channels;
    
    public SoundChannelData GetChannel(string channelName)
    {
        Channels ??= new List<SoundChannelData>();
        SoundChannelData channel = Channels.Find(c => c.Name == channelName);
        if (channel == null)
        {
            channel = new SoundChannelData
            {
                Name = channelName,
                Volume = 1f
            };
            
            Channels.Add(channel);
        }
        return channel;
    }
}

[Serializable]
public class SoundChannelData
{
    public string Name;
    public float Volume;
}
