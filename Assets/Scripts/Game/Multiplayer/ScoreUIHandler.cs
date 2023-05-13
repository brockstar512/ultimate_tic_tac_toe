using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreUIHandler : NetworkBehaviour
{
    //public static ScoreUIHandler Instance { get; private set; }
    [SerializeField] TextMeshProUGUI xScore;
    [SerializeField] TextMeshProUGUI yScore;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameManager.Instance.xScore.OnValueChanged += (byte previousValue, byte newVal) =>
        {
            xScore.text = newVal.ToString();
        };
        GameManager.Instance.oScore.OnValueChanged += (byte previousValue, byte newVal) =>
        {
            yScore.text = newVal.ToString();
        };
    }
}
