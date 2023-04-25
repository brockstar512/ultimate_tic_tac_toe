using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.UI.Image;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Button))]
public class InspectController : MonoBehaviour
{
    public Vector2 originMarkMax;
    public Vector2 targetMarkMax;
    public Vector3 originMarkMin;
    public Vector2 targetMarkMin;
    const float speed = .25f;
    [SerializeField] Button _button;
    [SerializeField] RectTransform origin;
    [SerializeField] RectTransform target;
    [SerializeField] Button _resetButton;


    private void Awake()
    {
        origin = this.GetComponent<RectTransform>();
        target = GameObject.FindWithTag("MainBoard").GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        originMarkMax = origin.anchorMax;
        originMarkMin = origin.anchorMin;
        targetMarkMax = target.anchorMax;
        targetMarkMin = target.anchorMin;
        _button.onClick.AddListener(Show);
    }

    void Show()
    {
        Debug.Log("Show");
        _button.onClick.RemoveAllListeners();
        this.GetComponent<RectTransform>().DOAnchorMax(targetMarkMax, speed).SetEase(Ease.InOutSine);
        this.GetComponent<RectTransform>().DOAnchorMin(targetMarkMin, speed).SetEase(Ease.InOutSine);
        _button.onClick.AddListener(Return);

        void Return()
        {
            Debug.Log("Return");

            _button.onClick.RemoveAllListeners();
            this.GetComponent<RectTransform>().DOAnchorMax(targetMarkMax, speed).SetEase(Ease.InOutSine);
            this.GetComponent<RectTransform>().DOAnchorMin(targetMarkMin, speed).SetEase(Ease.InOutSine);
            _button.onClick.AddListener(Show);
        }
    }
}
