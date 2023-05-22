using System;
using System.Linq;
using System.Threading;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class RoundOverManager : NetworkBehaviour
{
    public const string WAITING = "Waiting On Opponent...";
    public const string REQUESTED = "Opponent Wants To Play Again!";
    public const string OPPONENT_LEFT = "Opponent Has Left";
    public const string REMATCH = "Rematch?";

    const string TIE = "Round Tied!";
    const string LOSE = "Round Lost!";
    const string WIN = "Round Won!";
    public Color tie_Color;

    [SerializeField] GameObject _popUpScreen;
    [SerializeField] Image _banner;
    [SerializeField] TextMeshProUGUI _header;
    [SerializeField] WrapUpHandler _wrapUpHandler;
    [SerializeField] TextMeshProUGUI _promptText;
    [SerializeField] Button _playAgainButton;
    [SerializeField] Button _quitButton;
    [SerializeField] Button _acceptButton;
    CanvasGroup cg;
    public static event Action reset;
    [SerializeField] AudioClip roundOverSoundFX;

    void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();
        WinLineHandler.roundOver += Init;
        GameManager.Instance.TimeOut += Init;

        _playAgainButton.onClick.AddListener(PlayAgainRequest);
        _acceptButton.onClick.AddListener(HandlePlayAgainAcceptServerRpc);
        _quitButton.onClick.AddListener(Quit);

    }

    void Init(MarkType MarkType)
    {
        SoundManager.Instance.PlaySound(roundOverSoundFX);
        _popUpScreen.SetActive(true);

        GameManager.Instance.myPlayer.ForceOff();
        _playAgainButton.gameObject.SetActive(true);
        _quitButton.gameObject.SetActive(true);
        _acceptButton.gameObject.SetActive(false);

        SetUI(MarkType);
        _promptText.text = REMATCH;
        cg.DOFade(1,.5f).SetEase(Ease.OutSine);
        cg.interactable = true;
        cg.blocksRaycasts = true;
        _playAgainButton.interactable = true;
        _quitButton.interactable = true;
    }

    void SetUI(MarkType markType)
    {
        if (markType == MarkType.None)
        {
            _header.text = TIE;
            _banner.color = tie_Color;
            return;
        }
        _header.text = (MarkType)GameManager.Instance.myPlayer.MyType.Value == markType ? WIN : LOSE;
        _banner.color = (MarkType)GameManager.Instance.myPlayer.MyType.Value == markType ? GameManager.Instance.myPlayer.GetMyColor : GameManager.Instance.myPlayer.GetOpponentColor;
    }

    void PlayAgainRequest()
    {
        _playAgainButton.interactable = false;
        _playAgainButton.gameObject.SetActive(false);
        _promptText.text = WAITING;
        HandlePlayAgainRequestServerRpc(new ServerRpcParams());
    }

    private void Quit()
    {
        _quitButton.interactable = false;

        if (NetworkManager.Singleton.IsClient)
        {
            QuitServerRpc();
        }
        else
        {
            Debug.Log("Network is dead");
            var (xVal, oVal) = GameManager.Instance.EndGameStatus();
            bool didWin;
            if ((MarkType)GameManager.Instance.myPlayer.MyType.Value == MarkType.X && xVal > oVal)
            {
                didWin = true;
            }
            else
            {
                didWin = false;
            }

            _popUpScreen.SetActive(false);
            _wrapUpHandler.Init(xVal, oVal, didWin);
        }
    }
   

    [ServerRpc(RequireOwnership = false)]
    void HandlePlayAgainRequestServerRpc(ServerRpcParams serverRpcParams)
    {
        GameManager.Instance.ResetPlayerOrder();
        PlayAgainOfferClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void HandlePlayAgainAcceptServerRpc()
    {
        ResetGameClientRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    void QuitServerRpc(ServerRpcParams serverRpcParams = default)
    {
       
        //Debug.Log($"How many people are here? {NetworkManager.ConnectedClients.Count}");
        if (NetworkManager.ConnectedClients.Count == 2)
        {
            //Debug.Log($"Sending the other player that the opponent left?");

            //sned the other...OpponentHasLeft()
            //if someone is still around
            ulong updateClientId = GameManager.Instance.clientList.FirstOrDefault(otherPlayer => otherPlayer != serverRpcParams.Receive.SenderClientId);
            ClientRpcParams opponentRpcParams = default;
            ulong[] opponentTarget = new ulong[] { updateClientId };
            opponentRpcParams.Send.TargetClientIds = opponentTarget;
            OpponentHasLeftClientRpc(opponentRpcParams);
        }
        ulong leavingClientId = serverRpcParams.Receive.SenderClientId;
        ClientRpcParams rpcParams = default;
        ulong[] singleTarget = new ulong[] { leavingClientId };
        rpcParams.Send.TargetClientIds = singleTarget;
        //send the sender this
        var (xVal, oVal) = GameManager.Instance.EndGameStatus();
        QuitClientRpc(xVal, oVal, rpcParams);



    }

    [ClientRpc]
    void QuitClientRpc(int xScore, int oScore, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"xScore {xScore}");
        Debug.Log($"oScore {oScore}");
        Debug.Log($"my type {(MarkType)GameManager.Instance.myPlayer.MyType.Value}");
        Debug.Log($"did win? {(MarkType)GameManager.Instance.myPlayer.MyType.Value == MarkType.X && xScore > oScore}");


        bool didWin;
        if ((MarkType)GameManager.Instance.myPlayer.MyType.Value == MarkType.X && xScore > oScore)
        {
            didWin = true;
        }
        else
        {
            didWin = false;
        }

        _popUpScreen.SetActive(false);
        _wrapUpHandler.Init(xScore, oScore, didWin);
        NetworkManager.Singleton.Shutdown();
    }

    [ClientRpc]
    void OpponentHasLeftClientRpc(ClientRpcParams clientRpcParams = default)
    {
        _promptText.text = OPPONENT_LEFT;
        _playAgainButton.gameObject.SetActive(false);
        _acceptButton.gameObject.SetActive(false);
    }

    [ClientRpc]
    void PlayAgainOfferClientRpc(ulong RequestOwner)
    {

        if (NetworkManager.Singleton.LocalClientId == RequestOwner)
            return;

        _playAgainButton.interactable = false;
        _playAgainButton.gameObject.SetActive(false);
        _promptText.text = REQUESTED;
        _acceptButton.gameObject.SetActive(true);
        _acceptButton.interactable = true;
    }

    [ClientRpc]
    void ResetGameClientRpc()
    {
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.DOFade(0, .15f).SetEase(Ease.OutSine);
        TimeManager.Instance.gameObject.SetActive(true);
        reset?.Invoke();
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        reset = null;
    }
    public override void OnDestroy()
    {
        _playAgainButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
    }

}
