using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;

public class OfflineCountDownHandler : MonoBehaviour
{
    public static OfflineCountDownHandler Instance { get; private set; }
    [SerializeField] TextMeshProUGUI numberText;
    //[SerializeField] AudioClip _numberCount;
    //[SerializeField] AudioClip _start;


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
        OfflineRoundOverManager.reset += StartCountDown;
    }

    public void StartCountDown()
    {
        StopCoroutine(CountDown());
        StartCoroutine(CountDown());
    }


    private IEnumerator CountDown()
    {
        OfflineTurnIndicator.Instance.Show(false);

        numberText.text = 3.ToString();

        numberText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            //SoundManager.Instance.PlaySound(_numberCount);

            numberText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        //SoundManager.Instance.PlaySound(_start);
        numberText.text = "START!";
        yield return new WaitForSeconds(1);
        numberText.gameObject.SetActive(false);
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
        OfflineGameManager.Instance.StartGame();

    }
}
