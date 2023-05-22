using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Enums;

public class OfflineGameManager : MonoBehaviour
{
    public static OfflineGameManager Instance { get; private set; }
    public MarkType GetCurrentType { get { return players[CurrentPlayerIndex].MyType; } }
    public bool InputsEnabled { get; set; }
    public static event Action<MarkType> TimeOut;
    public Color GetColor { get { return players[CurrentPlayerIndex].GetMyColor; } }
    int CurrentPlayerIndex;
    int lastStarterIndex;
    public List<OfflinePlayer> players;


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

        players = new List<OfflinePlayer>();

        
    }

    private void Start()
    {
        RegisterGame();
    }


    public void RegisterGame()
    {
        InputsEnabled = false;
        OfflinePlayer playerOne = new OfflinePlayer(0);
        OfflinePlayer playerTwo = new OfflinePlayer(1);
        players.Add(playerOne);
        players.Add(playerTwo);


        CurrentPlayerIndex = 0;
        lastStarterIndex = CurrentPlayerIndex;


        OfflineRoundOverManager.reset += ResetPlayerOrder;
        //Start countdown
        //
        OfflineCountDownHandler.Instance.StartCountDown();
    }
    public void StartGame()
    {

            InputsEnabled = true;;
            OfflineTurnIndicator.Instance.Show(true);
    }

    public void UpdateTurn()
    {
        CurrentPlayerIndex = CurrentPlayerIndex == players.Count - 1 ? 0 : 1;
        OfflineTurnIndicator.Instance.Show();
    }

    public void ResetPlayerOrder()
    {
        CurrentPlayerIndex = lastStarterIndex == 0 ? 1 : 0;
        lastStarterIndex = CurrentPlayerIndex;
    }

    public void Timeout()
    {
        InputsEnabled = false;
        MarkType winner = MarkType.None;
        switch (GetCurrentType)
        {
            case MarkType.X:
                winner = MarkType.O;
                break;
            case MarkType.O:
                winner = MarkType.X;
                break;
        }

        TimeOut?.Invoke(winner);
    }
    public void RoundOver()
    {
        InputsEnabled = false;
    }
    private void OnDestroy()
    {
        TimeOut = null;
    }
    public class OfflinePlayer
    {

        public int Score;
        public MarkType MyType;
        public Color GetMyColor
        {
            get
            {
                if (MyType == MarkType.X)
                {
                    return new Color32(0, 194, 255, 255);
                }
                else
                {
                    return new Color32(141, 202, 0, 255);
                }
            }
        }

 
        public OfflinePlayer(int index)
        {
            Score = 0;
            MyType = index == 0 ? MarkType.X : MarkType.O;

        }

    }


}
