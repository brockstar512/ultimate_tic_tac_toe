using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class RoundOverManager : MonoBehaviour
{
    [SerializeField] Button _playAgainButton;
    [SerializeField] Button _quitButton;
    [SerializeField] Button _acceptButton;
    CanvasGroup cg;
    public static event Action reset;

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();
    }


    [ContextMenu("Reset Button")]
    private void Reset()
    {
        reset?.Invoke();
    }


}
