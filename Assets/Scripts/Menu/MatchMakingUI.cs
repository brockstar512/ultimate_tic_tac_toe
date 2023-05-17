using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MatchMakingUI : MonoBehaviour
{
    [SerializeField] Button _cancel;

    private void Awake()
    {
        _cancel.onClick.AddListener(Cancel);
    }


    void Cancel()
    {
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        _cancel.onClick.RemoveAllListeners();
    }
}
