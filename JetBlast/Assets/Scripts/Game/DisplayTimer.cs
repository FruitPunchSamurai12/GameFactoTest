using UnityEngine;
using TMPro;
using System;
using Photon.Pun;

public class DisplayTimer :MonoBehaviour
{
    TextMeshProUGUI timerText;
    JetEngine jetEngine;

    bool gameStarted = false;
    bool countdownStarted = false;

    float countdownTimer = 0;

    private void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        jetEngine = FindObjectOfType<JetEngine>();
        GameManager.Instance.onCountdownStart += HandleCountdownStart;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onCountdownStart -= HandleCountdownStart;
    }

    void HandleCountdownStart()
    {
        countdownStarted = true;
        countdownTimer = 0;
        AudioManager.Instance.PlaySoundEffect2D("Countdown");
    }

    private void Update()
    {
        if (gameStarted)
        {
            var timeLeft = jetEngine.TimeLeft;
            timerText.color = jetEngine.StrongWind ? Color.red : Color.white;
            timerText.SetText(Math.Round(timeLeft, 1).ToString());
        }
        else if(countdownStarted)
        {
            countdownTimer += Time.deltaTime;
            var timeLeft = 3 - countdownTimer;
            timerText.SetText(Math.Round(timeLeft, 1).ToString());
            if (countdownTimer>=3)
            {
                gameStarted = true;
            }
        }
        else
        {
            timerText.SetText("");
        }
    }
}