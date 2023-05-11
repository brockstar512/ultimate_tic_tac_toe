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
            ulong _playerOne = 0;
            ulong _playerTwo = 0;
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                //SpawnBoard();
                //register game
                //NetworkManager.Singleton.ConnectedClients[1].ClientId.ToString()
                Debug.Log($"Register game player count: {NetworkManager.Singleton.ConnectedClients.Count}");
                //ulong _playerOne;
                //ulong _playerTwo;
                int playerCount = 0;
                foreach (ulong client in NetworkManager.Singleton.ConnectedClients.Keys)
                {
                    if (playerCount == 0)
                    {
                        _playerOne = client;
                    }
                    else
                    {
                        _playerTwo = client;

                    }
  
                    playerCount++;
                }


                GameManager.Instance.RegisterGame(_playerOne, _playerTwo);

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
