using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using static Enums;

public class WrapUpHandler : MonoBehaviour
{
    CanvasGroup cg;
    [SerializeField] TextMeshProUGUI YourStatus;
    [SerializeField] TextMeshProUGUI xScore;
    [SerializeField] TextMeshProUGUI oScore;
    const string TIE ="You Tied!";
    const string WIN = "You Won!";
    const string LOSE = "You Lost!";
    const string OPPONENT_LEFT = "Opponent Left!";
    [SerializeField] AudioClip _finishedGame;

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();

    }
    public void Init(int xScore, int oScore, bool didWin, bool didLeaveEarly = false)
    {
        Debug.Log($"initialize wrap up {xScore} and {oScore} and {didWin}");
        SoundManager.Instance.PlaySound(_finishedGame);
        this.transform.localScale = new Vector3(1, 1, 1);
        this.xScore.text = xScore.ToString();
        this.oScore.text = oScore.ToString();
        if (didLeaveEarly)
        {
            YourStatus.text = OPPONENT_LEFT;

        }
        else
        {


            if (didWin)
            {
                YourStatus.text = WIN;
            }
            else if (xScore == oScore)
            {
                YourStatus.text = TIE;
            }
            else
            {
                YourStatus.text = LOSE;
            }
        }


        cg.DOFade(1, .25f).SetEase(Ease.OutSine);
        StopCoroutine(Continue());
        StartCoroutine(Continue());

    }

    public void OpponentLeft()
    {
        TimeManager.Instance.StopTimer();
        var (xVal, oVal) = GameManager.Instance.EndGameStatus();
        bool didWin;
        if ((MarkType)GameManager.Instance.myPlayer.MyType.Value == MarkType.X && xVal > oVal)
        {
            didWin = true;
        }
        else
        {
            didWin = false;
        }

        Init(xVal, oVal, didWin, true);
    }


    IEnumerator Continue()
    {
        //yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(2f);
        LoadingManager.Instance.QuickLoad(Enums.MyScenes.Menu);
        yield return null;
    }
}
