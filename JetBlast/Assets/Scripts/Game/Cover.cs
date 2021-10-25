using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviourPunCallbacks
{
    bool occupied = false;
    int occupiedPlayerID = -1;
    PlayerController occupiedPlayer = null;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            if (!occupied)
            {
                PhotonView pcView = pc.GetComponent<PhotonView>();
                if (pcView != null)
                    photonView.RPC(nameof(RPC_TogglePlayerInCoverRange), RpcTarget.All, pcView.ViewID, true);
            }
        }
    }

    [PunRPC]
    private void RPC_TogglePlayerInCoverRange(int pcID, bool inRange)
    {
        PhotonView pcView = PhotonView.Find(pcID);
        if (pcView != null)
        {
            PlayerController pc = pcView.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.InCoverRange(inRange);
                if (inRange)
                {
                    occupied = true;
                    occupiedPlayerID = pcView.ViewID;
                    occupiedPlayer = pc;
                    occupiedPlayer.onPushed += HandlePlayerPushed;
                }
                else
                {
                    if (pc == occupiedPlayer)
                    {
                        occupiedPlayer.onPushed -= HandlePlayerPushed;
                        occupiedPlayerID = -1;
                        occupiedPlayer = null;
                        occupied = false;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            PhotonView pcView = pc.GetComponent<PhotonView>();
            if (pcView != null)
                photonView.RPC(nameof(RPC_TogglePlayerInCoverRange), RpcTarget.All, pcView.ViewID, false);
        }
    }

    private void HandlePlayerPushed(PlayerController pusher)
    {
        PhotonView pcView = pusher.GetComponent<PhotonView>();
        if (pcView != null)
            photonView.RPC(nameof(RPC_HandlePlayerPushed), RpcTarget.All, pcView.ViewID);
    }

    [PunRPC]
    private void RPC_HandlePlayerPushed(int pusherID)
    {
        PhotonView pcView = PhotonView.Find(pusherID);
        if (pcView != null)
        {
            PlayerController pusher = pcView.GetComponent<PlayerController>();
            if (pusher != null)
            {
                occupiedPlayer.InCoverRange(false);
                occupiedPlayer.onPushed -= HandlePlayerPushed;
                occupiedPlayer = pusher;
                occupiedPlayer.InCoverRange(true);
                occupiedPlayer.onPushed += HandlePlayerPushed;
            }
        }
    }
}
