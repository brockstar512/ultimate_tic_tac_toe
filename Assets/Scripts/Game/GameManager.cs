using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using static Enums;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    private Game ActiveGame { get; set; }
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
    public NetworkVariable<byte> xScore = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<byte> yScore = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public event Action<MarkType> TimeOut;

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
        //BoardCells = new Dictionary< int,MarkType>();
        //Debug.Log("Game manager is initiazlized");

    }

    public (int xVal, int oVal, bool didWin) EndGameStatus()
    {

        bool didWin;
        if (myPlayer.MyType == MarkType.X && xScore.Value > yScore.Value)
        {
            didWin = true;
        }
        else
        {
            didWin = false;
        }


        return (xScore.Value, yScore.Value, didWin);
    }

    public void InitializePlayer(OnlinePlayer player)
    {
        players.Add(player);
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateTurnServerRpc()
    {
        if (!IsServer)
            return;

        Debug.Log($"Current turn:{this.ActiveGame.CurrentUser}");
        players[CurrentPlayer.Value].IsMyTurn.Value = false;
        CurrentPlayer.Value = CurrentPlayer.Value == (byte)0 ? (byte)1 : (byte)0;
        players[CurrentPlayer.Value].IsMyTurn.Value = true;
        //players[CurrentPlayer.Value].StartTurn();
        ActiveGame.SwitchCurrentPlayer();
        //Debug.Log($"New turn:{this.ActiveGame.CurrentUser}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateBoardServerRpc(byte boardIndex, byte cellIndex)
    {
        //int board = 0;
        //int cell= 0;
        //int cellDictIndex = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (int)board + (int)cell;
        //Debug.Log($"Checking board {cellDictIndex}");
        int cellDictIndex = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (int)boardIndex + (int)cellIndex;
        if (ActiveGame.BoardCells[cellDictIndex] != MarkType.None)
        {
            //Debug.Log("Reset the circle to what is saved on this grid");
            //todo
            //make sure the UI matches the grid
            //make sure the local grid matches the server grid... or just tell that cell to equal this with a server rpc
            //continue the timer

            //if the move is not valide return that cell to its state
        }
        else
        {
            ActiveGame.BoardCells[cellDictIndex] = GetMarkType;
            UpdateAwaitingPlayersBoardClientRpc(boardIndex, cellIndex);
            UpdateTurnServerRpc();
        }


    }

    
    public void RegisterGame(byte xUserId, byte oUserId)
    {
        Debug.Log($"Am I the server {IsServer}");
        if (!IsServer)
            return;
        //Debug.Log("Starting new game");
        //Debug.Log($"Player Count {players.Count}");
        Debug.Log($"creating the game");

        ActiveGame = new Game
        {
            XUser = xUserId,
            OUser = oUserId,
            XUserScore = 0,
            OUserScore = 0,
            CurrentUser = xUserId,
            LastStarter = xUserId,
            BoardCells = new Dictionary<int, MarkType>(),
            Grid = new MarkType[Utilities.GRID_SIZE, Utilities.GRID_SIZE],
        };

        RoundOverManager.reset += ActiveGame.Reset;

        //grid size square * board number + cell index
        int cellCount = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (Utilities.GRID_SIZE * Utilities.GRID_SIZE);

        while (cellCount > 0)
        {
            //Debug.Log("Cell initialize "+ cellCount);

            ActiveGame.BoardCells[cellCount - 1] = MarkType.None;
            cellCount--;
        }

        RegisterPlayerClientRpc();

        CurrentPlayer.Value = ActiveGame.CurrentUser;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RoundOverStatusServerRpc(MarkType winner)
    {
        //i feel like validation should be done as you are playing otherwise
        //its going to get way to complicated for not a very good reason
        switch (winner)
        {
            case MarkType.X:
                xScore.Value++;
                break;
            case MarkType.O:
                yScore.Value++;
                break;
            case MarkType.None:
                break;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerTimedOutServerRpc()
    {
        MarkType winner = players[CurrentPlayer.Value].MyType == MarkType.X ? MarkType.O : MarkType.X;
        RoundOverTimeOutClientRpc(winner);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc(byte myType)
    {
        Debug.Log($"We are starting the game we are comparning players type {myType} with the servers type {(byte)players[CurrentPlayer.Value].MyType}");
        if (myType != (byte)players[CurrentPlayer.Value].MyType)
            return;

        InputsEnabled.Value = true;
        players[CurrentPlayer.Value].IsMyTurn.Value = true;
    }

    [ClientRpc]
    public void RoundOverTimeOutClientRpc(MarkType winner)
    {
        //Debug.Log($"whose won:  {winner}        {myPlayer.MyType}");

        //validate board and if that win is legit
        TimeOut?.Invoke(winner);//this should not be a delagte... it should should be a generic round over 

    }

    [ClientRpc]
    void UpdateAwaitingPlayersBoardClientRpc(byte boardIndex, byte cellIndex)
    {
        //Debug.Log("My boarddoes not need to be updated: "+myPlayer.IsMyTurn.Value);
        if (myPlayer.IsMyTurn.Value)
            return;

        MacroBoardManager.Instance._boards[boardIndex]._cells[cellIndex].CellClicked();//this is going to run it again for the same player
    }

    [ClientRpc]
    void RegisterPlayerClientRpc()
    {
        Debug.Log($"All clients should recieve this");
        byte index = 0;
        foreach (OnlinePlayer player in players)
        {
            player.Init(index);
            index++;
        }

        StopCoroutine(CountDownHandler.Instance.CountDown());
        StartCoroutine(CountDownHandler.Instance.CountDown());
    }




    public class Game
    {
        public byte XUser { get; set; }
        public byte OUser { get; set; }
        public byte XUserScore { get; set; }
        public byte OUserScore { get; set; }
        public byte CurrentUser { get; set; }
        public byte LastStarter { get; set; }
        public Dictionary<int, MarkType> BoardCells { get; set; }
        public MarkType[,] Grid { get; set; }

        public void SwitchCurrentPlayer()
        {
            CurrentUser = GetOpponent(CurrentUser);
        }

        public void Reset()
        {

            int cellCount = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (Utilities.GRID_SIZE * Utilities.GRID_SIZE);
            while (cellCount > 0)
            {
                BoardCells[cellCount - 1] = MarkType.None;
                cellCount--;
            }

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
        //pass winners board
        public bool ValidateBoard(MarkType[,] Grid)
        {
 
                    return true;
        }

        void UpdateScore()
        {

        }
    }

}
