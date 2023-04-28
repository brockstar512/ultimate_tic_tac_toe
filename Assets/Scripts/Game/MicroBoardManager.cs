using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using System;

public class MicroBoardManager : MonoBehaviour
{


    public MarkType[,] Grid { get; private set; }
    private Button _button;
    private byte _index;
    private byte _row;
    private byte _col;
    private Transform _placements;
    private Dictionary<int, Cell> _cells;
    //public event Action<bool> retireBoard;//this might just go with markBoard
    public event Action<int, int> markBoard;


    //when a cell is selected this read its own board to see if the user won this micro board
    //then destroy that boards inspect controller
    public void Init(byte index)
    {
        _placements = this.transform.GetChild(1).GetComponent<Transform>();
        Grid = new MarkType[Utilities.GRID_SIZE, Utilities.GRID_SIZE];
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        InitializeMicroBoard();
    }

    void InitializeMicroBoard()
    {
        this.gameObject.name = $"Board: [{_row},{_col}]";
        _cells = new Dictionary<int, Cell>();

        for (int i = 0; i < _placements.childCount; i++)
        {
            Cell cell = _placements.GetChild(i).GetComponent<Cell>();
            cell.Init((byte)i);
            _cells.Add(i, cell);
            cell.onCellSelected += ReadBoard;
            cell.markCell += MarkCell;

        }
    }

    void ReadBoard()
    {

        Debug.Log($"Here is the board we are reading {this.gameObject.name}");
        var (isWin, lineType) = Utilities.CheckWin(_row, _col, Grid);//this should be the 
        if (isWin)
        {
            _button.interactable = false;
            Debug.Log("This board is done");
            //invoke line controller
            //retireBoard?.Invoke(false);
        }
        else
        {
            Debug.Log("you can keep going");

        }
    }

    void MarkCell(int row, int col)
    {
        //Debug.Log($"Marking cell for grid {gameObject.name}");
        Grid[row, col] = MarkType.X;

        //for (int x = 0; x < 3; x++)
        //{

        //    for (int y = 0; y < 3; y++)
        //    {

        //        //Acces the array like this

        //        Debug.Log($" here is what is at the matrix  {Grid[x, y]}");



        //    }
        //}
    }



    //void UpdateBoard(Net_OnMarkCell msg)
    //{
    //    _cells[msg.Index].UpdateUI(msg.Actor);
    //}

}
