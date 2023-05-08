using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Enums;


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
    public event Action TimeOut;

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


    [ClientRpc]
    void UpdateAwaitingPlayersBoardClientRpc(byte boardIndex, byte cellIndex)
    {
        //Debug.Log("My boarddoes not need to be updated: "+myPlayer.IsMyTurn.Value);
        if (myPlayer.IsMyTurn.Value)
            return;
        //_boards
        //byte _row = (byte)(cellIndex / 3);
        //byte _col = (byte)(cellIndex % 3);
        //Debug.Log("It would have ran");

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
            BoardCells[cellDictIndex] = GetMarkType;
            UpdateAwaitingPlayersBoardClientRpc(boardIndex, cellIndex);
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
            XUserScore = 0,
            OUserScore = 0,
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
    public void PlayerTimedOutServerRpc()
    {

        RoundOverTimeOutClientRpc();
        Debug.Log($"Time Out");

        //Debug.Log($"This player timed out: {myPlayer.MyType}");
        //        TimeOut?.Invoke(players[CurrentPlayer.Value].MyType);
        //TimeOut?.Invoke();
    }

    [ClientRpc]
    public void RoundOverTimeOutClientRpc()
    {
        //validate board and if that win is legit
        TimeOut?.Invoke();//this should not be a delagte... it should should be a generic round over 

    }

    public (int xVal, int oVal, bool didWin) EndGameStatus()
    {

        return (1,2,true);
    }



 

    public class Game
    {
        public byte XUser { get; set; }
        public byte OUser { get; set; }
        public byte XUserScore { get; set; }
        public byte OUserScore { get; set; }
        public byte CurrentUser { get; set; }
        public byte LastStarter { get; set; }


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
