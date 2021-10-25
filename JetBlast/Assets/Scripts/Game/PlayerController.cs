using System;
using Photon.Pun;
using UnityEngine;
using System.Collections;
using Photon.Pun.UtilityScripts;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject animatedModel;
    [SerializeField]
    GameObject ragdoll;
    [SerializeField]
    Transform ragdollRoot;
    [SerializeField]
    Rigidbody ragdollHead;
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

    [Header("Push things")]
    [SerializeField]
    float pushSpeed = 20f;
    [SerializeField]
    float pushedDuration = .3f;
    [SerializeField]
    float stunDuration = 1f;

    public bool Stunned { get; private set; }

    public bool Moving => playerInput.Move;
    public float Speed { get; private set; }
    public float MaxSpeed => maxSpeed;

    

    public Vector3 Direction { get; private set; }
    public bool Dead { get; private set; }
    public bool InCover { get; private set; }

    public event Action<bool> onGameEnd;
    public event Action onVault;
    public event Action<PlayerController> onPushed;
    public event Action onStunEnd;
    public event Action<bool> onThrowPunch;
    public static GameObject LocalPlayerInstance { get; private set; }

    bool vaulting = false;

    Camera cam;
    Rigidbody rb;
    Rigidbody[] ragdollRigidBodies;
    PlayerInput playerInput = new PlayerInput();
    float startYPosition;//used when getting up after being pushed

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        ragdollRigidBodies = ragdoll.GetComponentsInChildren<Rigidbody>();
        ragdoll.SetActive(false);
        startYPosition = transform.position.y;
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
        if (Dead || Stunned)
            return;
        if(vaulting)
        {
            Speed = vaultSpeed;
            Direction = transform.forward;
            return;
        }
        if (Moving)
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
        if (Dead || Stunned )
            return;
        rb.MovePosition(transform.position + Direction * Speed*Time.deltaTime);
        if(InCover && !vaulting && Moving)
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

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;
        PlayerController pc = other.GetComponent<PlayerController>();
        if(pc!=null && pc!=this)
        {
            if(InCover && !Moving)
            {
                GetPushed(pc);
            }
        }
    }

    private void GetPushed(PlayerController pc)
    {
        onPushed?.Invoke(pc);
        Vector3 forward = pc.transform.forward.FlatVector();
        Vector3 dir = (transform.position.FlatVector() - pc.transform.position.FlatVector()).normalized;
        Vector3 cross = Vector3.Cross(forward, dir);
        if (cross.y > 0)
        {
            pc.onThrowPunch?.Invoke(false);
            photonView.RPC(nameof(RPC_Push), RpcTarget.All, false);
        }
        else
        {
            pc.onThrowPunch?.Invoke(true);
            photonView.RPC(nameof(RPC_Push), RpcTarget.All, true);
        }
    }

    [PunRPC]
    void RPC_Push(bool leftDirection)
    {
        Vector3 dir = (leftDirection ? -transform.right : transform.right - transform.up + transform.forward).normalized;
        animatedModel.SetActive(false);
        ragdoll.SetActive(true);
        ragdoll.transform.SetParent(null);
        ragdollHead.AddForceAtPosition(dir * pushSpeed, ragdollHead.transform.position, ForceMode.VelocityChange);      
        StartCoroutine(Pushed());
    }

    IEnumerator Pushed()
    {
        //Speed = pushSpeed;
        Stunned = true;
        //yield return new WaitForSeconds(pushedDuration);
        Speed = 0;
        yield return new WaitForSeconds(stunDuration);
        Stunned = false;
        onStunEnd?.Invoke();
        transform.position = new Vector3(ragdollRoot.position.x,startYPosition,ragdollRoot.position.z);
        ragdoll.transform.SetParent(transform);
        animatedModel.SetActive(true);
        ragdoll.SetActive(false);
    }

    public void GameEnd(int winnerPlayerNumber)
    {
        if (winnerPlayerNumber == photonView.Controller.GetPlayerNumber())
            onGameEnd?.Invoke(true);
        else
            onGameEnd?.Invoke(false);
        //i know it doesnt make sense but it does what i want which is restricting input and i dont have to create another bool
        Dead = true;
    }
}
