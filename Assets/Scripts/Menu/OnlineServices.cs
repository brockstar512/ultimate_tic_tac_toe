using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class OnlineServices : MonoBehaviour
{
    [SerializeField] Button hostMatch;
    [SerializeField] Button joinMatch;
    [SerializeField] Button matchMakingMatch;
    public Action<string> JoinDelegate;
    private UnityTransport _transport;
    private const int MaxPlayers = 2;
    [SerializeField] HostUI hostScreen;
    [SerializeField] JoinUI joinScreen;
    [SerializeField] MatchMakingUI matchMakingScreen;
    private Lobby _connectedLobby;
    private QueryResponse _lobbies;
    private const string JoinCodeKey = "j";
    private string _playerId;


    private async void Start()
    {
        //NetworkManager.Singleton.Shutdown();

        //---
        _transport = FindObjectOfType<UnityTransport>();
        await Authenticate();



        JoinDelegate += JoinGame;
        hostMatch.onClick.AddListener(CreateGame);
        joinMatch.onClick.AddListener(ShowJoin);
        matchMakingMatch.onClick.AddListener(ShowMatchmaking);

    }

    //starts servivces and give anonomous id to player
    private async Task Authenticate()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;

        var options = new InitializationOptions();

#if UNITY_EDITOR
        // Remove this if you don't have ParrelSync installed. 
        // It's used to differentiate the clients, otherwise lobby will count them as the same
        options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerId = AuthenticationService.Instance.PlayerId;
    }

    //--matchamking matches

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            const int maxPlayers = 2;

            // Create a relay allocation and generate a join code to share with the lobby

            //allocation
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            //get join code
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Create a lobby, adding the relay join code to the lobby data
            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };
            var lobby = await Lobbies.Instance.CreateLobbyAsync("Useless Lobby Name", maxPlayers, options);

            // Send a heartbeat every 15 seconds to keep the room alive
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            // Set the game room to use the relay allocation
            _transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            // Start the room. I'm doing this immediately, but maybe you want to wait for the lobby to fill up
            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.LogFormat($"Failed creating a lobby: {e.Message}");
            return null;
        }
    }
    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            // Attempt to join a lobby in progress
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            // If we found one, grab the relay allocation details
            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            // Set the details to the transform
            SetTransformAsClient(a);

            // Join the game room as a client
            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log($"No lobbies available via quick join: {e.Message}");
            return null;
        }
    }



    private void SetTransformAsClient(JoinAllocation a)
    {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private async void CreateOrJoinLobby()
    {
        //await Authenticate();

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        //if (_connectedLobby != null) _buttons.SetActive(false);
    }


    //--private matches
    public async void CreateGame()
    {
        hostMatch.interactable = false;

        Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        HostMatch(joinCode);
        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame(string code)
    {
        //maybe try catch here?

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(code);

        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }


    //--UI
    void ShowJoin()
    {
        Instantiate(joinScreen, this.transform.parent).Init(JoinDelegate);

    }

    void ShowMatchmaking()
    {
        Instantiate(matchMakingScreen, this.transform.parent);
        CreateOrJoinLobby();
        //Init();

    }

    void HostMatch(string code)
    {
        Instantiate(hostScreen, this.transform.parent).Init(code);
    }

    private void OnDestroy()
    {
        matchMakingMatch.onClick.RemoveAllListeners();
        hostMatch.onClick.RemoveAllListeners();
        JoinDelegate -= JoinGame;

        try
        {
            StopAllCoroutines();
            // todo: Add a check to see if you're host
            if (_connectedLobby != null)
            {
                if (_connectedLobby.HostId == _playerId) Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                else Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error shutting down lobby: {e}");
        }
    }

}
