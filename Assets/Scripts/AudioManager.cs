using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField, Tooltip("Music plays on a loop throught the game.")]
    public Sound Music;

    [SerializeField, Tooltip("Sounds can be called by their name in scripts.")]
    public Sound[] Sounds;
    private void Awake()
    {
        foreach (Sound s in Sounds)
        {
            s.AudioSource = gameObject.AddComponent<AudioSource>();
            s.AudioSource.clip = s.Clip;

            s.AudioSource.volume = s.Volume;
            s.AudioSource.pitch = s.Pitch;
            s.AudioSource.spatialBlend = s.SpacialBlend;
            s.AudioSource.dopplerLevel = 0;
        }

        Music.AudioSource = gameObject.AddComponent<AudioSource>();
        Music.AudioSource.clip = Music.Clip;
        Music.AudioSource.volume = Music.Volume * Settings.MusicVolume;
        Music.AudioSource.pitch = Music.Pitch;
        Music.AudioSource.spatialBlend = Music.SpacialBlend;
        Music.AudioSource.dopplerLevel = 0;

        Music.AudioSource.loop = true;
        Music.AudioSource.Play();
    }

    public AudioSource PlayOnTarget(string name, GameObject target, bool loop = false)
    {
        Sound sound = null;
        foreach (Sound s in Sounds)
            if (s.Name == name)
                sound = s;

        if (sound == null)
            return null;

        var audio = target.AddComponent<AudioSource>();
        audio.clip = sound.Clip;
        audio.volume = sound.Volume * Settings.SFXVolume;
        audio.pitch = sound.Pitch;
        audio.loop = loop;
        audio.spatialBlend = sound.SpacialBlend;
        audio.dopplerLevel = 0;
        StartCoroutine(PlayOnTargetCoroutine(audio));

        return audio;
    }

    public void FadeOut(AudioSource audio, float duration)
    {
        audio.DOFade(0f, duration).OnComplete(() => audio.Stop());
    }

    public AudioSource Play(string name, bool loop = false, Vector3? position = null, float? pitch = null)
    {
        Sound sound = null;
        foreach (Sound s in Sounds)
            if (s.Name == name)
                sound = s;
        
        if (sound == null)
            return null;

        if (position != null)
        {
            var go = new GameObject(sound.Name);
            go.transform.position = position.Value;
            var audio = go.gameObject.AddComponent<AudioSource>();

            audio.clip = sound.Clip;
            audio.volume = sound.Volume * Settings.SFXVolume;
            audio.pitch = (pitch == null) ? sound.Pitch : pitch.Value;
            audio.spatialBlend = sound.SpacialBlend;
            audio.loop = loop;
            audio.dopplerLevel = 0;

            StartCoroutine(PlaySpacialSoundCoroutine(audio));
            return audio;
        }



        sound.AudioSource.volume = sound.Volume * Settings.SFXVolume;
        sound.AudioSource.pitch = (pitch == null) ? sound.Pitch : pitch.Value;
        sound.AudioSource.spatialBlend = 0;
        sound.AudioSource.dopplerLevel = 0;

        sound.AudioSource.loop = loop;
        sound.AudioSource.Play();
        return sound.AudioSource;
    }

    public void Stop(string name)
    {
        foreach (Sound s in Sounds)
            if (s.Name == name)
            {
                s?.AudioSource.Stop();
                return;
            }
    }

    public void StopLoop(string name)
    {
        foreach (Sound s in Sounds)
            if (s.Name == name)
            {
                s.AudioSource.loop = false;
                return;
            }
    }

    IEnumerator PlaySpacialSoundCoroutine(AudioSource audio)
    {
        audio.Play();

        while (audio.isPlaying)
            yield return null;

        Destroy(audio.gameObject);
    }

    IEnumerator PlayOnTargetCoroutine(AudioSource audio)
    {
        audio.Play();
        while (audio.isPlaying)
            yield return null;

        Destroy(audio);
    }
}
