using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountDownHandler : NetworkBehaviour
{

    public static CountDownHandler Instance { get; private set; }
    [SerializeField] TextMeshProUGUI numberText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }
    private void Start()
    {
        RoundOverManager.reset += StartCountDown;
    }

    public void StartCountDown()
    {
        StopCoroutine(CountDownHandler.Instance.CountDown());
        StartCoroutine(CountDownHandler.Instance.CountDown());
    }


    private IEnumerator CountDown()
    {
        TurnIndicatorHandler.Instance.Show(false);

        numberText.text = 3.ToString();
        numberText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            numberText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        numberText.text = "START!";
        yield return new WaitForSeconds(1);
        numberText.gameObject.SetActive(false);
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
        Debug.Log($"I am sending my type as {(int)GameManager.Instance.myPlayer.MyType.Value} and as a byte {(byte)GameManager.Instance.myPlayer.MyType.Value}");
        GameManager.Instance.StartGameServerRpc(GameManager.Instance.myPlayer.MyType.Value);
        TurnIndicatorHandler.Instance.Show(true);

    }


}
