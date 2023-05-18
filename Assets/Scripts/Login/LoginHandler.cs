using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class LoginHandler : MonoBehaviour
{

    [SerializeField] Button _play;
    MyScenes target = MyScenes.Menu;

    private void Awake()
    {
        _play.onClick.AddListener(Play);
    }

    void Play()
    {
        LoadingManager.Instance.QuickLoad(target);
    }

    void OnDestory()
    {
        _play.onClick.RemoveAllListeners();
    }
}
