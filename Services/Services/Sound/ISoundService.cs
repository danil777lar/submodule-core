using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    public interface ISoundService
    {
        public AudioSource PlayRandomFromSoundPack(SoundType soundType, bool loop);

        public AudioSource PlayFromSoundPackByIndex(SoundType soundType, int id, bool loop);

        public AudioSource PlayClip(AudioClip clip, bool loop);
    }
}