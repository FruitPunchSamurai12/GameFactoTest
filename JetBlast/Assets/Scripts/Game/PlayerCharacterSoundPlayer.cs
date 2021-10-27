using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerCharacterSoundPlayer : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void PlaySoundEvent(string soundName)
    {
        AudioClip clip = AudioManager.Instance.GetSoundEffect(soundName);
        audioSource.volume = AudioManager.Instance.sfxON ? 1 : 0;
        audioSource.PlayOneShot(clip);
    }

}
