using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Enums;

public class OfflineGameManager : MonoBehaviour
{
    public static OfflineGameManager Instance { get; private set; }
    public MarkType MyType { get; set; }
    public MarkType OpponentType { get; set; }
    public Game ActiveGame { get; private set; }
    public bool InputsEnabled { get; set; }
    public string MyUsername { get; set; }
    public string OpponentUsername { get; set; }
    public bool IsMyTurn
    {
        get
        {
            if (ActiveGame == null)
            {
                return false;
            }
            if (ActiveGame.CurrentUser != MyUsername)
            {
                return false;
            }
            return true;
        }
    }
    public Color GetColor
    {
        get
        {
            if (MyType == MarkType.X)
            {
                return new Color32(0,194,255,255);
            }
            else
            {

                return new Color32(0, 194, 255, 255); ;
            }
        }
    }


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
        MyType = MarkType.X;
    }

    public void RegisterGame(Guid gameId, string xUser, string oUser)
    {
        ActiveGame = new Game
        {
            Id = gameId,
            XUser = xUser,
            OUser = oUser,
            StartTime = DateTime.Now,
            CurrentUser = xUser,
            LastStarter = xUser
        };

        if (MyUsername == xUser)
        {
            MyType = MarkType.X;
            OpponentUsername = oUser;
            OpponentType = MarkType.O;
        }
        else
        {
            MyType = MarkType.O;
            OpponentUsername = xUser;
            OpponentType = MarkType.X;

        }
        InputsEnabled = true;

    }

    public class Game
    {
        public Guid? Id { get; set; }
        public string XUser { get; set; }
        public string OUser { get; set; }
        public string CurrentUser { get; set; }
        public string LastStarter { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void SwitchCurrentPlayer()
        {
            CurrentUser = GetOpponent(CurrentUser);
        }

        public void Reset()
        {
            //here
            //CurrentUser = XUser;
            CurrentUser = LastStarter == XUser ? OUser : XUser;
            LastStarter = CurrentUser;
        }

        public MarkType GetPlayerType(string userID)
        {
            if (userID == XUser)
            {
                return MarkType.X;
            }
            else
            {
                return MarkType.O;
            }
        }

        private string GetOpponent(string currentUser)
        {
            if (currentUser == XUser)
            {
                return OUser;
            }
            else
            {
                return XUser;
            }
        }
    }

}
