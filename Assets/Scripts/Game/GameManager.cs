using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static Enums;
using System.Text.RegularExpressions;
using static UnityEngine.UIElements.UxmlAttributeDescription;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public MarkType MyType { get; set; }
    public MarkType OpponentType { get; set; }
    public Game ActiveGame { get; private set; }
    //public byte xUser { get; set; }
    //public byte oUser { get; set; }
    public MarkType GetMarkType
    {
        get { return players.FirstOrDefault(x => x.MyUsername == CurrentPlayer.Value).MyType; }
    }
    public Color GetColor
    {
        get { return players.FirstOrDefault(x => x.MyUsername == CurrentPlayer.Value).GetColor; }
    }
    public NetworkVariable<bool> InputsEnabled = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//this is global
    public NetworkVariable<bool> IsMyTurn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);//this is global
    public NetworkVariable<byte> CurrentPlayer = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//this is global
    public List<OnlinePlayer> players = new List<OnlinePlayer>();
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
    }

    ////this only runs on the server... it will not return anything
    //[ServerRpc(RequireOwnership = false)]//called by client ran by server
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Debug.Log("P Pressed");
        //    IsMyTurn.Value = !IsMyTurn.Value;
        //}
        //Debug.Log(IsMyTurn.Value);
        //Debug.Log(CurrentPlayer.Value);
    }

    [ServerRpc(RequireOwnership = false)]//called by client ran by server
    public void UpdateTurnServerRpc()
    {
        ActiveGame.SwitchCurrentPlayer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateBoardServerRpc()
    {
        //if the move is not valide return that cell to its state
    }

    [ClientRpc]
    void RegisterPlayerClientRpc()
    {
        byte index = 0;
        foreach(OnlinePlayer player in players)
        {
            player.Init(index);
            index++;
        }
    }

    public void RegisterGame(byte xUserId, byte oUserId)
    {
        Debug.Log("Starting new game");
        Debug.Log($"Player Count {players.Count}");
        ActiveGame = new Game
        {
            XUser = xUserId,
            OUser = oUserId,
            StartTime = DateTime.Now,
            CurrentUser = xUserId,
            LastStarter = xUserId
        };
        //this could be in people

        RegisterPlayerClientRpc();

        CurrentPlayer.Value = ActiveGame.CurrentUser;
    }




    public class Game
    {
        public Guid? Id { get; set; }
        public byte XUser { get; set; }
        public byte OUser { get; set; }
        public byte CurrentUser { get; set; }
        public byte LastStarter { get; set; }
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

        public MarkType GetPlayerType(byte userID)
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

        private byte GetOpponent(byte currentUser)
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