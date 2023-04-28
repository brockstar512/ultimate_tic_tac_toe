using System;
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
    public event Action onCellSelected;

    public void Init(byte index)
    {
        _mark = this.transform.GetChild(0).GetComponent<Image>();
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        _button.onClick.AddListener(CellClicked);
        this.gameObject.name = $"Cell: [{_row},{_col}]";

    }

    private void CellClicked()
    {

        _button.interactable = false;
        _mark.enabled = true;
        Vector3 size = new Vector3(.90f, .90f, .90f);
        _mark.transform.DOScale(size, .05f).SetEase(Ease.OutBounce).OnComplete(()=> onCellSelected?.Invoke());
        //invoke event that this cell is clicked
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

}

//actions dont need to be declared before making a pointer
//events delegates have to be called within the class... normal delegate can be called any where
//so event delegates have to managed their own invoktion but subscribers can still subscribe to them they just can fire them
//public event OnCellSelected onCellSelected;
//public delegate void OnCellSelected();
//public static event Action<Net_OnAuthFail> OnAuthFail;
//_img.transform.DOScale(.90f,.05f).SetEase(Ease.OutBounce);

