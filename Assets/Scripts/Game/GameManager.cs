using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Enums;


public class GameManager : MonoBehaviour
{

    private const int MaxPlayers = 2;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;

    public static GameManager Instance { get; private set; }
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
        joinButton.onClick.AddListener(StartJoin);
        hostButton.onClick.AddListener(StartHost);

    }

    public void RegisterGame(Guid gameId, string xUser, string oUser)
    {
        Debug.Log("Starting new game");
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
    public void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clienId) =>
        {
            Debug.Log($"ClientID {clienId} has joined");
            if(NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                Debug.Log("Start Game");
                Guid Id = Guid.NewGuid();
                RegisterGame(Id, NetworkManager.Singleton.ConnectedClients[0].ClientId.ToString(), NetworkManager.Singleton.ConnectedClients[1].ClientId.ToString());
            }
        };
    }

    public void StartHost()
    {
        hostButton.interactable = false;

        NetworkManager.Singleton.StartHost();
    }
    public void StartJoin()
    {
        joinButton.interactable = false;

        NetworkManager.Singleton.StartClient();

    }

    private void OnDestroy()
    {
        joinButton.onClick.RemoveAllListeners();
        hostButton.onClick.RemoveAllListeners();
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
