using System.Collections.Generic;
using static Enums;
using UnityEngine;
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

        if(type == MarkType.None)
            return (false, WinLineType.None);


        //we are only checking the row we placed at... this should be the micro board
        for (int i = 0; i < GRID_SIZE; i++)
        {
            if (Grid[row, i] != type) break;
            if (i == GRID_SIZE - 1) return (true, ResolveLineTypeRow(row));
        }
        //check col
        for (int i = 0; i < GRID_SIZE; i++)
        {
            //Debug.Log($"this col {i},{col} is type {type}");

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
    // public static bool IsDraw<T>(T Dict) where T : Dictionary<int,T>// where T: Dictionary<int, MonoBehaviour>
    public static bool IsDraw<T>(Dictionary<int, T> board) where T: IBoard
    {
        //this runs for cells to determine if the board can go more
        //this runs for boards to determine if its a draw
        for(int i = 0; i < board.Count; i++)
        {
            //Debug.Log($"is this button interactable? if true the draw wont run  ->{board[i]._button.interactable}");
            if (board[i]._button.interactable)
                return false;
        }
        return true;
    }
}
