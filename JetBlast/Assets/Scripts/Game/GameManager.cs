using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject playerPrefab; 
    [SerializeField]
    List<Transform> playerSpawnPoints = new List<Transform>();
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;

    List<PlayerController> playerControllers = new List<PlayerController>();

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
