using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;
using Unity.Netcode;

public class RoundOverManager : NetworkBehaviour
{
    public const string Waiting = "Waiting On Opponent";
    public const string Requested = "Opponent Wants To Play Again!";

    //public const string Initial = "Waiting On Opponent";
    [SerializeField] TextMeshProUGUI _promptText;
    [SerializeField] Button _playAgainButton;
    [SerializeField] Button _quitButton;
    [SerializeField] Button _acceptButton;
    CanvasGroup cg;
    public static event Action reset;

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();
        WinLineHandler.roundOver += Init;
        GameManager.Instance.TimeOut += Init;

        _playAgainButton.onClick.AddListener(PlayAgainRequest);
    }

    private void Init()
    {
        cg.DOFade(1,.5f).SetEase(Ease.OutSine);
    }


    [ContextMenu("Reset Button")]
    private void Reset()
    {
        reset?.Invoke();
    }

    void PlayAgainRequest()
    {
        _playAgainButton.interactable = false;
        _playAgainButton.gameObject.SetActive(false);
        _promptText.text = Waiting;
        //change the UI
    }


}
