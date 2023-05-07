using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using DG.Tweening;
using Unity.Netcode;
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
    private byte _board;
    //public event Action onCellSelected;
    public event Action<int,int> markCell;

    public void Init(byte index, byte boardNumber)
    {
        _board = boardNumber;
        _mark = this.transform.GetChild(0).GetComponent<Image>();
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        _button.onClick.AddListener(CellClicked);
        _button.onClick.AddListener(delegate { GameManager.Instance.UpdateBoardServerRpc(_board, _index); });
        this.gameObject.name = $"Cell: [{_row},{_col}] board number {_board}";
        RoundOverManager.reset += Reset;
        //GameManager.Instance.UpdateBoardServerRpc(_board, _index);//take it out of here and just add it to the cell click button?

        //GameManager.Instance.InitializeBoardValidation(_board,_index);
    }


    public void CellClicked()
    {
        //if (GameManager.Instance.myPlayer.IsMyTurn.Value)
        //{
            //TimeManager.Instance.StopTimer();
        //}

        _mark.color = GameManager.Instance.GetColor;
        _button.interactable = false;
        _mark.enabled = true;
        Vector3 size = new Vector3(.90f, .90f, .90f);
        markCell?.Invoke(_row,_col);
        _mark.transform.DOScale(size, .1f).SetEase(Ease.OutElastic);
    }

    

    public void Reset()
    {
        _button.interactable = true;
        //_mark.enabled = false;
        Vector3 size = new Vector3(.25f, .25f, .25f);
        _mark.transform.DOScale(size, .15f).SetEase(Ease.InElastic).OnComplete(() => { _mark.enabled = false; });
    }

    //private void OnDestroy()
    //{
    //    _button.onClick.RemoveAllListeners();
        
    //    markCell = null;
    //}

}

//actions dont need to be declared before making a pointer
//events delegates have to be called within the class... normal delegate can be called any where
//so event delegates have to managed their own invoktion but subscribers can still subscribe to them they just can fire them
//public event OnCellSelected onCellSelected;
//public delegate void OnCellSelected();
//public static event Action<Net_OnAuthFail> OnAuthFail;
//_img.transform.DOScale(.90f,.05f).SetEase(Ease.OutBounce);

