using System;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(ISoundService))]
    public class SoundService : Service, ISoundService
    {
        [SerializeField] private SoundServiceConfig _config;
        
        [InjectService] private DataService _dataService;

        public override void Init() { }

        /// <summary>
        /// Спавнит GameObject c AudioSource, помещает в него рандомный клип из набора с соответствующим названем, задает настройки громкости.
        /// Объект автоматически удаляется после проигрывания клипа.
        /// </summary>
        public AudioSource PlayRandomFromSoundPack(SoundType soundType, bool loop)
        {
            SoundOption sound = GetRandomSound(soundType);
            if (!sound.clip)
            {
                return null;
            }
            return SpawnSoundSource(sound.clip, loop, sound.volume);
        }

        public AudioSource PlayFromSoundPackByIndex(SoundType soundType, int id, bool loop)
        {
            SoundOption sound = GetSoundByIndex(soundType, id);
            if (!sound.clip)
            {
                return null;
            }
            return SpawnSoundSource(sound.clip, loop, sound.volume);
        }
        
        /// <summary>
        /// Спавнит GameObject c AudioSource, помещает в него рандомный клип.
        /// Объект автоматически удаляется после проигрывания клипа.
        /// </summary>
        public AudioSource PlayClip(AudioClip clip, bool loop)
        {
            return SpawnSoundSource(clip, loop);
        }

        /// <summary>
        /// Возвращает рандомный звук с настройками громкости из соответствующего набора.
        /// </summary>
        private SoundOption GetRandomSound(SoundType soundType)
        {
            SoundPack pack = _config.soundPacks.Find((p) => p.soundType == soundType);
            if (pack == null || pack.sounds.Count <= 0) return null;
            return pack.sounds[UnityEngine.Random.Range(0, pack.sounds.Count)];
        }

        private SoundOption GetSoundByIndex(SoundType soundType, int id) 
        {
            SoundPack pack = _config.soundPacks.Find((p) => p.soundType == soundType);
            if (pack == null || pack.sounds.Count <= 0) return null;
            return pack.sounds[Mathf.Clamp(id, 0, pack.sounds.Count)];
        }

        private AudioSource SpawnSoundSource(AudioClip clip, bool loop, float volume = 1f)
        {
            AudioSource source = new GameObject($"{clip.name} source").AddComponent<AudioSource>();
            source.transform.SetParent(gameObject.transform);
            source.transform.localPosition = Vector3.zero;
            source.clip = clip;
            source.loop = loop;
            source.pitch += UnityEngine.Random.Range(-0.1f, 0.1f);
            if (_dataService.Data.Settings.SoundGlobal)
            {
                source.volume = volume;
            }
            else
            {
                source.volume = 0f;
            }

            source.Play();

            if (!loop)
            {
                Destroy(source.gameObject, clip.length);
            }

            return source;
        }
        
        [Serializable] 
        public class SoundPack
        {
            public SoundType soundType;
            public List<SoundOption> sounds;
        }
        
        [Serializable] 
        public class SoundOption
        {
            public AudioClip clip;
            public float volume = 1f;

            public SoundOption() 
            {
                volume = 1f;
            }
        }
    }
}