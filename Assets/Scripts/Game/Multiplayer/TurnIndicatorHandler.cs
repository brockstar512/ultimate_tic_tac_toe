using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using static Enums;
using System;

public class TurnIndicatorHandler : MonoBehaviour
{
    public static TurnIndicatorHandler Instance { get; private set; }
    [SerializeField] TextMeshProUGUI playerText;
    CanvasGroup cg;

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
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
    }

    public void Show(bool isOn)
    {
        //Debug.Log("Showing show");
        Color _color = GameManager.Instance.GetColor;
        playerText.color = _color;
        cg.alpha = isOn ? 1 : 0;
        Pulse();
    }

    public void ShowTurn(bool isMyTurn)
    {
        //Debug.Log("Showing header turn");
        Color _color = GameManager.Instance.GetColor;

        if (isMyTurn)
        {
            playerText.text = "Your";
        }
        else
        {
            playerText.text = "Enemies";
        }

        playerText.color = _color;
        playerText.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .15f).SetEase(Ease.InSine).OnComplete(() => playerText.transform.DOScale(new Vector3(1, 1, 1), .15f).SetEase(Ease.InSine));

    }

    public void Pulse()
     {
        playerText.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .15f).SetEase(Ease.InSine).OnComplete(() => playerText.transform.DOScale(new Vector3(1, 1, 1), .15f).SetEase(Ease.InSine));
     }
}
