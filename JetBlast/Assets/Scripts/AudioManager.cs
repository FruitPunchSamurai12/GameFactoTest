using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //all the background music
    [SerializeField]
    Sound[] bgMusic;

    //all the sound effects
    [SerializeField]
    Sound[] soundEffects;

    //different audio sources for bg and fx
    [SerializeField]
    AudioSource bgSource;

    [SerializeField]
    AudioSource[] fxSources;


    public static AudioManager Instance {get; private set;}
    public bool bgON { get; private set; }
    public bool sfxON { get; private set; }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bgON = true;
        sfxON = true;
    }

    public void MuteSound()
    {
        MuteBGMusic();
        MuteSoundEffects();
    }

    public void UnMuteSound()
    {
        UnMuteBGMusic();
        UnMuteSoundEffects();
    }

    public void MuteBGMusic()
    {
        bgSource.volume = 0;
        bgON = false;
    }

    public void UnMuteBGMusic()
    {
        bgSource.volume = 1;
        bgON = true;
    }

    public void MuteSoundEffects()
    {       
        sfxON = false;
    }

    public void UnMuteSoundEffects()
    {
        sfxON = true;
    }

    //find and play bg music using a string 
    public void PlayBGMusic(string name)
    {
        Sound s = new Sound();
        foreach (Sound sound in bgMusic)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if (bgSource.clip != s.clip)
        {
            bgSource.clip = s.clip;
            bgSource.Play();
        }
    }

    public void StopBGMusic()
    {
        bgSource.Stop();
    }

    //this finds and plays a sound effect using the audio manager audio source
    public void PlaySoundEffect3D(string name,Vector3 position)
    {
        Sound s = new Sound();
        foreach (Sound sound in soundEffects)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if(s.clip==null)
        {
            Debug.LogError("Couldnt find sound");
            return;
        }
        foreach(AudioSource source in fxSources)
        {
            if(!source.isPlaying)
            {
                source.spatialBlend = 1;
                source.transform.position = position;
                source.PlayOneShot(s.clip,sfxON?1:0);
                return;
            }
        }    
    }

    public void PlaySoundEffect2D(string name)
    {
        Sound s = new Sound();
        foreach (Sound sound in soundEffects)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if (s.clip == null)
        {
            Debug.LogError("Couldnt find sound");
            return;
        }
        foreach (AudioSource source in fxSources)
        {
            if (!source.isPlaying)
            {
                source.spatialBlend = 0;
                source.PlayOneShot(s.clip, sfxON ? 1 : 0);
                return;
            }
        }
    }

    //this returns the audio clip of a sound effect for other objects to play
    public AudioClip GetSoundEffect(string name)
    {
        Sound s = new Sound();
        foreach (Sound sound in soundEffects)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if (s != null)
        {
            return s.clip;
        }
        else
        {
            return null;
        }
    }

    public AudioClip GetBGMusic(string name)
    {
        Sound s = new Sound();
        foreach (Sound sound in bgMusic)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if (s != null)
        {
            return s.clip;
        }
        else
        {
            return null;
        }
    }
}
