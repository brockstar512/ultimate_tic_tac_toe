using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionHandler : NetworkBehaviour
{

    private const int MaxPlayers = 2;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;
    [SerializeField] Button serverButton;

    void Start()
    {
        joinButton.onClick.AddListener(StartJoin);
        hostButton.onClick.AddListener(StartHost);
        serverButton.onClick.AddListener(StartServer);

        NetworkManager.Singleton.OnClientConnectedCallback += (clienId) =>
        {
            Debug.Log($"ClientID {clienId} has joined");//IsServer
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                //SpawnBoard();
                //register game
                //NetworkManager.Singleton.ConnectedClients[1].ClientId.ToString()
                Debug.Log($"Register game");
                GameManager.Instance.RegisterGame((byte)0, (byte)1);

            }
            
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
    public void StartServer()
    {
        serverButton.interactable = false;

        NetworkManager.Singleton.StartServer();

    }

}
