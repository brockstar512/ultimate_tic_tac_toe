using System;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    [SerializeField] TextMeshProUGUI timeTitle;
    public TextMeshProUGUI time;
    private float timeRemaining;
    const float timeCountDown = 5;
    [SerializeField] bool timerIsRunning = false;
    Color32 normalColor = new Color32(101, 138, 167, 255);


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
    private void OnEnable()
    {
        time.text = $"{timeCountDown}:00";
        timeRemaining = timeCountDown;

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

    //reset.
    //when i read that it is my turn
    public void StartTimer(bool isBeginningTurn)
    {
        Debug.Log("Timer is running");
        //TurnIndicatorHandler.Instance.Show();
        //Debug.Log("starting timer " + isBeginningTurn);
        if (!isBeginningTurn)
            return;
        //maybe have a delay? so consider making this a voroutine
        DisplayTime(timeCountDown);

        timeRemaining = timeCountDown;

        // Starts the timer automatically   
        timerIsRunning = true;
    }

    //when i press the cell
    public void StopTimer()
    {
        timerIsRunning = false;
    }

    //this only will run in the event that the player tried to cheat
    public void ContinueTime()
    {
        timerIsRunning = true;
    }


    void MarkCellTimeSuccess()
    {
        timerIsRunning = false;
    }

    void MarkCellTimeFail()
    {
        GameManager.Instance.PlayerTimedOutServerRpc(GameManager.Instance.myPlayer.MyType.Value);
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
