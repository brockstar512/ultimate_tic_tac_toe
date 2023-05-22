using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class OfflineWrapUpHandler : MonoBehaviour
{
    CanvasGroup cg;
    [SerializeField] TextMeshProUGUI Status;
    [SerializeField] TextMeshProUGUI xScore;
    [SerializeField] TextMeshProUGUI oScore;
    const string TIE = "Draw!";
    const string OWIN = "Green Won!";
    const string XWON = "Blue Won!";
    [SerializeField] AudioClip _finishedGame;

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();

    }
    public void Init(int xScore, int oScore)
    {
        SoundManager.Instance.PlaySound(_finishedGame);

        Debug.Log($"initialize wrap up {xScore} and {oScore}");
        //SoundManager.Instance.PlaySound(_finishedGame);
        this.transform.localScale = new Vector3(1, 1, 1);
        if (xScore == oScore)
        {
            Status.text = TIE;
        }
        else if (xScore > oScore)
        {
            Status.text = XWON;
        }
        else
        {
            Status.text = OWIN;
        }
        this.xScore.text = xScore.ToString();
        this.oScore.text = oScore.ToString();



        cg.DOFade(1, .25f).SetEase(Ease.OutSine);
        StopCoroutine(Continue());
        StartCoroutine(Continue());

    }

    IEnumerator Continue()
    {
        //yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(2f);
        LoadingManager.Instance.QuickLoad(Enums.MyScenes.Menu);
        yield return null;
    }
}
