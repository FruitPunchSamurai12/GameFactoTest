using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    PlayerListing playerListingPrefab;
    [SerializeField]
    Transform content;
    [SerializeField]
    TextMeshProUGUI readyText;

    List<PlayerListing> listings = new List<PlayerListing>();

    bool ready;

    public override void OnLeftRoom()
    {
        content.DestroyChildren();
        SetReady(false);
    }

    public void SetReady(bool state)
    {
        ready = state;
        if (ready)
            readyText.SetText("R");
        else
            readyText.SetText("N");
    }


    public void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
        foreach (var playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    private void AddPlayerListing(Player p)
    {
        PlayerListing listing = Instantiate(playerListingPrefab, content);
        if (listing != null)
        {
            listing.SetPlayerInfo(p);
            listings.Add(listing);
        }
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = listings.FindIndex(t => t.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(listings[index].gameObject);
            listings.RemoveAt(index);
        }
    }

    public void OnClickStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < listings.Count; i++)
            {
                if(listings[i].Player !=PhotonNetwork.LocalPlayer)
                {
                    if (!listings[i].Ready)
                        return;
                }
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void OnClickReady()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            SetReady(!ready);
            base.photonView.RPC(nameof(RPC_ChangeReadyState), RpcTarget.MasterClient,PhotonNetwork.LocalPlayer, ready);
        }
    }

    [PunRPC]
    void RPC_ChangeReadyState(Player p, bool ready)
    {
        int index = listings.FindIndex(t => t.Player == p);
        if (index != -1)       
            listings[index].Ready = ready;
        
    }
}
