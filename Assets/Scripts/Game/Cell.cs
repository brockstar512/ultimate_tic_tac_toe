using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    private Button _button;
    private byte _index;
    private byte _row;
    private byte _col;

    public void Init(byte index)
    {
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        _button.onClick.AddListener(CellClicked);
    }

    private void CellClicked()
    {

        _button.interactable = false;

        //Debug.Log("Sending MarkCellRequest to Server!");

        //var msg = new Net_MarkCellRequest()
        //{
        //    Index = _index
        //};

        //NetworkClient.Instance.SendServer(msg);
    }

}
