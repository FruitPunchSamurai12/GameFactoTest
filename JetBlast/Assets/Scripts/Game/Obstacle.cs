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

    Rigidbody rb;
    bool entered = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
