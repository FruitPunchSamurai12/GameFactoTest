using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class LeaveRoomMenu : MonoBehaviourPunCallbacks
{
    public UnityEvent onLeaveRoom;

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        OnClickLeaveRoom();
    }

    public void OnClickLeaveRoom()
    {
        AudioManager.Instance.PlaySoundEffect2D("Button");
        PhotonNetwork.LeaveRoom(true);
        onLeaveRoom?.Invoke();
    }
}