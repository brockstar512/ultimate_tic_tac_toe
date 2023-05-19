using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
        //if (NetworkManager.Singleton.LocalClient == null)
        //    return;
        NetworkManager.Singleton.Shutdown();
        //Destroy(NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<OnlinePlayer>().gameObject);
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        _cancel.onClick.RemoveAllListeners();
    }
}
