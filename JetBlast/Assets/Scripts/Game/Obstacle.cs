using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Obstacle : PooledMonoBehaviour
{
    [SerializeField]
    float speed = 10f;
    [SerializeField]
    float velocityThresholdToKill = 10f;
    [SerializeField]
    float zThresholdToReturnToPool = -9f;
    [SerializeField]
    string audioClipName = "Obstacle1";

    Rigidbody rb;
    bool entered = false;
    AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

    private void Start()
    {
        audioSource.clip = AudioManager.Instance.GetSoundEffect(audioClipName);       
    }

    public override void OnEnable()
    {
        base.OnEnable();
        entered = false;
    }

    private void Update()
    {
        if (transform.position.z < zThresholdToReturnToPool)
            ReturnToPool();
    }

    private void FixedUpdate()
    {
        if(entered)
            rb.AddForce(new Vector3(0, 0.2f, -0.8f) * speed, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        entered = true;
        if(!AudioManager.Instance.sfxON)
        {
            audioSource.volume = 0;
        }
        else
        {
            audioSource.volume = Mathf.Clamp(rb.velocity.magnitude/velocityThresholdToKill, 0, 1);
        }
        audioSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if(pc!=null)
        {
            if(!pc.InCover && rb.velocity.magnitude>=velocityThresholdToKill)
            {
                pc.KnockOut(rb.velocity);
            }
        }
    }
}
