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
    public NetworkVariable<byte> CurrentPlayerIndex = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//this is global
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
        Debug.Log($"Initializing player");

        players.Add(player);
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateTurnServerRpc()
    {
        if (!IsServer)
            return;

        Debug.Log($"Current turn:{this.ActiveGame.currentUserIndex} and shoudl the timer still be running: {InputsEnabled.Value}");
        //todo this might need to change. 
        if (InputsEnabled.Value)
        {
            players[CurrentPlayerIndex.Value].IsMyTurn.Value = false;
            CurrentPlayerIndex.Value = CurrentPlayerIndex.Value == (byte)0 ? (byte)1 : (byte)0;
            players[CurrentPlayerIndex.Value].IsMyTurn.Value = true;
            //players[CurrentPlayer.Value].StartTurn();
            ActiveGame.SwitchCurrentPlayer();
            //Debug.Log($"New turn:{this.ActiveGame.CurrentUser}");
        }
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
            UpdateTurnServerRpc();//i might need to change how the timer interacts with the utrn switching 
        }


    }

    //user id will either be 1 or 0
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
            currentUserIndex = xUserId,
            LastStarterIndex = xUserId,
            BoardCells = new Dictionary<int, MarkType>(),
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

        CurrentPlayerIndex.Value = ActiveGame.currentUserIndex;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RoundOverStatusServerRpc(MarkType winner)
    {
        Debug.Log("We have a winner ");
        InputsEnabled.Value = false;
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
        MarkType winner = players[CurrentPlayerIndex.Value].MyType == MarkType.X ? MarkType.O : MarkType.X;
        RoundOverStatusServerRpc(winner);
        RoundOverTimeOutClientRpc(winner);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc(byte myType)
    {
        Debug.Log($"We are starting the game we are comparning players type {myType} with the servers type {(int)players[CurrentPlayerIndex.Value].MyType}");//my type should be 1 or 2 shuld never be 0
        Debug.Log($"This is where I might have to switch the users and make the current users whatever activegame is {CurrentPlayerIndex.Value}-are these the same->{ActiveGame.currentUserIndex}");

        if (myType != (byte)players[CurrentPlayerIndex.Value].MyType)
            return;

        InputsEnabled.Value = true;
        players[CurrentPlayerIndex.Value].IsMyTurn.Value = true;
    }

    [ClientRpc]
    public void RoundOverTimeOutClientRpc(MarkType winner)
    {
        TimeOut?.Invoke(winner);
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
            Debug.Log($"this should run twice");

            player.Init(index);
            index++;
        }

        CountDownHandler.Instance.StartCountDown();
    }




    public class Game
    {
        public byte XUser { get; set; }
        public byte OUser { get; set; }
        public byte currentUserIndex { get; set; }
        public byte LastStarterIndex { get; set; }
        public Dictionary<int, MarkType> BoardCells { get; set; }

        public void SwitchCurrentPlayer()
        {
            currentUserIndex = GetOpponent(currentUserIndex);
        }

        public void Reset()
        {

            int cellCount = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (Utilities.GRID_SIZE * Utilities.GRID_SIZE);
            while (cellCount > 0)
            {
                BoardCells[cellCount - 1] = MarkType.None;
                cellCount--;
            }

            currentUserIndex = LastStarterIndex == XUser ? OUser : XUser;
            LastStarterIndex = currentUserIndex;

            Debug.Log($"The two users we are making the index eqaul are {OUser} and {XUser} now it eauals {CurrentUserIndex}");
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
