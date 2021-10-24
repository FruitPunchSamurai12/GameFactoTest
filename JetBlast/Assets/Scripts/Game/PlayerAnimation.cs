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
        playerController.onVault += () => animator.SetTrigger("Vault");
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        if (playerController.Dead)
            return;
        animator.SetFloat("Speed", playerController.Speed/playerController.MaxSpeed);
        animator.SetBool("Crouch", playerController.InCover && playerController.Speed < 0.1f);
    }
}
