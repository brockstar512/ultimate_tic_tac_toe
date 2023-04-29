using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinLineHandler : MonoBehaviour
{
    [SerializeField] Image line;
    [SerializeField] RectTransform rt;
    LineConfig lineConfig;

    void Awake()
    {
        lineConfig = GetComponent<LineConfig>();
        PositionLine();
    }

    void PositionLine()
    {
        var (one, two) = lineConfig.GetBothRect();
        //position it needs to go
        //Vector3 relativePos = 
        lineConfig.GetLength(one, two, rt);
        //size to reach distance
        //rt.sizeDelta = relativePos;
        //rotation to point to it
        //rt.rotation = lineConfig.GetRotation(relativePos);
    }

    System.Collections.IEnumerator AnimateLine()
    {
        yield return new WaitForSeconds(0.5f);

        while (line.fillAmount < 1f)
        {
            line.fillAmount += 2f * Time.deltaTime;
            yield return null;
        }
    }
}
