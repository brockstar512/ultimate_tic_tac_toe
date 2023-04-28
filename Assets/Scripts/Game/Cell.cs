using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    Image _mark;
    private Button _button;
    private byte _index;
    private byte _row;
    private byte _col;

    private void Awake()
    {
        Init(0);
    }
    public void Init(byte index)
    {
        _mark = this.transform.GetChild(0).GetComponent<Image>();
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        _button.onClick.AddListener(CellClicked);
    }

    private void CellClicked()
    {
        Debug.Log("Cell clicked");
        _button.interactable = false;
        _mark.enabled = true;
        _mark.transform.DOScale(Vector3.one,.1f).SetEase(Ease.OutBounce);
        //Debug.Log("Sending MarkCellRequest to Server!");

        //var msg = new Net_MarkCellRequest()
        //{
        //    Index = _index
        //};

        //NetworkClient.Instance.SendServer(msg);
    }

}
