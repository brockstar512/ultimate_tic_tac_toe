using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Enums;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class OnlinePlayer : NetworkBehaviour
{
    public byte namer;
    public byte MyUsername { get; private set; }
    public MarkType MyType { get; private set; }
    public Color GetColor
    {
        get
        {
            if (MyType == MarkType.X)
            {
                return new Color32(0, 194, 255, 255);
            }
            else
            {

                return new Color32(0, 194, 255, 255); ;
            }
        }
    }

    private void Start()
    {
        if (!IsOwner)
            this.enabled = false;
        GameManager.Instance.players.Add(this);
    }

    public void Init(byte xUser)
    {
        if (!IsOwner)
            return;

        if ((int)MarkType.X == (int)xUser)
        {
            MyType = MarkType.X;
            MyUsername = 0;
        }
        else
        {
            MyType = MarkType.O;
            MyUsername = 1;

        }
        namer = MyUsername;
        Debug.Log(MyType);


    }

}
