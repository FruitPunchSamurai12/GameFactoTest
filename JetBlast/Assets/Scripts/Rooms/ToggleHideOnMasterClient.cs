using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHideOnMasterClient : MonoBehaviourPunCallbacks
{
    [SerializeField]
    bool hideOnMasterClient;


    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {           
            if (hideOnMasterClient)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }
        else
        {
            if (hideOnMasterClient)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }
    }
}
