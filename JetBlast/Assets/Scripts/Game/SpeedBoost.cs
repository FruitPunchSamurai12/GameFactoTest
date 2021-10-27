using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : PooledMonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if(pc!=null)
        {
            PhotonView pv = pc.GetComponent<PhotonView>();
            if(pv.IsMine)
            {
                pc.GottaGoFast();
                AudioManager.Instance.PlaySoundEffect3D("SpeedBoost", transform.position);
                ReturnToPool();
            }
        }
    }
}
