using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class MacroBoardManager : MonoBehaviour
{
    public MarkType[,] Grid { get; private set; }
    private const int GRID_SIZE = 3;

    void Awake()
    {
    }

    void InitializeMacroBoard()
    {
        Grid = new MarkType[GRID_SIZE, GRID_SIZE];

    }

}
