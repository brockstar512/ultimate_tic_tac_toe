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
        Diagonal = -1,
        AntiDiagonal = 1,
    }
}
