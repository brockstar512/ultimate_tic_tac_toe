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
using System.Reflection;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    private Game ActiveGame { get; set; }
    //this is just for validation purposes which is why they should be private
    public MarkType GetMarkType
    {
        get { return myPlayer.IsMyTurn.Value ? myPlayer.MyType : myPlayer.OpponentType; }
    }
    public Color GetColor
    {
        get { return myPlayer.IsMyTurn.Value ? myPlayer.GetMyColor : myPlayer.GetOpponentColor; }
    }
    public NetworkVariable<bool> InputsEnabled = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//this is global
    public NetworkVariable<byte> CurrentPlayer = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//this is global
    private List<OnlinePlayer> players = new List<OnlinePlayer>();
    public OnlinePlayer myPlayer;
    private Dictionary<int, MarkType> BoardCells;
    public NetworkVariable<byte> xScore = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<byte> yScore = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
        BoardCells = new Dictionary< int,MarkType>();
        Debug.Log("Game manager is initiazlized");
    }


    public void InitializePlayer(OnlinePlayer player)
    {
        players.Add(player);
    }


    [ClientRpc]//called by server ran on client
    void UpdateAwaitingPlayersBoardClientRpc(byte boardIndex, byte cellIndex)
    {
        Debug.Log("My boarddoes not need to be updated: "+myPlayer.IsMyTurn.Value);
        if (myPlayer.IsMyTurn.Value)
            return;
        //_boards
        //byte _row = (byte)(cellIndex / 3);
        //byte _col = (byte)(cellIndex % 3);
        Debug.Log("It would have ran");

        //return;
        MacroBoardManager.Instance._boards[boardIndex]._cells[cellIndex].CellClicked();//this is going to run it again for the same player

        //UpdateTurnServerRpc();
    }

    
    void UpdateTurn()
    {
        if (!IsServer)
            return;

        players[CurrentPlayer.Value].IsMyTurn.Value = false;
        CurrentPlayer.Value = CurrentPlayer.Value == (byte)0 ? (byte)1 : (byte)0;
        players[CurrentPlayer.Value].IsMyTurn.Value = true;
        //players[CurrentPlayer.Value].StartTurn();
        ActiveGame.SwitchCurrentPlayer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateBoardServerRpc(byte boardIndex, byte cellIndex)
    {
        //int board = 0;
        //int cell= 0;
        //int cellDictIndex = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (int)board + (int)cell;
        //Debug.Log($"Checking board {cellDictIndex}");
        int cellDictIndex = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (int)boardIndex + (int)cellIndex;
        if (BoardCells[cellDictIndex] != MarkType.None)
        {
            Debug.Log("Reset the circle to what is saved on this grid");
            //todo
            //make sure the UI matches the grid
            //make sure the local grid matches the server grid... or just tell that cell to equal this with a server rpc
            //continue the timer

            //if the move is not valide return that cell to its state
        }
        else
        {
            //else update other users board and change turn
            BoardCells[cellDictIndex] = GetMarkType;
            UpdateAwaitingPlayersBoardClientRpc(boardIndex, cellIndex);
            //
            UpdateTurn();
        }


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
        if (!IsServer)
            return;
        Debug.Log("Starting new game");
        Debug.Log($"Player Count {players.Count}");
        ActiveGame = new Game
        {
            XUser = xUserId,
            OUser = oUserId,
            StartTime = DateTime.Now,
            CurrentUser = xUserId,
            LastStarter = xUserId,
    };

        //grid size square * board number + cell index
        int cellCount = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (Utilities.GRID_SIZE* Utilities.GRID_SIZE);

        while(cellCount > 0)
        {
            //Debug.Log("Cell initialize "+ cellCount);
            
            BoardCells[cellCount - 1] = MarkType.None;
            cellCount--;
        }

        RegisterPlayerClientRpc();

        CurrentPlayer.Value = ActiveGame.CurrentUser;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerTimedOutRpc()
    {
        //either use default params
        //Or
        //just use the active game class
    }


    public class Game
    {
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

        public bool ValidateBoard()
        {
            return true;
        }
    }

}
