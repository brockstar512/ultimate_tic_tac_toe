using System.Threading.Tasks;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Enums;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Image _progressBar;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        CanvasGroup cg = _loaderCanvas.transform.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
    }

    public void LoadNetwork(MyScenes targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
        _progressBar.DOFillAmount(1,1);
        Debug.Log("Show Loading screen");
    }

    public async void QuickLoad(MyScenes target)
    {
        await Enter();
        SceneManager.LoadScene(target.ToString());
        await Task.Delay(500);
        Exit();
    }

    public async void LoadScene(MyScenes targetScene)
    {


        await Enter();

        LoadNetwork(targetScene);

        //await Exit();
    }


    [ContextMenu("Enter")]
    async Task Enter()
    {
        Debug.Log("LOAD PAGE");
        _loaderCanvas.gameObject.SetActive(true);
        //_loaderCanvas.transform.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 1.5f).SetEase(enterEase);
        CanvasGroup cg = _loaderCanvas.transform.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = true;
        cg.DOFade(1, .1f);

        await Task.Delay(500);

    }

    [ContextMenu("Exit")]
    public void Exit()
    {
        //_progressBar.fillAmount = 1;
        //await Task.Delay(500);
        _progressBar.fillAmount = 1;
        CanvasGroup cg = _loaderCanvas.transform.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.DOFade(0, .15f).OnComplete(() => { _loaderCanvas.SetActive(false); });

    }



}
