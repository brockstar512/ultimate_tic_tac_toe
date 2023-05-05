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
    private Game ActiveGame { get; set; }
    //this is just for validation purposes which is why they should be private
    public MarkType GetMarkType
    {
        get { return players.FirstOrDefault(x => x.MyUsername == CurrentPlayer.Value).MyType; }
    }
    public Color GetColor
    {
        get { return players.FirstOrDefault(x => x.MyUsername == CurrentPlayer.Value).GetColor; }
    }
    public NetworkVariable<bool> InputsEnabled = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//this is global
    public NetworkVariable<byte> CurrentPlayer = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//this is global
    private List<OnlinePlayer> players = new List<OnlinePlayer>();
    public OnlinePlayer myPlayer;

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

    private void Update()
    {
        //Debug.Log(IsMyTurn.Value);
    }
    public void InitializePlayer(OnlinePlayer player)
    {
        players.Add(player);
    }

    public bool IsMyTurn()
    {
        //Debug.Log(players.FirstOrDefault(x => x.MyUsername == CurrentPlayer.Value).IsOwner);
        return players.FirstOrDefault(x =>
        x.MyUsername == CurrentPlayer.Value)
            .IsOwner;
    }

    [ContextMenu("Test")]
    void UpdateTurn()
    {
        UpdateBoardServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]//called by client ran by server
    private void UpdateTurnServerRpc()
    { 
        players[CurrentPlayer.Value].IsMyTurn.Value = false;
        CurrentPlayer.Value = CurrentPlayer.Value == (byte)0 ? (byte)1 : (byte)0;
        players[CurrentPlayer.Value].IsMyTurn.Value = true;
        ActiveGame.SwitchCurrentPlayer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateBoardServerRpc()
    {
        //if the move is not valide return that cell to its state

        //else update other users board and change turn
        UpdateTurnServerRpc();
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
