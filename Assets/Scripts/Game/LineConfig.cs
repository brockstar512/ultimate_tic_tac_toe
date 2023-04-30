using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class LineConfig : MonoBehaviour
{
    public AngleType angleType;
    public RectTransform holder;
    RectTransform me;

    private void Awake()
    {
        holder = this.transform.parent.GetComponent<RectTransform>();
        me = this.transform.GetComponent<RectTransform>();
        float hypotenuse = (float)Math.Sqrt(holder.rect.width * holder.rect.width + holder.rect.height * holder.rect.height);
        Debug.Log(hypotenuse);//length//yes
        float angle = Mathf.Atan2(holder.rect.width, holder.rect.height) * Mathf.Rad2Deg;
        Debug.Log(angle);//yes
        me.rotation = Quaternion.Euler(0f, 0f, (int)angleType* angle); ;
        me.sizeDelta = new Vector2(me.sizeDelta.x, hypotenuse);
    }
}

