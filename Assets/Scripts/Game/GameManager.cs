using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using static Enums;
using static UnityEngine.UIElements.UxmlAttributeDescription;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
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
    public Dictionary<int, MarkType> BoardCells { get; private set; }
    private int lastStarterIndex;

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

        //todo this might need to change for UI reasons with time and time fireing when round is over. 
        if (InputsEnabled.Value)
        {
            players[CurrentPlayerIndex.Value].IsMyTurn.Value = false;
            CurrentPlayerIndex.Value = CurrentPlayerIndex.Value == (byte)0 ? (byte)1 : (byte)0;
            players[CurrentPlayerIndex.Value].IsMyTurn.Value = true;
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

        if (BoardCells[cellDictIndex] != MarkType.None)
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
            BoardCells[cellDictIndex] = GetMarkType;
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

 

        RoundOverManager.reset += Reset;

        //grid size square * board number + cell index
        int cellCount = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (Utilities.GRID_SIZE * Utilities.GRID_SIZE);
        BoardCells = new Dictionary<int, MarkType>();

        while (cellCount > 0)
        {
            //Debug.Log("Cell initialize "+ cellCount);

            BoardCells[cellCount - 1] = MarkType.None;
            cellCount--;
        }

        RegisterPlayerClientRpc();

        CurrentPlayerIndex.Value = 0;
        lastStarterIndex = CurrentPlayerIndex.Value;
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
        int i = 0;
        foreach ( OnlinePlayer p in players)
        {
            Debug.Log($"Here is the players type at index {i} Type: {p.MyType}");
            i++;
        }
        Debug.Log($"The byte I am sending through should be 1 or 2 {myType} the new index should be 0 or 1 its {CurrentPlayerIndex.Value} and the type at that index is {players[CurrentPlayerIndex.Value].MyType} should be 1 if index: 0 and 2 if index: 1");
        //Debug.Log($"We are starting the game we are comparning players type {myType} with the servers type {(int)players[CurrentPlayerIndex.Value].MyType}");//my type should be 1 or 2 shuld never be 0
        //Debug.Log($"This is where I might have to switch the users and make the current users whatever activegame is {CurrentPlayerIndex.Value}");

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
        //Debug.Log($"All clients should recieve this");
        byte index = 0;
        foreach (OnlinePlayer player in players)
        {
            //Debug.Log($"this should run twice");

            player.Init(index);
            index++;
        }

        CountDownHandler.Instance.StartCountDown();
    }

    public void Reset()
    {

        int cellCount = (Utilities.GRID_SIZE * Utilities.GRID_SIZE) * (Utilities.GRID_SIZE * Utilities.GRID_SIZE);
        while (cellCount > 0)
        {
            BoardCells[cellCount - 1] = MarkType.None;
            cellCount--;
        }
        CurrentPlayerIndex.Value = (byte)lastStarterIndex == (byte)0 ? (byte)1 : (byte)0;
        lastStarterIndex = CurrentPlayerIndex.Value;
        Debug.Log($"new current player should be {CurrentPlayerIndex.Value}");
    }

}
