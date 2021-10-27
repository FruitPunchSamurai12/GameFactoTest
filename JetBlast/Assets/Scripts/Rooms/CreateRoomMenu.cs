using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Events;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TextMeshProUGUI roomNameText;

    public UnityEvent onJoinRoom;

    public override void OnJoinedRoom()
    {
        onJoinRoom?.Invoke();

    }

    public void OnClickCreateRoom()
    {
        if (!PhotonNetwork.IsConnected || string.IsNullOrEmpty(roomNameText.text))
            return;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        PhotonNetwork.JoinOrCreateRoom(roomNameText.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {      
        Debug.Log("Created room successfully");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("room creation failed " + message);
    }
}
