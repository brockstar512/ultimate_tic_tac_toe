using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OptionSelected : MonoBehaviour
{
    public Button _button;
    public Image _img;
    [SerializeField] Color playerOne;//reference static field instead
    [SerializeField] Color playerTwo;//reference static field instead


    private void Awake()
    {
        _button = GetComponent<Button>();
        _img = GameObject.FindWithTag("Mark").GetComponent<Image>();
        _button.onClick.AddListener(Click);
    }

    void Click()
    {
        _button.onClick.RemoveAllListeners();
        _img.color = playerOne;
        _img.enabled = true;
        _img.transform.DOScale(.90f,.05f).SetEase(Ease.OutBounce);
    }
}
