using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

public class Connect : MonoBehaviourPunCallbacks
{
    public UnityEvent onConnected;
    public GameSettings gameSettings;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayBGMusic("Menu");
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = gameSettings.NickName;
            PhotonNetwork.GameVersion = gameSettings.GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
            onConnected?.Invoke();
    }

    public override void OnConnectedToMaster()
    {
        print("connected to master");
        print(PhotonNetwork.LocalPlayer.NickName);
        onConnected?.Invoke();
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("disconnected from server for reason " + cause.ToString());
    }

}
