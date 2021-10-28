using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class RoomListing:MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        text.SetText($"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers} {roomInfo.Name}");
    }

    public void OnClickButton()
    {
        PhotonNetwork.JoinRoom(RoomInfo.Name);
        AudioManager.Instance.PlaySoundEffect2D("Button");
    }

}
