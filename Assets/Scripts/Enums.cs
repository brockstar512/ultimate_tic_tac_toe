using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum MarkType
    {
        None = 0,
        X = 1,
        O = 2
    }

    public enum EndGameStatus
    {
        Win = 0,
        Lose = 1,
        Draw = 2
    }

    public enum WinLineType
    {
        None,
        Diagonal,
        AntiDiagonal,
        ColLeft,
        ColMid,
        ColRight,
        RowTop,
        RowMiddle,
        RowBottom,
    }

    public enum AngleType
    {
        
        AntiDiagonal = -1,
        Diagonal = 1,
    }

    public enum MyScenes
    {
        Login,
        Menu,
        OnlineGame,
        LocalGame
    }
}
