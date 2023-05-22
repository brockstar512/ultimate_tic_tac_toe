using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class OfflineRoundOverManager : MonoBehaviour
{
    [SerializeField] Image _banner;
    [SerializeField] TextMeshProUGUI _header;
    [SerializeField] OfflineWrapUpHandler _wrapUpHandler;
    [SerializeField] Button _playAgainButton;
    [SerializeField] Button _quitButton;
    CanvasGroup cg;
    const string TIE = "Round Tied!";
    const string OWin = "O Won!";
    const string XWin = "X Won!";
    public Color tie_Color;
    private Color32 x_Color = new Color32(0, 194, 255, 255);
    private Color32 o_Color = new Color32(141, 202, 0, 255);

    public static event Action reset;

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();
        OfflineWinLineHandler.roundOver += Init;
        OfflineGameManager.TimeOut += Init;
        _playAgainButton.onClick.AddListener(ResetGame);
        _quitButton.onClick.AddListener(Quit);

    }

    private void Init(MarkType markType)
    {
        cg.DOFade(1,.5f).SetEase(Ease.OutSine);
        //SoundManager.Instance.PlaySound(roundOverSoundFX);
        OfflineScoreKeeper.Instance.RoundOver(markType);
        //OfflineGameManager.Instance.myPlayer.ForceOff();
        _playAgainButton.gameObject.SetActive(true);
        _quitButton.gameObject.SetActive(true);

        SetUI(markType);
        cg.interactable = true;
        cg.blocksRaycasts = true;
        _playAgainButton.interactable = true;
        _quitButton.interactable = true;
    }





    void SetUI(MarkType markType)
    {
        if (markType == MarkType.X)
        {
            _header.text = XWin;
            _banner.color = x_Color;
        }
        else if (markType == MarkType.O)
        {
            _header.text = OWin;
            _banner.color = o_Color;
        }
        else
        {
            _header.text = TIE;
            _banner.color = tie_Color;
        }

    }


    private void Quit()
    {
        _quitButton.interactable = false;
        var (xVal, oVal) = OfflineScoreKeeper.Instance.Outcome();
         _wrapUpHandler.Init(xVal, oVal);
    }

  
    void ResetGame()
    {
        _playAgainButton.interactable = false;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.DOFade(0, .15f).SetEase(Ease.OutSine);
        OfflineTimeManager.Instance.gameObject.SetActive(true);
        reset?.Invoke();
    }



    public void OnDestroy()
    {
        _playAgainButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
        reset = null;

    }

}
