using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HostUI : MonoBehaviour
{
    [SerializeField] TMP_Text _joinCodeText;
    [SerializeField] Button _copy;
    [SerializeField] Button _cancel;
    [SerializeField] CanvasGroup copyPromptCG;



    private void Awake()
    {
        _cancel.onClick.AddListener(() => Destroy(this.gameObject));
        _copy.onClick.AddListener(Copy);

    }

    public void Init(string hostCode)
    {
        _joinCodeText.text = hostCode;
    }

    void Copy()
    {
        GUIUtility.systemCopyBuffer = _joinCodeText.text;
        copyPromptCG.DOFade(1, 0).OnComplete(() => { copyPromptCG.DOFade(0, 3); });
    }

    private void OnDestroy()
    {
        _copy.onClick.RemoveAllListeners();
        _cancel.onClick.RemoveAllListeners();

    }
}
