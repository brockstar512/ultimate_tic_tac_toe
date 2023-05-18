using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControl : MonoBehaviour
{
    [SerializeField] Button icon;
    [SerializeField] Sprite soundOn;
    [SerializeField] Sprite soundOff;
    bool isMute = false;
    [SerializeField] AudioClip _soundSample;

    private void Awake()
    {
        icon.onClick.AddListener(ChangeVolume);
        //get player prefs
    }

    void ChangeVolume()
    {
        isMute = !isMute;
        icon.image.sprite = isMute ? soundOff : soundOn;
        float volumeLevel = isMute ? 0 : .75f;
        SoundManager.Instance.ChangeMatserVolume(volumeLevel);
        SoundManager.Instance.PlaySound(_soundSample);
    }

    private void OnDestroy()
    {
        icon.onClick.RemoveAllListeners();
    }

}
