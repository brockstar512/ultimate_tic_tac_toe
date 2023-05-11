using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using static Enums;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public MarkType GetMarkType
    {
        get { return myPlayer.IsMyTurn.Value ? (MarkType)myPlayer.MyType.Value : myPlayer.OpponentType; }
    }
    public Color GetColor
    {
        get { return myPlayer.IsMyTurn.Value ? myPlayer.GetMyColor : myPlayer.GetOpponentColor; }
    }
    public NetworkVariable<bool> InputsEnabled = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//this is global
    public NetworkVariable<byte> CurrentPlayerIndex = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<byte> LastPlayerIndex = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);                                                                                                                                                       
    public OnlinePlayer myPlayer;
    public NetworkVariable<byte> xScore = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<byte> yScore = new NetworkVariable<byte>((byte)0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public event Action<MarkType> TimeOut;
    public Dictionary<int, MarkType> BoardCells { get; private set; }
    private int lastStarterIndex;
    private ulong[] clientList;

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
        if ((MarkType)myPlayer.MyType.Value == MarkType.X && xScore.Value > yScore.Value)
        {
            didWin = true;
        }
        else
        {
            didWin = false;
        }


        return (xScore.Value, yScore.Value, didWin);
    }

    private void Update()
    {


        if (myPlayer != null)
        {
            Debug.Log($"Here is the Mine: {(MarkType)myPlayer.MyType.Value}");
            Debug.Log($"Here is the bool: {InputsEnabled.Value}");


            return;
        }

        //if (!IsServer)
        //{
        //    return;
        //}

        //if (players.Count >= 2)
        //{
        //    Debug.Log($"Here is the first index: {(MarkType)players[0].MyType.Value}");
        //    Debug.Log($"Here is the second index: {(MarkType)players[1].MyType.Value}");

        //}

    }

    //user id will either be 1 or 0
    public void RegisterGame(ulong xUserId, ulong oUserId)
    {
        Debug.Log($"Am I the server {IsServer}");
        if (!IsServer)
            return;
        //Debug.Log("Starting new game");
        //Debug.Log($"Player Count {players.Count}");
        Debug.Log($"creating the game");

        clientList = new ulong[]
        {
            xUserId,
            oUserId
        };

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

        int index = 0;
        ulong playerX = clientList[0];
        ulong playerO = clientList[1];

        CurrentPlayerIndex.Value = 0;
        lastStarterIndex = CurrentPlayerIndex.Value;

        while (index < 2)
        {

            ClientRpcParams rpcParams = default;
            ulong[] singleTarget = new ulong[] { clientList[index] };
            rpcParams.Send.TargetClientIds = singleTarget;

            Debug.Log($"Index of players {singleTarget[0]}");

            //ExampleMethodClientRpc(index, rpcParams);
            RegisterPlayerClientRpc((byte)index, rpcParams);
            index++;
        }

    }


    void UpdateTurnServer()
    {
        if (!IsServer)
            return;


            ClientRpcParams rpcParams = default;
            ulong[] singleTarget = new ulong[] { clientList[CurrentPlayerIndex.Value] };
            Debug.Log($"Starting player index {CurrentPlayerIndex.Value}");
            rpcParams.Send.TargetClientIds = singleTarget;
            UpdateTurnClientRpc(rpcParams);
            //update players turn for next time this runs
            //CurrentPlayerIndex.Value = CurrentPlayerIndex.Value == (byte)0 ? (byte)1 : (byte)0;

       
    }


    bool ValidateTurn(byte playerRequesting)
    {
        if (!IsServer)
            return false;

        if (playerRequesting - 1 == CurrentPlayerIndex.Value)
        {
            Debug.Log($"Player {(MarkType)playerRequesting} is going");
            return true;
        }

        return false;
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
            UpdateTurnServer();//this is board validation
            //i might need to change how the timer interacts with the utrn switching 
        }


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
    public void PlayerTimedOutServerRpc(byte PlayerMarkType)
    {
        MarkType winner = (MarkType)PlayerMarkType == MarkType.X ? MarkType.O : MarkType.X;
        RoundOverStatusServerRpc(winner);
        RoundOverTimeOutClientRpc(winner);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc(byte playerType)
    {
        
        if (ValidateTurn(playerType))
        {
            InputsEnabled.Value = true;
            Debug.Log("Starting the game");
            lastStarterIndex = lastStarterIndex == 0 ? 1 : 0;
            UpdateTurnServer();
        }
    }

    [ClientRpc]
    void UpdateTurnClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("UPDATING MY TURN");
        myPlayer.UpdateTurn();
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
    void RegisterPlayerClientRpc(byte index, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"You have pinged client client {index}");

        myPlayer.Init(index);
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
