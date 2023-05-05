using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Enums;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class OnlinePlayer : NetworkBehaviour
{
    //public byte namer;
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
    public NetworkVariable<bool> IsMyTurn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);

    private void Start()
    {
        if (!IsOwner)
        {
            this.enabled = false;
        }
        else
        {
            GameManager.Instance.myPlayer = this;
        }
        //if (!IsOwner)
        //{
        //    this.enabled = false;
        //    return;
        //    this was vausing an issue for initializing the init ismyturn variable. I think it was forcing the client to run the var
        //}


        GameManager.Instance.InitializePlayer(this);
    }

    private void Update()
    {
        //Debug.Log(IsMyTurn.Value);
    }

    public void Init(byte xUser)
    {
        if (!IsOwner)
            return;

        if (0 == (int)xUser)
        {
            MyType = MarkType.X;
            MyUsername = 0;
            IsMyTurn.Value = true;
        }
        else
        {
            MyType = MarkType.O;
            MyUsername = 1;
            IsMyTurn.Value = false;
        }
        //namer = MyUsername;
        Debug.Log(MyType);


    }

}
