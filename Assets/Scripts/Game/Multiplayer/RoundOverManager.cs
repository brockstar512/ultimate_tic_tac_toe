using System;
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
    public const string OPPONENT_LEFT = "Opponent Already Left";
    public const string REMATCH = "Rematch?";

    const string TIE = "Round Tied!";
    const string LOSE = "Round Lost!";
    const string WIN = "Round Won!";
    public Color tie_Color;

    [SerializeField] Image _banner;
    [SerializeField] TextMeshProUGUI _header;
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

    private void Init(MarkType MarkType)
    {
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

        var (xVal, oVal, didWin) = GameManager.Instance.EndGameStatus();
        QuitClientRpc(xVal, oVal, didWin);
    }

    [ClientRpc]
    void QuitClientRpc(int xScore, int oScore, bool didWin)
    {

        NetworkManager.Singleton.Shutdown();
        _wrapUpHandler.Init(xScore, oScore, didWin);

    }

    public override void OnDestroy()
    {
        _playAgainButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
    }

}
