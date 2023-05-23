using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class JoinUI : MonoBehaviour
{
    [SerializeField] Button _cancel;
    [SerializeField] Button _join;
    [SerializeField] private TMP_InputField _joinInput;


    public void Init(Action<string> JoinDelegate, Action ResetButtonsDelegate)
    {
        _cancel.onClick.AddListener(Cancel);
        _cancel.onClick.AddListener(() => ResetButtonsDelegate?.Invoke()); ;
        _join.onClick.AddListener(delegate { JoinDelegate?.Invoke(_joinInput.text); });

    }

    void Cancel()
    {
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        _cancel.onClick.RemoveAllListeners();
        _join.onClick.RemoveAllListeners();
    }
}
