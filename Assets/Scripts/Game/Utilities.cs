using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public static class Utilities
{
    public const int GRID_SIZE = 3;

    public static (bool isWin, WinLineType lineType) CheckWin(byte row, byte col, MarkType[,] Grid)
    {
        var type = Grid[row, col];

        //check col
        for (int i = 0; i < GRID_SIZE; i++)
        {
            if (Grid[row, i] != type) break;
            if (i == GRID_SIZE - 1) return (true, ResolveLineTypeRow(row));
        }
        //check row
        for (int i = 0; i < GRID_SIZE; i++)
        {
            if (Grid[i, col] != type) break;
            if (i == GRID_SIZE - 1) return (true, ResolveLineTypeCol(col));
        }

        //check diagnonal
        if (row == col)
        {
            for (int i = 0; i < GRID_SIZE; i++)
            {
                if (Grid[i, i] != type) break;
                if (i == GRID_SIZE - 1) return (true, WinLineType.Diagonal);
            }
        }

        //check ANTI diagnonal (if any of the col + row combines to equal 2)
        //[0,2][1,1][2,0]
        if (row + col == GRID_SIZE - 1)
        {
            //i = 0,1,2
            //[0,2],[1,1],[2,0]
            //if we get to i == 2 we have a match
            for (int i = 0; i < GRID_SIZE; i++)
            {
                if (Grid[i, (GRID_SIZE - 1) - i] != type) break;
                if (i == GRID_SIZE - 1) return (true, WinLineType.AntiDiagonal);
            }
        }

        return (false, WinLineType.None);
    }

    public static WinLineType ResolveLineTypeCol(byte col)
    {
        return (WinLineType)(col + 3);
    }

    public static WinLineType ResolveLineTypeRow(byte row)
    {
        return (WinLineType)(row + 6);
    }
}
