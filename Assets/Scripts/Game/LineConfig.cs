using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static UnityEngine.GraphicsBuffer;

public class LineConfig : MonoBehaviour
{
    [SerializeField] RectTransform holder;
    [SerializeField] RectTransform me;
    private void Awake()
    {
        Debug.Log(holder.rect.yMax);
        Debug.Log(holder.rect.xMax);
        float val = (holder.rect.xMax- holder.rect.yMax);
        Debug.Log(me.rect.width);//base
        Debug.Log(me.rect.height);//height
        float hyp = (float)Math.Sqrt(holder.rect.width * holder.rect.width + holder.rect.height * holder.rect.height);
        Debug.Log(hyp);//length//yes
        //float arc = Mathf.Asin((hyp / holder.rect.height)); //Atan2(float y, float x)
        float arc = Mathf.Atan2(holder.rect.width, holder.rect.height);
        Debug.Log(arc * Mathf.Rad2Deg);//yes
    }
}

