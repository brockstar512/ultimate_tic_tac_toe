using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using System;
using DG.Tweening;
using System.Drawing;

public class OfflineMicroBoardManager : MonoBehaviour, IBoard
{

    public MarkType[,] Grid { get; private set; }
    public Button _button { get; private set; }
    public byte _index { get; private set; }
    public byte _row { get; private set; }
    public byte _col { get; private set; }
    private Transform _placements;
    CanvasGroup cg;
    private Dictionary<int, OfflineCell> _cells;
    public event Action<int, int, MarkType> markBoard;
    public event Action onCellSelected;
    private Image _mark;
    [SerializeField] AudioClip _cellPressedFx;


    //when a cell is selected this read its own board to see if the user won this micro board
    //then destroy that boards inspect controller
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
        OfflineRoundOverManager.reset += Reset;
    }

    void InitializeMicroBoard()
    {
        this.gameObject.name = $"Board: [{_row},{_col}]";
        _cells = new Dictionary<int, OfflineCell>();

        for (int i = 0; i < _placements.childCount; i++)
        {
            OfflineCell cell = _placements.GetChild(i).GetComponent<OfflineCell>();
            cell.Init((byte)i);
            _cells.Add(i, cell);
            //cell.onCellSelected += ReadBoard;
            cell.markCell += MarkCell;

        }
    }

    void MarkCell(int row, int col)
    {
        //Debug.Log($"Marking cell for grid {gameObject.name}");
        Grid[row, col] = OfflineGameManager.Instance.GetCurrentType;
        SoundManager.Instance.PlaySound(_cellPressedFx);

        //Debug.Log($"Here is the board we are reading {this.gameObject.name}");
        var (isDone, lineType) = Utilities.CheckWin((byte)row,(byte)col, Grid);//this should be the 

        if (isDone)
        {
            StopCoroutine(ResetView(.25f));
            StartCoroutine(ResetView(.25f));

            for(int i = 0; i < _cells.Count; i++)
            {
                _cells[i].Reset();
            }

            _button.interactable = false;
            Debug.Log("CG CHANGE");
            cg.blocksRaycasts = false;
            Debug.Log("This board is done");
            markBoard?.Invoke(_row, _col, OfflineGameManager.Instance.GetCurrentType);
            Vector3 size = new Vector3(.90f, .90f, .90f);
            _mark.color = OfflineGameManager.Instance.GetColor;
            _mark.enabled = true;
            _mark.transform.DOScale(size, .15f);
            //do whatever animations you need
            


        }
        else if (Utilities.IsDraw(_cells))
        {
            Debug.Log("This board is done with a draw");
            _button.interactable = false;
            markBoard?.Invoke(_row, _col, MarkType.None);

            StopCoroutine(ResetView(.1f));
            StartCoroutine(ResetView(.1f));

        }
        else
        {
            Debug.Log("you can keep going");//switch turns
            StopCoroutine(ResetView(.1f));
            StartCoroutine(ResetView(.1f));
            //OfflineGameManager.Instance.UpdateTurn();

        }

        OfflineTimeManager.Instance.StopTimer();
        OfflineGameManager.Instance.UpdateTurn();

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
        Debug.Log("CG CHANGE");


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
        OfflineRoundOverManager.reset -= Reset;
    }


}
//void UpdateBoard(Net_OnMarkCell msg)
//{
//    _cells[msg.Index].UpdateUI(msg.Actor);
//}