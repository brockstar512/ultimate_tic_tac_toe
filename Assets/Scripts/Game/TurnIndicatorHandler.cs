using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using static Enums;
using System;

public class TurnIndicatorHandler : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI playerText;
    //public static event Action roundOver;
    // Start is called before the first frame update
    private void Show()
    {
        Color _color = GameManager.Instance.GetColor;

        if (GameManager.Instance.myPlayer.IsMyTurn.Value)
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
    //do i need a hide?
    private void Hide()
    {
        Color _color = GameManager.Instance.GetColor;

        if (GameManager.Instance.myPlayer.IsMyTurn.Value)
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
}
