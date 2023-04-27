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

    public void Init(byte index)
    {
        Grid = new MarkType[GRID_SIZE, GRID_SIZE];
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        //_button.onClick.AddListener(CellClicked);
    }

    void InitializeMicroBoard()
    {

    }


}
