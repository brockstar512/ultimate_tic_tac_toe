using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ButtonPressed : MonoBehaviour
{
    Button _button;
    [SerializeField] AudioClip _pressed;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Pressed);
    }
    void Pressed()
    {
        return;
        Debug.Log(gameObject.name);
        SoundManager.Instance.PlaySound(_pressed);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();

    }
}
