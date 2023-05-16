using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Enums;
using Unity.Netcode;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Image _progressBar;
    float _target;

    public Ease exitEase;
    public Ease enterEase;
    public bool showBar;


    //loading screen enter animation and exit
    //enforce a one second delay
    //fade in  and fade out

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
    }

    public void LoadNetwork(MyScenes targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public void QuickLoad(MyScenes target)
    {
        SceneManager.LoadScene(target.ToString());
    }

    public async void LoadScene(string sceneName)
    {
        if (!showBar)
            _progressBar.gameObject.SetActive(false);

        await Enter();

        _target = 0;
        _progressBar.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);

        do
        {
            await System.Threading.Tasks.Task.Delay(100);
            _target = scene.progress;


        } while (scene.progress < 0.9f);


        scene.allowSceneActivation = true;
        //make fill either a slider or completely fill the image
        //start task of exit.
        await Exit();
        //_loaderCanvas.SetActive(false);
    }
    private void Update()
    {
        if (!showBar)
            return;
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 3 * Time.deltaTime);
    }

    [ContextMenu("Enter")]
    async Task Enter()
    {
        Debug.Log("LOAD PAGE");
        //_loaderCanvas.transform.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 1.5f).SetEase(enterEase);
        _loaderCanvas.transform.GetComponent<CanvasGroup>().DOFade(1, .25f).SetEase(enterEase);

        await Task.Delay(1000);

    }

    [ContextMenu("Exit")]
    async Task Exit()
    {
        //_progressBar.fillAmount = 1;
        await Task.Delay(1000);
        _progressBar.fillAmount = 1;
        // _loaderCanvas.transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 1920), 1f).SetEase(exitEase);
        _loaderCanvas.transform.GetComponent<CanvasGroup>().DOFade(0, .25f).SetEase(enterEase).OnComplete(() => _progressBar.fillAmount = 1f);

    }


    public void Show()
    {
        _loaderCanvas.transform.GetComponent<CanvasGroup>().DOFade(1, 0).SetEase(enterEase);
    }
    public void Hide()
    {
        _loaderCanvas.transform.GetComponent<CanvasGroup>().DOFade(0, 0).SetEase(enterEase);
    }
}
