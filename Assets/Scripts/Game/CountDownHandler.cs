using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CountDownHandler : MonoBehaviour
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


    public IEnumerator CountDown()
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
        TurnIndicatorHandler.Instance.Show(true);

    }


}
