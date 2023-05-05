using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.UI.Image;
using static UnityEngine.GraphicsBuffer;
using Unity.Netcode;

[RequireComponent(typeof(Button))]
public class InspectController : MonoBehaviour
{
    Vector2 originMarkMax;
    Vector2 targetMarkMax;
    Vector3 originMarkMin;
    Vector2 targetMarkMin;
    const float speed = .15f;
    Button _button;
    RectTransform origin;
    RectTransform target;
    Button _resetButton;
    CanvasGroup _resetCanvasGroup;
    [SerializeField]CanvasGroup _placements;
    //OnlinePlayer _player;

    void Awake()
    {
        origin = this.GetComponent<RectTransform>();
        target = GameObject.FindWithTag("MainBoard").GetComponent<RectTransform>();
        _resetButton = GameObject.FindWithTag("ResetButton").GetComponent<Button>();
        _resetCanvasGroup = _resetButton.GetComponent<CanvasGroup>();
        _resetCanvasGroup.alpha = 0;
        _button = GetComponent<Button>();
        originMarkMax = origin.anchorMax;
        originMarkMin = origin.anchorMin;
        targetMarkMax = target.anchorMax;
        targetMarkMin = target.anchorMin;
        _button.onClick.AddListener(Show);
        HandleMarks(false);
    }

    void Start()
    {
        HandleSubscription(true);
    }

    void OnDestroy()
    {
        HandleSubscription(false);
    }

    //public void Init(OnlinePlayer player)
    //{
    //    _player = player;
    //}

    void Show()
    {
        //if (!GameManager.Instance.InputsEnabled)// wait for UI to update
        if (!GameManager.Instance.myPlayer.IsMyTurn.Value)
            return;

        this.transform.SetAsLastSibling();
        _button.onClick.RemoveAllListeners();
        _button.enabled = false;
        this.GetComponent<RectTransform>().DOAnchorMax(targetMarkMax, speed).SetEase(Ease.InOutSine);
        this.GetComponent<RectTransform>().DOAnchorMin(targetMarkMin, speed).SetEase(Ease.InOutSine);
        HandleMarks(true);
        _resetButton.onClick.AddListener(Return);
        _resetCanvasGroup.DOFade(1, speed);
    }

    void Return()
    {
        //Debug.Log("Return");
        _resetCanvasGroup.DOFade(0, speed);
        _resetButton.onClick.RemoveAllListeners();
        _button.enabled = true;
        this.GetComponent<RectTransform>().DOAnchorMax(originMarkMax, speed).SetEase(Ease.InOutSine);
        this.GetComponent<RectTransform>().DOAnchorMin(originMarkMin, speed).SetEase(Ease.InOutSine);
        HandleMarks(false);
        _button.onClick.AddListener(Show);
    }

    void HandleMarks(bool isEnabled)
    {
        _placements.blocksRaycasts = isEnabled;
    }

    void HandleSubscription(bool subscribe)
    {
        for (int i = 0; i < _placements.transform.childCount; i++)
        {
            switch (subscribe) {
                case true:
                    this.GetComponent<MicroBoardManager>().onCellSelected += Return;
                    break;
                case false:
                    this.GetComponent<MicroBoardManager>().onCellSelected -= Return;
                    break;
            }
        }
    }
}
