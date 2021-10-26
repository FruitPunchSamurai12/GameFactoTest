using Photon.Pun;
using System;
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

    int MaxPlayers =1;
    int currentPlayersConnected = 0;
    public event Action onGameStart;

    List<PlayerController> playerControllers = new List<PlayerController>();

    public int PlayersRemaining => playerControllers.Count(t => !t.Dead);
    public float LastPlayerZ => playerControllers.Where(t=>!t.Dead).Min(t => t.transform.position.z);
    public float FirstPlayerZ => playerControllers.Where(t=>!t.Dead).Max(t => t.transform.position.z);

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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

    public void AddPlayerController(PlayerController pc)
    {
        playerControllers.Add(pc);
        if (PhotonNetwork.IsMasterClient)
        {
            currentPlayersConnected++;
            if (currentPlayersConnected == MaxPlayers)
                onGameStart?.Invoke();
        }
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
