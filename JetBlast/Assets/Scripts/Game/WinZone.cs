using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class WinZone : MonoBehaviourPun
{
    public event Action<int> onGameEnd;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if(pc!=null)
        {
            PhotonView playerView = pc.GetComponent<PhotonView>();
            int winnerPlayerNumber = playerView.Controller.GetPlayerNumber();
            photonView.RPC(nameof(RPC_SendGameEndEvent), RpcTarget.All, winnerPlayerNumber);
        }
    }

    [PunRPC]
    void RPC_SendGameEndEvent(int winnerPlayerNumber)
    {
        onGameEnd?.Invoke(winnerPlayerNumber);
    }
}
