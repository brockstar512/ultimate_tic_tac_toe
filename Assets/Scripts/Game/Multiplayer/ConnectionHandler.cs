using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionHandler : MonoBehaviour
{
    public Transform _board;
    [SerializeField] Transform boardPrefab;
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
            Debug.Log($"ClientID {clienId} has joined");//IsServer
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                SpawnBoard();
            }

            //SpawnBoard();
        };
    }

    void SpawnBoard()
    {
        Debug.Log("board spawn");
        //_board = Instantiate(boardPrefab, boardHolder);
        _board = Instantiate(boardPrefab);
        _board.GetComponent<NetworkObject>().Spawn();
        //_board.SetParent(boardHolder);
        //_board.GetComponent<NetworkObject>().Spawn();
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
