using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class JoinUI : MonoBehaviour
{
    [SerializeField] Button _cancel;
    [SerializeField] Button _join;
    [SerializeField] private TMP_InputField _joinInput;


    public void Init(Action<string> JoinDelegate)
    {
        _cancel.onClick.AddListener(Cancel);
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
