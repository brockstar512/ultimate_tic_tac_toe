using UnityEngine;
using static Enums;
using TMPro;
using System;

public class OfflineScoreKeeper : MonoBehaviour
{
    public static OfflineScoreKeeper Instance { get; private set; }
    [SerializeField] TextMeshProUGUI xScore;
    [SerializeField] TextMeshProUGUI oScore;
    int gameXScore;
    int gameOScore;
    //public static event Action<MarkType> scoreEvent;

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
        gameXScore = 0;
        gameOScore = 0;
        xScore.text = $"{gameXScore}";
        oScore.text = $"{gameOScore}";


    }



    public void RoundOver(MarkType type)
    {
        if(type == MarkType.X)
        {
            gameXScore++;
            xScore.text = $"{gameXScore}";
        }
        else if (type == MarkType.O)
        {
            gameOScore++;
            oScore.text = $"{gameOScore}";
        }
    }

    public (int xVal, int oVal) Outcome()
    {

        return (gameXScore, gameOScore);
    }
}
