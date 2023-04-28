using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public static class Utilities
{
    public const int GRID_SIZE = 3;
    //grid we are looking at.. row and col is the val of the big board
    //row and col should be cel val... grid should also be micro board
    //row and col are currently the mico board
    public static (bool isWin, WinLineType lineType) CheckWin(byte row, byte col, MarkType[,] Grid)
    {
        //get the type we are checking for
        var type = Grid[row, col];
        Debug.Log($"array   {row},{col}");

        //we are only checking the row we placed at... this should be the micro board
        for (int i = 0; i < GRID_SIZE; i++)
        {
            Debug.Log($"col for this one is {Grid[row, i]}");
            if (Grid[row, i] != type) break;
            if (i == GRID_SIZE - 1) return (true, ResolveLineTypeRow(row));
        }
        //check row
        for (int i = 0; i < GRID_SIZE; i++)
        {
            Debug.Log($"row for this one is {Grid[i, col]}");
            if (Grid[i, col] != type) break;
            if (i == GRID_SIZE - 1) return (true, ResolveLineTypeCol(col));
        }

        //check diagnonal
        if (row == col)
        {
            for (int i = 0; i < GRID_SIZE; i++)
            {
                Debug.Log($"diagnonal for this one is {Grid[i, i]}");
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
                Debug.Log($"ANTI diagnonal for this one is {Grid[i, (GRID_SIZE - 1) - i]}");
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
