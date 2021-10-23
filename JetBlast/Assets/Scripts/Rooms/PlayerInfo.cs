using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInfo : MonoBehaviour
{
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    public void OnNicknameChange(string newNickname)
    {
        playerProperties["Nickname"] = newNickname;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

}
