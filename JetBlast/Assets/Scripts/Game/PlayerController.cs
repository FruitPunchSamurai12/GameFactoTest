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
    float pushForce = 20f;
    [SerializeField]
    float stunDuration = 1f;

    [Header("Speed boost things")]
    [SerializeField]
    float boostMaxSpeedModifier = 1.2f;
    [SerializeField]
    float speedBoostDuration = 3f;

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
    public event Action onRagdoll;
    public event Action onDeath;
    public event Action<bool> onThrowPunch;
    public static GameObject LocalPlayerInstance { get; private set; }

    bool vaulting = false;
    float currentMaxSpeedModifier = 1;

    Camera cam;
    Rigidbody rb;
    Rigidbody[] ragdollRigidBodies;
    PlayerInput playerInput = new PlayerInput();
    float startYPosition;//used when getting up after being pushed
    bool gameStarted = false;

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
        GameManager.Instance.AddPlayerController(this,photonView.IsMine);
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
            LocalPlayerInstance = null;
    }


    public void HandleGameStart() => gameStarted = true;

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        if (Dead || Stunned || !gameStarted)
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
        Speed = Mathf.Clamp(Speed, 0, maxSpeed*currentMaxSpeedModifier);

    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        if (Dead || Stunned ||!gameStarted)
            return;
        rb.MovePosition(transform.position + Direction * Speed*Time.deltaTime);
        if(InCover && !vaulting && Moving)
        {
            if(Physics.Raycast(rayStartPosition.position, transform.forward,checkForVaultRayLength,coverLayerMask))
            {
                Debug.Log("ktis");
                Vault();
            }
        }
    }

    public void GetBlownAway(Vector3 force)
    {
        if(!Dead)
        {
            AudioManager.Instance.PlaySoundEffect3D("WindDeath", transform.position);
            Dead = true;
            onDeath?.Invoke();
            animatedModel.SetActive(false);
            ragdoll.SetActive(true);
            onRagdoll?.Invoke();
        }
        if (PhotonNetwork.IsMasterClient)
            force = force / 2f;//i dont know why but on the master client the ragdoll travels much higher in the air than in the other clients

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
                GetPunched(pc);
            }
        }
    }

    private void GetPunched(PlayerController pc)
    {
        onPushed?.Invoke(pc);
        Vector3 forward = pc.transform.forward.FlatVector();
        Vector3 dir = (transform.position.FlatVector() - pc.transform.position.FlatVector()).normalized;
        Vector3 cross = Vector3.Cross(forward, dir);
        if (cross.y > 0)
        {
            pc.onThrowPunch?.Invoke(false);
            photonView.RPC(nameof(RPC_Punch), RpcTarget.All, false);
        }
        else
        {
            pc.onThrowPunch?.Invoke(true);
            photonView.RPC(nameof(RPC_Punch), RpcTarget.All, true);
        }
    }

    [PunRPC]
    void RPC_Punch(bool leftDirection)
    {
        Vector3 dir = leftDirection ? Vector3.left : Vector3.right;
        animatedModel.SetActive(false);
        onRagdoll?.Invoke();
        ragdoll.SetActive(true);
        ragdoll.transform.SetParent(null);
        ragdollHead.AddForce(dir * pushForce, ForceMode.VelocityChange);
        AudioManager.Instance.PlaySoundEffect3D("PunchReaction", ragdollHead.transform.position);
        StartCoroutine(Punched());
    }

    IEnumerator Punched()
    {
        Stunned = true;
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

    public void KnockOut(Vector3 force)
    {
        if (!Dead)
        {
            AudioManager.Instance.PlaySoundEffect3D("ObstacleHitReaction", transform.position);
            Dead = true;
            onDeath?.Invoke();
            StopCoroutine(Punched());
            animatedModel.SetActive(false);
            onRagdoll?.Invoke();
            ragdoll.SetActive(true);
        }

        foreach (var rb in ragdollRigidBodies)
        {
            rb.AddForce(force, ForceMode.VelocityChange);
        }
    }

    public void GottaGoFast()
    {
        StartCoroutine(SpeedUp());
    }

    IEnumerator SpeedUp()
    {
        currentMaxSpeedModifier = boostMaxSpeedModifier;
        yield return new WaitForSeconds(speedBoostDuration);
        currentMaxSpeedModifier = 1;
    }
}
