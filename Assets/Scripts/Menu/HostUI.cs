using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostUI : MonoBehaviour
{
    [SerializeField] TMP_Text _joinCodeText;
    [SerializeField] Button _copy;
    [SerializeField] Button _cancel;
    [SerializeField] CanvasGroup copyPromptCG;



    private void Awake()
    {
        _cancel.onClick.AddListener(Cancel);
        _copy.onClick.AddListener(Copy);

    }

    public void Init(string hostCode)
    {
        _joinCodeText.text = hostCode;
    }

    void Copy()
    {
        GUIUtility.systemCopyBuffer = _joinCodeText.text;
        copyPromptCG.DOFade(1, 0).OnComplete(() => { copyPromptCG.DOFade(1, 1).OnComplete(() => { copyPromptCG.DOFade(0, 2); }); });
    }
    void Cancel()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(this.gameObject);   
    }

    private void OnDestroy()
    {
        _copy.onClick.RemoveAllListeners();
        _cancel.onClick.RemoveAllListeners();

    }
}
