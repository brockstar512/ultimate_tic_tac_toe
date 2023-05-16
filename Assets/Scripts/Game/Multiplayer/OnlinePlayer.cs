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
    }


    private void Start()
    {
        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }
        else
        {
            Debug.Log($"I am the Owner: {IsClient}");

            //GameManager.
            //    Instance.
            //    myPlayer = this;
        }
        //Debug.Log("I am the Owner");

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

        TurnIndicatorHandler.Instance.ShowTurn();

        GameManager.Instance.myPlayer = this;


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
        TimeManager.Instance.StartTimer(IsMyTurn.Value);
        TurnIndicatorHandler.Instance.ShowTurn();
    }
    public void ForceOff()
    {
        if (!IsOwner)
            return;
        Debug.Log("Forcing player off");
        IsMyTurn.Value = false;
        //TimeManager.Instance.StartTimer(false);
        TimeManager.Instance.StopTimer();
        TurnIndicatorHandler.Instance.Show(false);

    }

    public override void OnNetworkDespawn()
    {
        IsMyTurn.OnValueChanged = null;
    }
}
