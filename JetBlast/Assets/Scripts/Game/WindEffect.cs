using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem wind1;
    [SerializeField] ParticleSystem wind2;

    [SerializeField] float normalEmission = 0.5f;
    [SerializeField] float strongEmission = 2f;
    [SerializeField] float normalSpeedModifier = 1;
    [SerializeField] float strongSpeedModifier = 1.5f;

    private void Start()
    {
        JetEngine jetEngine = FindObjectOfType<JetEngine>();
        jetEngine.onWindStart += HandleWindStart;
        jetEngine.onStrongWind += StrongWind;
        jetEngine.onWindReset += NormalWind;

    }

    void HandleWindStart()
    {
        wind1.gameObject.SetActive(true);
        wind2.gameObject.SetActive(true);
    }

    void StrongWind()
    {
        var emission1 = wind1.emission;
        emission1.rateOverTime = strongEmission;
        var velocity1 = wind1.velocityOverLifetime;
        velocity1.speedModifierMultiplier = strongSpeedModifier;
    
        var emission2 = wind2.emission;
        emission2.rateOverTime = strongEmission;
        var velocity2 = wind2.velocityOverLifetime;
        velocity2.speedModifierMultiplier = strongSpeedModifier;
    }
    
    void NormalWind()
    {
        var emission1 = wind1.emission;
        emission1.rateOverTime = normalEmission;
        var velocity1 = wind1.velocityOverLifetime;
        velocity1.speedModifierMultiplier = normalSpeedModifier;
    
        var emission2 = wind2.emission;
        emission2.rateOverTime = normalEmission;
        var velocity2 = wind2.velocityOverLifetime;
        velocity2.speedModifierMultiplier = normalSpeedModifier;
    }

}
