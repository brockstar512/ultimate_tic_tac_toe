using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class MacroBoardManager : MonoBehaviour
{
    //when a micro board is finished this should read the whole boards

    public MarkType[,] Grid { get; private set; }
    private const int GRID_SIZE = 3;
    private Dictionary<int, MicroBoardManager> _boards;

    private void Awake()
    {
        Grid = new MarkType[GRID_SIZE, GRID_SIZE];
        InitializeMacroBoard();
    }

    void InitializeMacroBoard()
    {
        Grid = new MarkType[GRID_SIZE, GRID_SIZE];

        _boards = new Dictionary<int, MicroBoardManager>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            MicroBoardManager board = this.transform.GetChild(i).GetComponent<MicroBoardManager>();
            board.Init((byte)i);
            _boards.Add(i, board);
        }
        
    }

}
