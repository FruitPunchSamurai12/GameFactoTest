﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator),typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviourPunCallbacks
{
    Animator animator;
    PlayerController playerController;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        playerController.onVault += () => animator.SetTrigger("Vault");
        playerController.onStunEnd += () => animator.SetTrigger("Stand");
        playerController.onThrowPunch += PunchAnimation;
    }

    private void PunchAnimation(bool leftPunch)
    {
        photonView.RPC(nameof(RPC_PunchAnimation), RpcTarget.All, leftPunch);
    }

    [PunRPC]
    void RPC_PunchAnimation(bool leftPunch)
    {
        animator.SetTrigger("Hook");
        animator.SetBool("Left", leftPunch);
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        if (playerController.Dead)
            return;
        animator.SetFloat("Speed", playerController.Speed/playerController.MaxSpeed);
        animator.SetBool("Crouch", playerController.InCover && !playerController.Moving);
    }


}
