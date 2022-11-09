﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    [BindService(typeof(SoundService))]
    public class SoundService : Service
    {
        [Serializable] public class SoundPack
        {
            public SoundType soundType;
            public List<SoundOption> sounds;
        }
        [Serializable] public class SoundOption
        {
            public AudioClip clip;
            public float volume = 1f;
        }

        public List<SoundPack> soundPacks;

        public override void Init() { }

        /// <summary>
        /// Спавнит GameObject c AudioSource, помещает в него рандомный клип из набора с соответствующим названем, задает настройки громкости.
        /// Объект автоматически удаляется после проигрывания клипа.
        /// </summary>
        public void PlayRandomFromSoundPack(SoundType soundType)
        {
            SoundOption sound = GetRandomSound(soundType);
            if (!sound.clip)
            {
                return;
            }
            SpawnSoundSource(sound.clip, sound.volume);
        }

        public void PlayFromSoundPackByIndex(SoundType soundType, int id)
        {
            SoundOption sound = GetSoundByIndex(soundType, id);
            if (!sound.clip)
            {
                return;
            }
            SpawnSoundSource(sound.clip, sound.volume);
        }



        /// <summary>
        /// Спавнит GameObject c AudioSource, помещает в него рандомный клип.
        /// Объект автоматически удаляется после проигрывания клипа.
        /// </summary>
        public void PlayClip(AudioClip clip)
        {
            SpawnSoundSource(clip);
        }

        /// <summary>
        /// Возвращает рандомный звук с настройками громкости из соответствующего набора.
        /// </summary>
        public SoundOption GetRandomSound(SoundType soundType)
        {
            SoundPack pack = soundPacks.Find((p) => p.soundType == soundType);
            if (pack == null || pack.sounds.Count <= 0) return null;
            return pack.sounds[UnityEngine.Random.Range(0, pack.sounds.Count)];
        }

        public SoundOption GetSoundByIndex(SoundType soundType, int id) 
        {
            SoundPack pack = soundPacks.Find((p) => p.soundType == soundType);
            if (pack == null || pack.sounds.Count <= 0) return null;
            return pack.sounds[Mathf.Clamp(id, 0, pack.sounds.Count)];
        }

        private void SpawnSoundSource(AudioClip clip, float volume = 1f)
        {
            AudioSource source = new GameObject($"{clip.name} source").AddComponent<AudioSource>();
            source.transform.SetParent(gameObject.transform);
            source.transform.localPosition = Vector3.zero;
            source.clip = clip;
            source.volume = volume;
            source.pitch += UnityEngine.Random.Range(-0.1f, 0.1f);
            source.Play();
            Destroy(source.gameObject, clip.length);
        }
    }
}