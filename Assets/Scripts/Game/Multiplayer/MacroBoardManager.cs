using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class MacroBoardManager : MonoBehaviour
{
    //when a micro board is finished this should read the whole boards
    public static MacroBoardManager Instance { get; private set; }
    public MarkType[,] Grid { get; private set; }
    public Dictionary<int, MicroBoardManager> _boards { get; private set; }
    public static event Action<WinLineType, MarkType> winLine;
    private CanvasGroup cg;

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
    

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
        Grid = new MarkType[Utilities.GRID_SIZE, Utilities.GRID_SIZE];
        InitializeMacroBoard();
        RoundOverManager.reset += Reset;
    }

    void InitializeMacroBoard()
    {
        Grid = new MarkType[Utilities.GRID_SIZE, Utilities.GRID_SIZE];

        _boards = new Dictionary<int, MicroBoardManager>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            MicroBoardManager board = this.transform.GetChild(i).GetComponent<MicroBoardManager>();
            board.Init((byte)i);
            _boards.Add(i, board);
            board.markBoard += MarkBoard;
        }
    }

    void MarkBoard(int row, int col, MarkType markType)
    {
        //i think its probably becase when the bpard is marked we do not chnge the button here
        //Debug.Log($"MACROBOARD CHECK");
        // GameManager.Instance.GetMarkType
        //Debug.Log($"Marking cell for grid {gameObject.name}");
        Grid[row, col] = markType;//this needs to be an argument
        //GameManager.Instance.ActiveGame.GetPlayerType(actor);

        //Debug.Log($"Here is the board we are reading {this.gameObject.name}");
        var (isOver, lineType) = Utilities.CheckWin((byte)row, (byte)col, Grid);
        //Debug.Log($"Line Type: {lineType}");

        if (isOver)
        {
            //_boards.interactable = false;
            //Debug.Log($"Game is over");
            //do whatever animations you need
            winLine?.Invoke(lineType, Grid[row, col]);
            cg.blocksRaycasts = false;


            //win
        }
        else if(Utilities.IsDraw(_boards))
        {
            //Debug.Log("This match is a draw");

            winLine?.Invoke(WinLineType.None, MarkType.None);
            cg.blocksRaycasts = false;


        }

    }



    private void Reset()
    {
        for (int col = 0; col < Grid.GetLength(0); col++)
        {
            for (int row = 0; row < Grid.GetLength(1); row++)
            {
                _boards = new Dictionary<int, MicroBoardManager>();
                Grid[row, col] = MarkType.None;
            }
        }
        cg.blocksRaycasts = true;

    }


    private void OnDestroy()
    {
        RoundOverManager.reset -= Reset;
    }
}
