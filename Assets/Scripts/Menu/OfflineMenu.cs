using UnityEngine;
using UnityEngine.UI;

using static Enums;

public class OfflineMenu : MonoBehaviour
{
    MyScenes target = MyScenes.LocalGame;
    Button _button;
    // Start is called before the first frame update
    void Start()
    {
        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(() => LoadingManager.Instance.QuickLoad(target)); 
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();

    }
}
