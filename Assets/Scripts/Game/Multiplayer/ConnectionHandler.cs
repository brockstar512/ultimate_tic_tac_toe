using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionHandler : NetworkBehaviour
{
    [SerializeField] Transform board;
    [SerializeField] Transform boardHolder;

    private const int MaxPlayers = 2;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;

    void Start()
    {
        joinButton.onClick.AddListener(StartJoin);
        hostButton.onClick.AddListener(StartHost);

        NetworkManager.Singleton.OnClientConnectedCallback += (clienId) =>
        {
            Debug.Log($"ClientID {clienId} has joined");//
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                Debug.Log("Start Game");
                //Guid Id = Guid.NewGuid();
                SpawnBoard();
            }
            //else if (NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                //GameManager.Instance.RegisterGame(NetworkManager.Singleton.ConnectedClients[0].ClientId.ToString(), NetworkManager.Singleton.ConnectedClients[1].ClientId.ToString());

            }
        };
    }

    void SpawnBoard()
    {
        Debug.Log("board spawn");
        Transform _board = Instantiate(board, boardHolder);
        _board.GetComponent<NetworkObject>().Spawn();
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
