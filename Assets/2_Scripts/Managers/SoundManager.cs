using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace LUP
{
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Audio Source Prefabs")]
        public AudioSource bgmSource;
        public AudioSource sfxPrefab;

        [Header("Audio Volume")]
        public float currentBGMVolume=1;
        public float currentSFXVolume=1;

        [Header("SFX Settings")]
        public int maxSameSFXCount = 10;
        private Dictionary<string, List<AudioSource>> activeSFX = new();

        Vector3 zeroVector = Vector3.zero;
        public void PlayBGM(string bgmname, bool loop = true)
        {
            AudioClip clip = ResourceManager.Instance.LoadAudioBGM<AudioClip>(bgmname);
            if (clip == null)
            {
                Debug.LogWarning($"[SoundManager] BGM not found: {name}");
                return;
            }

            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = currentBGMVolume;
            bgmSource.Play();
        }
        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void PlaySFX(string sfxname, GameObject gameobject = null, bool spatial = true)
        {
            AudioClip clip = ResourceManager.Instance.LoadAudioSFX<AudioClip>(sfxname);
            if (clip == null)
            {
                Debug.LogWarning($"[SoundManager] SFX not found: {name}");
                return;
            }

            if (!activeSFX.ContainsKey(name))
                activeSFX[name] = new List<AudioSource>();

            List<AudioSource> list = activeSFX[name];
            list.RemoveAll(a => a == null || !a.isPlaying);

            if (list.Count >= maxSameSFXCount)
                return;
            AudioSource newSFX;

            if (gameobject == null)
            {
                newSFX = Instantiate(sfxPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                newSFX = Instantiate(sfxPrefab, gameobject.transform.position, Quaternion.identity);
            }
            newSFX.clip = clip;
            newSFX.volume = currentSFXVolume;
            newSFX.spatialBlend = spatial ? 1f : 0f;
            newSFX.Play();

            list.Add(newSFX);


            Destroy(newSFX.gameObject, clip.length + 0.1f);
        }

        public void SetBGMVolume(float volume)
        {
            currentBGMVolume = volume;
            bgmSource.volume = currentBGMVolume;
        }

        public void SetSFXVolume(float volume)
        {
            currentSFXVolume = volume;

            foreach (KeyValuePair<string, List<AudioSource>> pair in activeSFX)
            {
                foreach (AudioSource src in pair.Value)
                {
                    if (src != null)
                        src.volume = currentSFXVolume;
                }
            }
        }
    }
}
