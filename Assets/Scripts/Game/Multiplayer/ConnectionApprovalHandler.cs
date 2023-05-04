using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : MonoBehaviour
{
    [SerializeField] Transform board;
    private const int MaxPlayers = 2;
    // Start is called before the first frame update

    public void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clienId) =>
        {
            Debug.Log($"ClientID {clienId} has joined");
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                Debug.Log("Start Game");
                //Guid Id = Guid.NewGuid();
            }
            else if (NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                //GameManager.Instance.RegisterGame(NetworkManager.Singleton.ConnectedClients[0].ClientId.ToString(), NetworkManager.Singleton.ConnectedClients[1].ClientId.ToString());

            }
            Instantiate(board, this.transform);


        };
    }
    //void Start()
    //{
    //    NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    //}

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Connection approval");
        response.Approved = true;
        response.CreatePlayerObject = false; //creates the player object
        response.PlayerPrefabHash = null;//uses the default prfab

        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "Server is full";
        }
        Instantiate(board, this.transform);
        response.Pending = false;
    }

}
