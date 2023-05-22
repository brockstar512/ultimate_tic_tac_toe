using TMPro;
using Unity.Netcode;
using UnityEngine;
using static Enums;

public class ScoreUIHandler : NetworkBehaviour
{
    public static ScoreUIHandler Instance { get; private set; }
    [SerializeField] TextMeshProUGUI xScore;
    [SerializeField] TextMeshProUGUI yScore;
    [SerializeField] TextMeshProUGUI blueName;
    [SerializeField] TextMeshProUGUI greenName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }

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

    public void ShowPlayersNames(MarkType markType)
    {
        if (markType == MarkType.X)
        {
            blueName.text = "Enemy";
            greenName.text = "You";

        }
        else
        {
            blueName.text = "You";
            greenName.text = "Enemy";
        }

        blueName.gameObject.SetActive(true);
        greenName.gameObject.SetActive(true);
    }
}
