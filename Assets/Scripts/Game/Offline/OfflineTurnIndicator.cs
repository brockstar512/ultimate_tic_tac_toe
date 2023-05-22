using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;

public class OfflineTurnIndicator : MonoBehaviour
{
    public static OfflineTurnIndicator Instance { get; private set; }
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

    public void Show(bool isOn = true)
    {

        Color _color = OfflineGameManager.Instance.GetColor;
        if (OfflineGameManager.Instance.GetCurrentType == Enums.MarkType.X)
        {
            playerText.text = "Blue";
        }
        else
        {
            playerText.text = "Green";
        }
        playerText.color = _color;
        cg.alpha = isOn ? 1 : 0;
        Pulse();
        if (isOn)
        {
            OfflineTimeManager.
                Instance.
                StartTimer();
        }
    }



    public void Pulse()
    {
        playerText.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .15f).SetEase(Ease.InSine).OnComplete(() => playerText.transform.DOScale(new Vector3(1, 1, 1), .15f).SetEase(Ease.InSine));
    }
}
