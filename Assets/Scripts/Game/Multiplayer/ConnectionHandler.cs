using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionHandler : NetworkBehaviour
{
    public static ConnectionHandler Instance { get; private set; }

    private const int MaxPlayers = 2;
    ulong _playerOne;
    ulong _playerTwo;

    //[SerializeField] Button joinButton;
    //[SerializeField] Button hostButton;
    //[SerializeField] Button serverButton;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        //joinButton.onClick.AddListener(StartJoin);
        //hostButton.onClick.AddListener(StartHost);
        //serverButton.onClick.AddListener(StartServer);

        NetworkManager.Singleton.OnClientConnectedCallback += (clienId) =>
        {
            Debug.Log($"ClientID {clienId} has joined");//IsServer
            _playerOne = 0;
            _playerTwo = 0;
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
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


                //GameManager.Instance.RegisterGame(_playerOne, _playerTwo);
                LoadingManager.Instance.LoadNetwork(Enums.MyScenes.OnlineGame);
            }
            
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc()
    {
        Debug.Log("Start Game");

        GameManager.Instance.RegisterGame(_playerOne, _playerTwo);
    }

    public void StartHost()
    {
        //hostButton.interactable = false;

        NetworkManager.Singleton.StartHost();
    }
    public void StartJoin()
    {
        //joinButton.interactable = false;

        NetworkManager.Singleton.StartClient();

    }

    public void StartServer()
    {
        //serverButton.interactable = false;

        NetworkManager.Singleton.StartServer();

    }

}
