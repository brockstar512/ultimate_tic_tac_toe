using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class Cell : MonoBehaviour
{
    Image _mark;
    private Button _button;
    private byte _index;
    private byte _row;
    private byte _col;
    public event Action onCellSelected;
    public event Action<int,int> markCell;


    public void Init(byte index)
    {
        _mark = this.transform.GetChild(0).GetComponent<Image>();
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        _button.onClick.AddListener(CellClicked);
        this.gameObject.name = $"Cell: [{_row},{_col}]";
        RoundOverManager.reset += Reset;


    }

    private void CellClicked()
    {

        _button.interactable = false;
        _mark.enabled = true;
        Vector3 size = new Vector3(.90f, .90f, .90f);
        markCell?.Invoke(_row,_col);
        _mark.transform.DOScale(size, .15f).SetEase(Ease.OutElastic).OnComplete(()=> onCellSelected?.Invoke());
        //invoke event that this cell is clicked
    }

    private void Reset()
    {
        _button.interactable = true;
        //_mark.enabled = false;
        Vector3 size = new Vector3(.25f, .25f, .25f);
        _mark.transform.DOScale(size, .15f).SetEase(Ease.InElastic).OnComplete(() => { _mark.enabled = false; });
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
        onCellSelected = null;
        markCell = null;
    }

}

//actions dont need to be declared before making a pointer
//events delegates have to be called within the class... normal delegate can be called any where
//so event delegates have to managed their own invoktion but subscribers can still subscribe to them they just can fire them
//public event OnCellSelected onCellSelected;
//public delegate void OnCellSelected();
//public static event Action<Net_OnAuthFail> OnAuthFail;
//_img.transform.DOScale(.90f,.05f).SetEase(Ease.OutBounce);

