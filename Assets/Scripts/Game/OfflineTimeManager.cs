using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;

public class OfflineTimeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeTitle;
    public static OfflineTimeManager Instance { get; private set; }
    public TextMeshProUGUI time;
    private float timeRemaining;
    const float timeCountDown = 5;
    public bool timerIsRunning = false;
    Color32 normalColor = new Color32(101, 138, 167, 255);
    [SerializeField] AudioClip timerFailedSoundFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        timeRemaining = timeCountDown;
        timerIsRunning = false;
    }
    private void Start()
    {

    }

    private void OnDestroy()
    {


    }

    public void StartTimer()
    {
        DisplayTime(timeCountDown);

        timeRemaining = timeCountDown;

        // Starts the timer automatically   
        timerIsRunning = true;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining >= 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
        }
    }

    void MarkCellTimeSuccess()
    {
        timerIsRunning = false;
    }
    public void StopTimer()
    {
        timerIsRunning = false;
    }

    void MarkCellTimeFail()
    {

    }

    void DisplayTime(float timeToDisplay)
    {
        TimeSpan timeLeft = TimeSpan.FromSeconds(timeToDisplay);
        time.text = string.Format("{0:00}:{1:00}", timeLeft.Seconds, timeLeft.Milliseconds);
        if (timeToDisplay < 0)
        {
            MarkCellTimeFail();
            time.text = string.Format("{0:00}:{1:00}", 00, 00);
            time.text = "00:00";
            timerIsRunning = false;
            timeRemaining = timeCountDown;
        }
    }
}
