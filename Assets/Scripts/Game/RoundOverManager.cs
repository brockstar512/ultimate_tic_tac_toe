using System;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RoundOverManager : NetworkBehaviour
{
    public const string Waiting = "Waiting On Opponent...";
    public const string Requested = "Opponent Wants To Play Again!";
    public const string OpponentLeft = "Opponent Already Left";


    //public const string Initial = "Waiting On Opponent";
    [SerializeField] WrapUpHandler _wrapUpHandler;
    [SerializeField] TextMeshProUGUI _promptText;
    [SerializeField] Button _playAgainButton;
    [SerializeField] Button _quitButton;
    [SerializeField] Button _acceptButton;
    CanvasGroup cg;
    public static event Action reset;

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();
        WinLineHandler.roundOver += Init;
        GameManager.Instance.TimeOut += Init;

        _playAgainButton.onClick.AddListener(PlayAgainRequest);
        _acceptButton.onClick.AddListener(HandlePlayAgainAcceptServerRpc);
        _quitButton.onClick.AddListener(Quit);

    }

    private void Init()
    {
        Debug.Log("Running  init");
        cg.DOFade(1,.5f).SetEase(Ease.OutSine);
        cg.interactable = true;
        cg.blocksRaycasts = true;
        _playAgainButton.interactable = true;
        _quitButton.interactable = true;
    }


    [ContextMenu("Reset Button")]
    private void Reset()
    {
        //cg.interactable = false;
        //cg.blocksRaycasts = false;
        //cg.DOFade(1, .15f).SetEase(Ease.OutSine);
        reset?.Invoke();
    }

    void PlayAgainRequest()
    {
        //serverRpcParams.Receive.SenderClientId
        _playAgainButton.interactable = false;
        _playAgainButton.gameObject.SetActive(false);
        _promptText.text = Waiting;
        HandlePlayAgainRequestServerRpc(new ServerRpcParams());
    }

    [ServerRpc(RequireOwnership = false)]
    void HandlePlayAgainRequestServerRpc(ServerRpcParams serverRpcParams)
    {
        PlayAgainOfferClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void HandlePlayAgainAcceptServerRpc()
    {
        ResetGameClientRpc();
    }

    [ClientRpc]
    void PlayAgainOfferClientRpc(ulong RequestOwner)
    {
        //OwnerClientId isd is outputting wrong number its giving the same numbers for both clients... maybe the client id is the netcode objects id and not the client
        //Debug.Log($"Server send request made by {RequestOwner} and this is who is recieving it {OwnerClientId} and here is the network signleton {NetworkManager.Singleton.LocalClientId}");

        if (NetworkManager.Singleton.LocalClientId == RequestOwner)
            return;

        _playAgainButton.interactable = false;
        _playAgainButton.gameObject.SetActive(false);
        _promptText.text = Requested;
        _acceptButton.gameObject.SetActive(true);
        _acceptButton.interactable = true;
        Debug.Log("Other client wants to play again");
    }

    [ClientRpc]
    void ResetGameClientRpc()
    {
        Debug.Log("Everyone Reset");
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.DOFade(0, .15f).SetEase(Ease.OutSine);
        reset?.Invoke();
    }

    private void Quit()
    {
        _quitButton.interactable = false;
        QuitServerRpc();
        NetworkManager.Singleton.Shutdown();
    }

    [ServerRpc(RequireOwnership = false)]
    void QuitServerRpc()
    {
        Debug.Log("Player has Quit");
        //disconnect
        //go to lobby
        var (xVal, oVal, didWin) = GameManager.Instance.EndGameStatus();
        QuitClientRpc(xVal, oVal, didWin);
        //this should be in the overall wrap up
        //LoadingManager.Instance.QuickLoad(Enums.MyScenes.Menu);

    }

    [ClientRpc]
    void QuitClientRpc(int xScore, int oScore, bool didWin)
    {
        Debug.Log("Everyone Quit");
        //disconnect
        //go to lobby
        NetworkManager.Singleton.Shutdown();
        _wrapUpHandler.Init(xScore, oScore, didWin);
        //this should be in the overall wrap up
        //LoadingManager.Instance.QuickLoad(Enums.MyScenes.Menu);

    }

    public override void OnDestroy()
    {
        _playAgainButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
    }

}
