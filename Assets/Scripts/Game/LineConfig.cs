using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static UnityEngine.GraphicsBuffer;

public class LineConfig : MonoBehaviour
{
    [SerializeField] RectTransform rUpCorner;
    [SerializeField] RectTransform lUpCorner;
    [SerializeField] RectTransform rMiddleCorner;
    [SerializeField] RectTransform lMiddleCorner;
    [SerializeField] RectTransform rDownCorner;
    [SerializeField] RectTransform lDownCorner;
    [SerializeField] RectTransform midDownCorner;
    [SerializeField] RectTransform midUpCorner;
    Transform target;


    public Quaternion GetRotation(Vector3 relativePos)
    {
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        return rotation;

    }

    

    public Vector2 GetLength(RectTransform start, RectTransform finish, RectTransform image)
    {

        //Vector3 relativePos = finish.position - start.position;
        Vector2 relativePos = (finish.anchoredPosition - start.anchoredPosition).normalized; ;
        //Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(start.anchoredPosition, finish.anchoredPosition);
        image.anchorMin = new Vector2(0, 1);
        image.anchorMax = new Vector2(0, 1);
        image.sizeDelta = new Vector2(distance, 10f);
        image.anchoredPosition = start.anchoredPosition + relativePos * distance * .5f;
        image.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(relativePos));
        // the second argument, upwards, defaults to Vector3.up
        return relativePos;
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public (RectTransform from, RectTransform to) GetBothRect()
    {
        return (rUpCorner, lDownCorner);
    }
}

