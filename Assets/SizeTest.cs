using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SizeTest : MonoBehaviour
{
    const float speed = .25f;
    [SerializeField] Button _button;
    [SerializeField] RectTransform origin;
    [SerializeField] RectTransform target;
    public Vector2 originMarkMax;
    public Vector2 targetMarkMax;
    public Vector3 originMarkMin;
    public Vector2 targetMarkMin;
    bool isShowing;
    private void Awake()
    {
        _button = GetComponent<Button>();
        originMarkMax = origin.anchorMax;
        originMarkMin = origin.anchorMin;
        targetMarkMax = target.anchorMax;
        targetMarkMin = target.anchorMin;
        _button.onClick.AddListener(AnimateAnchors);
    }

    [ContextMenu("Animate Anchors")]
    void AnimateAnchors()
    {
        _button.onClick.RemoveAllListeners();
        this.GetComponent< RectTransform >().DOAnchorMax(targetMarkMax, speed).SetEase(Ease.InOutSine);
        this.GetComponent<RectTransform>().DOAnchorMin(targetMarkMin, speed).SetEase(Ease.InOutSine);
        _button.onClick.AddListener(Reverse);


    }
    [ContextMenu("Reverse")]
    void Reverse()
    {
        _button.onClick.RemoveAllListeners();
        this.GetComponent<RectTransform>().DOAnchorMax(originMarkMax, speed).SetEase(Ease.InOutSine);
        this.GetComponent<RectTransform>().DOAnchorMin(originMarkMin, speed).SetEase(Ease.InOutSine);
        _button.onClick.AddListener(AnimateAnchors);
    }
}
//anchors... moving the anchors