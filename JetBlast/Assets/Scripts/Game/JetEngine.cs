using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JetEngine : MonoBehaviourPunCallbacks
{
    [SerializeField]
    float blowWindThreshold = 5f;
    [SerializeField]
    float blowWindDuration = 3f;
    [SerializeField]
    float windForce = 25f;

    AudioSource audioSource;

    bool startWind = false;

    public float Timer { get; private set; }

    public bool StrongWind { get; private set; }
    public float TimeLeft => Timer > blowWindThreshold ? blowWindThreshold + blowWindDuration - Timer : blowWindThreshold - Timer;

    public event Action onWindStart;
    public event Action onWindReset;
    public event Action onStrongWind;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            GameManager.Instance.onGameStart += HandleGameStart;
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
            GameManager.Instance.onGameStart -= HandleGameStart;
    }

    void HandleGameStart()
    {
        photonView.RPC(nameof(RPC_HandleGameStart), RpcTarget.All);
    }

    [PunRPC]
    void RPC_HandleGameStart()
    {
        startWind = true;
        onWindStart?.Invoke();
        audioSource.clip = AudioManager.Instance.GetSoundEffect("Wind");
        audioSource.volume = AudioManager.Instance.sfxON ? 1 : 0;
        audioSource.Play();
    }

    void Update()
    {
        if (!startWind)
            return;
        Timer += Time.deltaTime;
        if (Timer > blowWindThreshold)
        {
            if (!StrongWind)
                onStrongWind?.Invoke();
            StrongWind = true;
            foreach (var pc in GameManager.Instance.GetPlayers())
            {
                if (pc != null && !pc.InCover)
                    pc.GetBlownAway((new Vector3(0, 1, -.1f)).normalized * windForce);
            }
            if (Timer > blowWindThreshold + blowWindDuration)
            {
                Timer = 0;
                StrongWind = false;
                onWindReset?.Invoke();
            }
        }
    }
}
