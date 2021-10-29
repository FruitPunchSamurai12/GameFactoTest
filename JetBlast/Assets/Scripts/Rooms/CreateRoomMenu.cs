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

    int maxPlayers;

    private void Start()
    {
        maxPlayers = FindObjectOfType<Connect>().gameSettings.MaxPlayers;
    }

    public override void OnJoinedRoom()
    {
        onJoinRoom?.Invoke();

    }

    public void OnClickCreateRoom()
    {
        if (!PhotonNetwork.IsConnected || string.IsNullOrEmpty(roomNameText.text))
            return;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayers;
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
