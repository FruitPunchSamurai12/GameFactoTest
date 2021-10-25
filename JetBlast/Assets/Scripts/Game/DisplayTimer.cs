using UnityEngine;
using TMPro;
using System;

public class DisplayTimer :MonoBehaviour
{
    TextMeshProUGUI timerText;
    JetEngine jetEngine;

    private void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        jetEngine = FindObjectOfType<JetEngine>();
    }

    private void Update()
    {
        var timeLeft = jetEngine.TimeLeft;
        timerText.SetText(Math.Round(timeLeft, 1).ToString());
    }
}