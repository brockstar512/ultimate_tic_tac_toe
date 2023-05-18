using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Enums;


public class MicroBoardManager : MonoBehaviour, IBoard
{

    public MarkType[,] Grid { get; private set; }
    public Button _button { get; private set; }
    public byte _index { get; private set; }
    public byte _row { get; private set; }
    public byte _col { get; private set; }
    public CanvasGroup cg { get; private set; }
    private Transform _placements;
    public Dictionary<int, Cell> _cells { get; private set; }
    public event Action<int, int, MarkType> markBoard;
    public event Action onCellSelected;
    private Image _mark;
    [SerializeField] AudioClip _cellPressedFx;


    public void Init(byte index)
    {
        cg = GetComponent<CanvasGroup>();
        _placements = this.transform.GetChild(1).GetComponent<Transform>();
        _mark = this.transform.GetChild(2).GetComponent<Image>();
        Grid = new MarkType[Utilities.GRID_SIZE, Utilities.GRID_SIZE];
        _index = index;
        _button = GetComponent<Button>();
        _row = (byte)(index / 3);
        _col = (byte)(index % 3);
        InitializeMicroBoard();
        RoundOverManager.reset += Reset;
    }

    void InitializeMicroBoard()
    {
        this.gameObject.name = $"Board: [{_row},{_col}]";
        _cells = new Dictionary<int, Cell>();

        for (int i = 0; i < _placements.childCount; i++)
        {
            Cell cell = _placements.GetChild(i).GetComponent<Cell>();
            cell.Init((byte)i, _index);
            _cells.Add(i, cell);
            //cell.onCellSelected += ReadBoard;
            cell.markCell += MarkCell;

        }
    }

    public void FailedValidation(byte cellIndex, byte markTypeOwner)
    {
        //if it failed validation it's probabaly because its owned by the opponeet
        _row = (byte)(cellIndex / 3);
        _col = (byte)(cellIndex % 3);
        Grid[_row, _col] = (MarkType)markTypeOwner;

        //in case it is not there turn
        if ((MarkType)markTypeOwner == MarkType.None)
        {
            _cells[cellIndex]._mark.transform.localScale = new Vector3(.25f, .25f, .25f);
            _cells[cellIndex]._mark.enabled = false;
            _cells[cellIndex]._button.interactable = true; 
            return;
        }

        _cells[cellIndex]._mark.color =
            markTypeOwner == GameManager.Instance.myPlayer.MyType.Value ?
            GameManager.Instance.myPlayer.GetMyColor : GameManager.Instance.myPlayer.GetOpponentColor; 
    }

    void MarkCell(int row, int col)
    {
        SoundManager.Instance.PlaySound(_cellPressedFx);

        Grid[row, col] = GameManager.Instance.GetMarkType;

        var (isDone, lineType) = Utilities.CheckWin((byte)row,(byte)col, Grid);

        if (isDone)
        {
            StopCoroutine(ResetView(.25f));
            StartCoroutine(ResetView(.25f));

            for(int i = 0; i < _cells.Count; i++)
            {
                _cells[i].Reset();
            }

            _button.interactable = false;
            cg.blocksRaycasts = false;
            _mark.color = GameManager.Instance.GetColor;
            markBoard?.Invoke(_row, _col, GameManager.Instance.GetMarkType);
            Vector3 size = new Vector3(.90f, .90f, .90f);


            _mark.enabled = true;
            _mark.transform.DOScale(size, .15f);
            return;
            
        }
        else if (Utilities.IsDraw(_cells))
        {
            Debug.Log("This board is done with a draw");
            _button.interactable = false;
            markBoard?.Invoke(_row, _col, MarkType.None);
            
        }


        StopCoroutine(ResetView(.1f));
        StartCoroutine(ResetView(.1f));
        
    }

    IEnumerator ResetView(float pause)
    {
        yield return new WaitForSeconds(pause);
        onCellSelected?.Invoke();
        yield return null;
    }

    private void Reset()
    {
        _button.interactable = true;
        cg.blocksRaycasts = true;

        for (int col = 0; col < Grid.GetLength(0); col++)
        {
            for (int row = 0; row < Grid.GetLength(1); row++)
            {
                Grid[row, col] = MarkType.None;
            }
        }
        Vector3 size = new Vector3(.25f, .25f, .25f);
        
        _mark.transform.DOScale(size, .75f).SetEase(Ease.InElastic).OnComplete(()=> _mark.enabled = false);
    }

    private void OnDestroy()
    {
        markBoard = null;
        onCellSelected = null;
    }


}
