using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

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
        GameManager.Instance.yScore.OnValueChanged += (byte previousValue, byte newVal) =>
        {
            yScore.text = newVal.ToString();
        };
    }
}
