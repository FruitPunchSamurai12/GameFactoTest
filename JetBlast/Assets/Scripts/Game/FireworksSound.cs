using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksSound : MonoBehaviour
{
    ParticleSystem effect;
    AudioSource audioSource;

    bool playedOnce = false;
    private void Awake()
    {
        effect = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.clip = AudioManager.Instance.GetSoundEffect("Fireworks");
    }

    // Update is called once per frame
    void Update()
    {
        if (playedOnce)
        {
            if (!effect.IsAlive())
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.time = 0;
                    audioSource.Play();
                }
            }
        }
        else
        {
            if (effect.IsAlive())
                playedOnce = true;
        }
    }
}
