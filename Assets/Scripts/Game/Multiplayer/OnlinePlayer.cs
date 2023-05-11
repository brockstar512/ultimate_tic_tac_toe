using Unity.Netcode;
using UnityEngine;
using static Enums;


public class OnlinePlayer : NetworkBehaviour
{
    public NetworkVariable<PlayerPacket> playerPacket = new NetworkVariable<PlayerPacket>(
        new PlayerPacket{
            MyType = 0,
            IsMyTurn = false,
        });
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
        playerPacket.Value.IsMyTurn.OnValueChanged += (bool previousValue, bool newVal) =>
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

        TurnIndicatorHandler.Instance.ShowTurn(false);


        if (0 == (int)xUser)
        {
            playerPacket = new NetworkVariable<PlayerPacket>()
            {
                MyType = (byte)MarkType.X,
            };
            OpponentType = MarkType.O;

        }
        else
        {
            MyType.Value = (byte)MarkType.O;
            OpponentType = MarkType.X;

        }
        //namer = MyUsername;
        Debug.Log(MyType);
    }

    public void UpdateTurn()
    {
        if (!IsOwner)
            return;
        IsMyTurn.Value = !IsMyTurn.Value;
    }

    public override void OnNetworkDespawn()
    {
        IsMyTurn.OnValueChanged = null;
    }
    public struct PlayerPacket : INetworkSerializable
    {
        public byte MyType;
        public bool IsMyTurn;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref MyType);
            serializer.SerializeValue(ref IsMyTurn);

        }
    }
}
