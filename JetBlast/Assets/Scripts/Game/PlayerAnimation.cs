using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator),typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviourPun
{
    Animator animator;
    PlayerController playerController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
       if(photonView.IsMine)
            animator.SetFloat("Speed", playerController.Speed/playerController.MaxSpeed);
    }
}
