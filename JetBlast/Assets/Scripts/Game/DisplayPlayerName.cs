using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class DisplayPlayerName : MonoBehaviour
{
    TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        PhotonView pv = GetComponentInParent<PhotonView>();
        Player player = pv.Owner;
        if (player.CustomProperties.ContainsKey("Nickname"))
            player.NickName = player.CustomProperties["Nickname"].ToString();
        text.SetText(player.NickName);
    }
}
