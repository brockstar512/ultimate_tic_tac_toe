using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionHandler : MonoBehaviour
{

    private const int MaxPlayers = 2;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;

    void Start()
    {
        joinButton.onClick.AddListener(StartJoin);
        hostButton.onClick.AddListener(StartHost);

        NetworkManager.Singleton.OnClientConnectedCallback += (clienId) =>
        {
            Debug.Log($"ClientID {clienId} has joined");//IsServer
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                //SpawnBoard();
                //register game
                Debug.Log($"Register game");
                GameManager.Instance.RegisterGame(NetworkManager.Singleton.ConnectedClients[0].ClientId.ToString(), NetworkManager.Singleton.ConnectedClients[1].ClientId.ToString());
            }
            GameManager.Instance.MyUsername = clienId.ToString();


        };
    }



    public void StartHost()
    {
        hostButton.interactable = false;

        NetworkManager.Singleton.StartHost();
    }
    public void StartJoin()
    {
        joinButton.interactable = false;

        NetworkManager.Singleton.StartClient();

    }

}
