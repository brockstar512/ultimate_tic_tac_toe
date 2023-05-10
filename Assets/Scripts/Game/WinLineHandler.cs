using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class WinLineHandler : MonoBehaviour
{
    [SerializeField] Color LineColor;
    Image line;
    [SerializeField] Transform LeftVerticalLine;
    [SerializeField] Transform MiddleVerticalLine;
    [SerializeField] Transform RightVerticalLine;
    [SerializeField] Transform TopHorizontalLine;
    [SerializeField] Transform HorizontalLine;
    [SerializeField] Transform BottomHorizontalLine;
    [SerializeField] Transform DiagonalLine;
    [SerializeField] Transform AntiDiagonalLine;
    const float _speed = 2f;
    public static event Action<MarkType> roundOver;

    private void Awake()
    {
        MacroBoardManager.winLine += ConfigLine;
        RoundOverManager.reset += Reset;
    }

    void ConfigLine(WinLineType WinLineType, MarkType MarkType)
    {
        //this might need to be a network object to call the scerver
        if (GameManager.Instance.myPlayer.MyType == MarkType)
        {
            GameManager.Instance.RoundOverStatusServerRpc(MarkType);
        }

        //Debug.Log($"Here is the marktype {MarkType}");
        if (MarkType == MarkType.None)
        {
            roundOver?.Invoke(MarkType);
            return;
        }

        Transform prefab = GetLine(WinLineType);
        line = Instantiate(prefab, this.transform).GetComponent<Image>();
        line.color = LineColor;
        
        StopCoroutine(AnimateLine(MarkType));
        StartCoroutine(AnimateLine(MarkType));
    }

    private Transform GetLine(WinLineType WinLineType)
    {
        Transform line = null;
        switch (WinLineType)
        {
            case WinLineType.Diagonal:
                line = DiagonalLine;
                break;
            case WinLineType.AntiDiagonal:
                line = AntiDiagonalLine;
                break;
            case WinLineType.RowBottom:
                line = BottomHorizontalLine;
                break;
            case WinLineType.RowMiddle:
                line = HorizontalLine;
                break;
            case WinLineType.RowTop:
                line = TopHorizontalLine;
                break;
            case WinLineType.ColLeft:
                line = LeftVerticalLine;
                break;
            case WinLineType.ColMid:
                line = MiddleVerticalLine;
                break;
            case WinLineType.ColRight:
                line = RightVerticalLine;
                break;
            default:
                line = RightVerticalLine;
                break;
        }
        return line;
    }

    IEnumerator AnimateLine(MarkType markType)
    {
        yield return new WaitForSeconds(0.5f);

        while (line.fillAmount < 1f)
        {
            line.fillAmount += _speed * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(.5f);
        roundOver?.Invoke(markType);

    }

    void Reset()
    {
        if(line!= null)
            Destroy(line.gameObject);
    }


    private void OnDestroy()
    {
        MacroBoardManager.winLine -= ConfigLine;
        RoundOverManager.reset -= Reset;
        roundOver = null;
    }
}
