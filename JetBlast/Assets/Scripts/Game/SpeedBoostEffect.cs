using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostEffect : MonoBehaviour
{
    [SerializeField]
    ParticleSystem speedBoostParticleSystem;

    private void Start()
    {
        GameManager.Instance.onLocalPlayerLoad += SubscribeToLocalPlayerSpeedBoost;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onLocalPlayerLoad -= SubscribeToLocalPlayerSpeedBoost;
    }

    private void SubscribeToLocalPlayerSpeedBoost(PlayerController pc)
    {
        pc.onSpeedBoost += ActivateEffect;
    }

    private void ActivateEffect()
    {
        speedBoostParticleSystem.time = 0;
        speedBoostParticleSystem.Play();
    }
}
