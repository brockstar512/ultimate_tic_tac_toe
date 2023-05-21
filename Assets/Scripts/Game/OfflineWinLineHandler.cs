using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using System;
public class OfflineWinLineHandler : MonoBehaviour
{
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
        OfflineMacroBoardManager.winLine += ConfigLine;
        OfflineRoundOverManager.reset += Reset;
    }

    void ConfigLine(WinLineType WinLineType)
    {
        Transform prefab = GetLine(WinLineType);
        line = Instantiate(prefab, this.transform).GetComponent<Image>();
        StopCoroutine(AnimateLine());
        StartCoroutine(AnimateLine());
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

    IEnumerator AnimateLine()
    {
        yield return new WaitForSeconds(0.5f);

        while (line.fillAmount < 1f)
        {
            line.fillAmount += _speed * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(.5f);
        roundOver?.Invoke(OfflineGameManager.Instance.GetCurrentType);

    }

    void Reset()
    {
        Destroy(line.gameObject);
    }


    private void OnDestroy()
    {
        OfflineMacroBoardManager.winLine -= ConfigLine;
        OfflineRoundOverManager.reset -= Reset;
        roundOver = null;
    }
}
