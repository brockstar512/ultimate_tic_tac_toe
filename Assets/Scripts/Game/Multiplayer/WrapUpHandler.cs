using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class WrapUpHandler : MonoBehaviour
{
    CanvasGroup cg;
    [SerializeField] TextMeshProUGUI YourStatus;
    [SerializeField] TextMeshProUGUI xScore;
    [SerializeField] TextMeshProUGUI oScore;
    const string TIE ="You Tied!";
    const string WIN = "You Lost!";
    const string LOSE = "You Won!";

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();

    }
    public void Init(int xScore, int oScore, bool didWin)
    {
        Debug.Log($"initialize wrap up");

        this.transform.localScale = new Vector3(1, 1, 1);
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
        this.xScore.text = xScore.ToString();
        this.oScore.text = oScore.ToString();



        cg.DOFade(1, .25f).SetEase(Ease.OutSine);
        StopCoroutine(Continue());
        StartCoroutine(Continue());

    }

    IEnumerator Continue()
    {
        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(3f);
        LoadingManager.Instance.QuickLoad(Enums.MyScenes.Menu);
        yield return null;
    }
}
