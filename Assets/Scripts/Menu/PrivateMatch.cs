using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System;

public class PrivateMatch : MonoBehaviour
{
    [SerializeField] Button hostMatch;
    [SerializeField] Button joinMatch;
    public Action<string> JoinDelegate;
    private UnityTransport _transport;
    private const int MaxPlayers = 2;
    [SerializeField] HostUI hostScreen;
    [SerializeField] JoinUI joinScreen;



    private async void Awake()
    {
        
        _transport = FindObjectOfType<UnityTransport>();
        JoinDelegate += JoinGame;

        //_buttons.SetActive(false);

        await Authenticate();

        //_buttons.SetActive(true);
        //findMatch.onClick.AddListener(FindMatch);
        hostMatch.onClick.AddListener(CreateGame);
        joinMatch.onClick.AddListener(ShowJoin);
        //matchMakingMatch.onClick.AddListener(ShowMatchmaking);
    }

    private static async Task Authenticate()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

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

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(code);

        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }

    void ShowJoin()
    {
        Instantiate(joinScreen, this.transform.parent).Init(JoinDelegate);

    }


    void HostMatch(string code)
    {
        Instantiate(hostScreen, this.transform.parent).Init(code);
    }




    private void OnDestroy()
    {
        hostMatch.onClick.RemoveAllListeners();
        JoinDelegate -= JoinGame;
    }
}
