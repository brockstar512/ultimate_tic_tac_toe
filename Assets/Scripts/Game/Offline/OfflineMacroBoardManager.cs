using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class OfflineMacroBoardManager : MonoBehaviour
{
    //when a micro board is finished this should read the whole boards

    public MarkType[,] Grid { get; private set; }
    CanvasGroup cg;
    private Dictionary<int, OfflineMicroBoardManager> _boards;
    public static event Action<WinLineType, MarkType> winLine;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        Grid = new MarkType[Utilities.GRID_SIZE, Utilities.GRID_SIZE];
        InitializeMacroBoard();
        OfflineRoundOverManager.reset += Reset;
    }

    void InitializeMacroBoard()
    {
        Grid = new MarkType[Utilities.GRID_SIZE, Utilities.GRID_SIZE];

        _boards = new Dictionary<int, OfflineMicroBoardManager>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            OfflineMicroBoardManager board = this.transform.GetChild(i).GetComponent<OfflineMicroBoardManager>();
            board.Init((byte)i);
            _boards.Add(i, board);
            board.markBoard += MarkBoard;
        }
    }

    void MarkBoard(int row, int col, MarkType markType)
    {
        //Debug.Log($"Marking cell for grid {gameObject.name}");
        Grid[row, col] = markType;
        //GameManager.Instance.ActiveGame.GetPlayerType(actor);

        //Debug.Log($"Here is the board we are reading {this.gameObject.name}");
        var (isOver, lineType) = Utilities.CheckWin((byte)row, (byte)col, Grid);
        Debug.Log($"Line Type: {lineType}");

        if (isOver)
        {
            //_boards.interactable = false;
            Debug.Log($"Game is over");
            //do whatever animations you need
            winLine?.Invoke(lineType, OfflineGameManager.Instance.GetCurrentType);
            cg.blocksRaycasts = false;

        }
        else if (Utilities.IsDraw(_boards))
        {
            Debug.Log("Is Draw!");

            winLine?.Invoke(WinLineType.None, MarkType.None);
            cg.blocksRaycasts = false;
        }
        else
        {
            Debug.Log("you can keep going");

        }
    }


    private void Reset()
    {
        for (int col = 0; col < Grid.GetLength(0); col++)
        {
            for (int row = 0; row < Grid.GetLength(1); row++)
            {
                Grid[row, col] = MarkType.None;
            }
        }
        cg.blocksRaycasts = true;
    }


    private void OnDestroy()
    {
        OfflineRoundOverManager.reset -= Reset;
    }
}
