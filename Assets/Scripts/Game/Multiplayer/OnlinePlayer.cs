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
    public MarkType OpponentType { get; private set; }
    public Color GetMyColor
    {
        get
        {
            if (MyType == MarkType.X)
            {
                return new Color32(0, 194, 255, 255);
            }
            else
            {

                return new Color32(141, 202, 0, 255); 
            }
        }
    }
    public Color GetOpponentColor
    {
        get
        {
            if (OpponentType == MarkType.X)
            {
                return new Color32(0, 194, 255, 255);
            }
            else
            {

                return new Color32(141, 202, 0, 255);
            }
        }
    }
    public NetworkVariable<bool> IsMyTurn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        IsMyTurn.OnValueChanged += (bool previousValue, bool newVal) =>
        {
            TimeManager.Instance.StartTimer(newVal);
            TurnIndicatorHandler.Instance.Show(newVal);
        };

    }


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
            OpponentType = MarkType.O;
            MyUsername = 0;
            IsMyTurn.Value = true;
        }
        else
        {
            MyType = MarkType.O;
            OpponentType = MarkType.X;
            MyUsername = 1;
            IsMyTurn.Value = false;
            TurnIndicatorHandler.Instance.Show(false);
        }
        //namer = MyUsername;
        Debug.Log(MyType);


    }


    public override void OnNetworkDespawn()
    {
        IsMyTurn.OnValueChanged = null;
    }
}
