using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;
using Cinemachine;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject playerPrefab; 
    [SerializeField]
    List<Transform> playerSpawnPoints = new List<Transform>();
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;

    int MaxPlayers =2;
    int currentPlayersConnected = 0;//THIS IS LOCAL
    int currentPlayersLoaded = 0;//THIS INCREAMENTS ONLY ON THE MASTER CLIENT
    public event Action onGameStart;
    public event Action onLocalPlayerDeath;
    public event Action onCountdownStart;

    List<PlayerController> playerControllers = new List<PlayerController>();

    public int PlayersRemaining => playerControllers.Count(t => !t.Dead);
    public float LastPlayerZ => PlayersRemaining>0?playerControllers.Where(t=>!t.Dead).Min(t => t.transform.position.z):0;
    public float FirstPlayerZ => PlayersRemaining > 0 ? playerControllers.Where(t=>!t.Dead).Max(t => t.transform.position.z):0;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        AudioManager.Instance.PlayBGMusic("Game");
    }

    private void Start()
    {
        if (PlayerController.LocalPlayerInstance == null)
        {
            int index = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            GameObject p = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoints[index].transform.position, Quaternion.identity);
            virtualCamera.m_Follow = p.transform;           
        }
        FindObjectOfType<WinZone>().onGameEnd += GameEnd;
    }

    public void AddPlayerController(PlayerController pc, bool localPlayer)
    {
        playerControllers.Add(pc);
        if (localPlayer)
            pc.onDeath += HandlePlayerDied;
        currentPlayersConnected++;
        if (currentPlayersConnected == MaxPlayers)
            photonView.RPC(nameof(RPC_MasterClientAllPlayersLoadSceneReceive), RpcTarget.MasterClient);
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    currentPlayersConnected++;
        //    if (currentPlayersConnected == MaxPlayers)
        //        onGameStart?.Invoke();
        //}
    }

    [PunRPC]
    void RPC_MasterClientAllPlayersLoadSceneReceive()
    {
        currentPlayersLoaded++;
        if (currentPlayersLoaded == MaxPlayers)
        {
            //onGameStart?.Invoke();
            //photonView.RPC(nameof(RPC_AllowInputOnLocalPlayer), RpcTarget.All);
            photonView.RPC(nameof(RPC_StartTimer), RpcTarget.All);
            StartCoroutine(Countdown());
        }
    }

    [PunRPC]
    void RPC_StartTimer()
    {
        onCountdownStart?.Invoke();
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(3f);
        onGameStart?.Invoke();
        photonView.RPC(nameof(RPC_AllowInputOnLocalPlayer), RpcTarget.All);
    }

    [PunRPC]
    void RPC_AllowInputOnLocalPlayer()
    {
        Debug.Log("kitsi");
        PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().HandleGameStart();
    }

    void HandlePlayerDied()
    {
        onLocalPlayerDeath?.Invoke();
    }

    void GameEnd(int winnerPlayerNumber)
    {
        foreach (var pc in playerControllers)
        {
            pc.GameEnd(winnerPlayerNumber);
        }
    }

    public List<PlayerController> GetPlayers() => playerControllers;
}
