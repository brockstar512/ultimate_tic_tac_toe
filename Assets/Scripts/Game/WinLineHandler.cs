using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinLineHandler : MonoBehaviour
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


    private void Awake()
    {
        ConfigLine();
    }

    void ConfigLine()
    {
        line = Instantiate(BottomHorizontalLine, this.transform).GetComponent<Image>();
        StopCoroutine(AnimateLine());
        StartCoroutine(AnimateLine());
    }

    IEnumerator AnimateLine()
    {
        yield return new WaitForSeconds(0.5f);

        while (line.fillAmount < 1f)
        {
            line.fillAmount += _speed * Time.deltaTime;
            yield return null;
        }
    }
}
