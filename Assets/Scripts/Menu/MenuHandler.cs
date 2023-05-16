using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Unity.Services.Core;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] Button findMatch;
    //LoadingManager.Instance.LoadScene(target.ToString())
    private void Awake()
    {

        findMatch.onClick.AddListener(FindMatch);
    }

    void InitializeMultiPlayer()
    {

    }

    void FindMatch()
    {

    }
    void JoinMatch()
    {

    }
    void HostMatch()
    {

    }


    private void OnDestroy()
    {
        findMatch.onClick.RemoveAllListeners();
    }
}
