using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    float maxSpeed = 8f;
    [SerializeField]
    float acceleration = 10f;

    public float Speed { get; private set; }
    public float MaxSpeed => maxSpeed;

    public static GameObject LocalPlayerInstance { get; private set; }

    Rigidbody rb;
    bool moving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (photonView.IsMine)
            LocalPlayerInstance = gameObject;
    }


    private void Update()
    {
        if (!photonView.IsMine)
            return;
        moving = Input.GetMouseButton(0);
        if (moving)
            Speed = Speed + acceleration * Time.deltaTime;
        else
            Speed = Speed - acceleration * Time.deltaTime;
        Speed = Mathf.Clamp(Speed, 0, maxSpeed);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        rb.MovePosition(transform.position + transform.forward * Speed*Time.deltaTime);
    }

}