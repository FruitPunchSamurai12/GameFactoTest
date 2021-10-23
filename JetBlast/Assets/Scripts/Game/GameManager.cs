using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject playerPrefab; 
    public static GameManager Instance { get; private set; }

    [SerializeField]
    List<Transform> playerSpawnPoints = new List<Transform>();

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
            PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoints[index].transform.position, Quaternion.identity);

        }
    }

}
