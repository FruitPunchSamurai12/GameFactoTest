using UnityEngine;
using TMPro;
using Photon.Realtime;

public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    public Player Player { get; private set; }
    public bool Ready = false;

    public void SetPlayerInfo(Player p)
    {
        Player = p;
        if(Player.CustomProperties.ContainsKey("Nickname"))
            Player.NickName = Player.CustomProperties["Nickname"].ToString();
        text.SetText(Player.NickName);
    }
}