using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class MicroBoardManager : MonoBehaviour
{


    public MarkType[,] Grid { get; private set; }
    private const int GRID_SIZE = 3;
    private Button _button;
    private byte _index;
    private byte _row;
    private byte _col;
    private Transform _placements;
    private Dictionary<int, Cell> _cells;

    //when a cell is selected this read its own board to see if the user won this micro board
    //then destroy that boards inspect controller
    public void Init(byte index)
    {
        _placements = this.transform.GetChild(1).GetComponent<Transform>();
        Grid = new MarkType[GRID_SIZE, GRID_SIZE];
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        //_button.onClick.AddListener(CellClicked);
        InitializeMicroBoard();
    }

    void InitializeMicroBoard()
    {
        this.gameObject.name = $"Cell: [{_row},{_col}]";
        _cells = new Dictionary<int, Cell>();

        for (int i = 0; i < _placements.childCount; i++)
        {
            Cell cell = _placements.GetChild(i).GetComponent<Cell>();
            cell.Init((byte)i);
            _cells.Add(i, cell);
        }
    }


}
