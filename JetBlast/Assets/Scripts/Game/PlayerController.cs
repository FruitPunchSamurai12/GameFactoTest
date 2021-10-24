using System;
using Photon.Pun;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject animatedModel;
    [SerializeField]
    GameObject ragdoll;
    [SerializeField]
    float maxSpeed = 8f;
    [SerializeField]
    float acceleration = 10f;
    [SerializeField]
    float deacceleration = 20f;
    [SerializeField]
    float leftRightInputOffset = 20f;
    
    [Header("Vault things")]
    [SerializeField]
    float vaultSpeed = 10f;
    [SerializeField]
    float vaultDuration = .5f;
    [SerializeField]
    float checkForVaultRayLength = .5f;
    [SerializeField]
    LayerMask coverLayerMask;
    [SerializeField]
    Transform rayStartPosition;

    public float Speed { get; private set; }
    public float MaxSpeed => maxSpeed;
    public Vector3 Direction { get; private set; }
    public bool Dead { get; private set; }
    public bool InCover { get; private set; }

    public event Action onVault;
    public static GameObject LocalPlayerInstance { get; private set; }

    bool vaulting = false;

    Camera cam;
    Rigidbody rb;
    Rigidbody[] ragdollRigidBodies;
    PlayerInput playerInput = new PlayerInput();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        ragdollRigidBodies = ragdoll.GetComponentsInChildren<Rigidbody>();
        ragdoll.SetActive(false);
        if (photonView.IsMine)
            LocalPlayerInstance = gameObject;
    }

    private void Start()
    {
        GameManager.Instance.AddPlayerController(this);       
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        if (Dead)
            return;
        if(vaulting)
        {
            Speed = vaultSpeed;
            return;
        }
        if (playerInput.Move)
        {
            Speed = Speed + acceleration * Time.deltaTime;
            float xOnScreen = cam.WorldToScreenPoint(transform.position).x;
            float inputX = playerInput.TapX;
            if (inputX < xOnScreen - leftRightInputOffset)
                Direction = transform.forward - transform.right;
            else if (inputX > xOnScreen + leftRightInputOffset)
                Direction = transform.forward + transform.right;         
            else
                Direction = transform.forward;
            Direction = Direction.normalized;
        }
        else
        {
            Speed = Speed - deacceleration * Time.deltaTime;
            Direction = transform.forward;
        }
        Speed = Mathf.Clamp(Speed, 0, maxSpeed);

    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        if (Dead)
            return;
        rb.MovePosition(transform.position + Direction * Speed*Time.deltaTime);
        if(InCover && !vaulting && playerInput.Move)
        {
            if(Physics.Raycast(rayStartPosition.position, transform.forward,checkForVaultRayLength,coverLayerMask))
            {
                Vault();
            }
        }
    }

    public void GetBlownAway(Vector3 force)
    {
        if(!Dead)
        {
            Dead = true;
            animatedModel.SetActive(false);
            ragdoll.SetActive(true);
        }
        
        foreach (var rb in ragdollRigidBodies)
        {
            rb.AddForceAtPosition(force, rb.transform.position, ForceMode.Acceleration);
        }
    }

    public void InCoverRange(bool inCoverRange)
    {
        InCover = inCoverRange;
    }

    public void Vault()
    {
        vaulting = true;
        onVault?.Invoke();
        StartCoroutine(Vaulting());
    }

    IEnumerator Vaulting()
    {
        yield return new WaitForSeconds(vaultDuration);
        vaulting = false;
    }
}