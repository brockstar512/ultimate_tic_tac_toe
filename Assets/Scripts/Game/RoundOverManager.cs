using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
public class RoundOverManager : MonoBehaviour
{
    [SerializeField] Button _playAgainButton;
    [SerializeField] Button _quitButton;
    [SerializeField] Button _acceptButton;
    CanvasGroup cg;
    public static event Action reset;

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();
        WinLineHandler.roundOver += Init;
        _playAgainButton.onClick.AddListener(Reset);
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


}
