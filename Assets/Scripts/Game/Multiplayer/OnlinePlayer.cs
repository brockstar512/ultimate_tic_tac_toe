using Unity.Netcode;
using UnityEngine;
using static Enums;


public class OnlinePlayer : NetworkBehaviour
{
    public NetworkVariable<byte> MyType = new NetworkVariable<byte>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> IsMyTurn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);
    public MarkType OpponentType { get; private set; }
    public Color GetMyColor
    {
        get
        {
            if ((MarkType)MyType.Value == MarkType.X)
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



    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        base.OnNetworkSpawn();
        IsMyTurn.OnValueChanged += (bool previousValue, bool newVal) =>
        {
            TimeManager.Instance.StartTimer(newVal);
            TurnIndicatorHandler.Instance.ShowTurn(newVal);
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

        //GameManager.Instance.InitializePlayer(this);
    }

    private void Update()
    {
        //Debug.Log(IsMyTurn.Value);
    }

    public void Init(byte xUser)
    {
        if (!IsOwner)
            return;

        TurnIndicatorHandler.Instance.ShowTurn(false);


        if (0 == (int)xUser)
        {
            MyType.Value = (byte)MarkType.X;
            OpponentType = MarkType.O;
        }
        else
        {
            MyType.Value = (byte)MarkType.O;
            OpponentType = MarkType.X;

        }
        //namer = MyUsername;
        Debug.Log((MarkType)MyType.Value);
    }

    public void UpdateTurn()
    {
        //Debug.LogError($"Am I the owner? {IsOwner}");
        if (!IsOwner)
            return;
        IsMyTurn.Value = !IsMyTurn.Value;

        Debug.LogError($"My Status is Update {IsMyTurn.Value}");
        //Debug.Log($"It is my turn {IsMyTurn.Value} for player {(MarkType)MyType.Value}");
    }

    public override void OnNetworkDespawn()
    {
        IsMyTurn.OnValueChanged = null;
    }
}
