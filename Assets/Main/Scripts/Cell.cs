using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public struct Index
    {
        public Index(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x, y;
    }
    public enum Stone
    {
        None,
        Black,
        White
    }

    #region Field
    [SerializeField]
    private RectTransform _rectTransform = null;
    [SerializeField]
    private Button _button = null;
    [SerializeField]
    private Color _defaultColor = Color.green;
    [SerializeField]
    private Color _activateColor = Color.red;
    public Index _index;
    private Stone _stone = Stone.None;
    private GameObject _blackStone = null;
    private GameObject _whiteStone = null;

    public RectTransform rectTransform { get => _rectTransform; }
    public Index index { get => _index; set => _index = value; }
    public Stone stone { get => _stone; set => _stone = value; }
    #endregion


    private void Awake()
    {
        // �F�ݒ�
        var colors = _button.colors;
        colors.normalColor = _defaultColor;
        colors.disabledColor = _defaultColor;
        _button.colors = colors;

        // �ΐ���
        _blackStone = Instantiate(GameBoard.Instance.blackStone, transform);
        _whiteStone = Instantiate(GameBoard.Instance.whiteStone, transform);
        _blackStone.SetActive(false);
        _whiteStone.SetActive(false);
        _stone = Stone.None;
    }

    /// <summary>
    /// �΂�u��
    /// </summary>
    public void PlaceStone()
    {
        PlaceStone(GameBoard.Instance.turn);
    }

    /// <summary>
    /// �΂�u��
    /// </summary>
    /// <param name="turn">���݂̃^�[��</param>
    public void PlaceStone(GameBoard.Turn turn)
    {
        switch (turn)
        {
            case GameBoard.Turn.Black:
                _blackStone.SetActive(true);
                _whiteStone?.SetActive(false);
                stone = Stone.Black;
                break;
            case GameBoard.Turn.White:
                _whiteStone.SetActive(true);
                _blackStone?.SetActive(false);
                stone = Stone.White;
                break;
        }
        _button.interactable = false;
        GameBoard.Instance.ReverseStones(this);
        GameBoard.Instance.SwitchTurn();
    }

    /// <summary>
    /// �΂𔽓]������
    /// </summary>
    public void Reverse()
    {
        switch (stone)
        {
            case Stone.Black:
                _blackStone.SetActive(false);
                _whiteStone.SetActive(true);
                stone = Stone.White;
                break;
            case Stone.White:
                _whiteStone.SetActive(false);
                _blackStone.SetActive(true);
                stone = Stone.Black;
                break;
        }
    }

    /// <summary>
    /// �΂�u���邩�ǂ���
    /// </summary>
    /// <returns></returns>
    public bool IsDeployable(GameBoard.Turn turn)
    {
        // ��������΂��u����Ă�Ɣz�u�s��
        if (stone != Stone.None)
            return false;

        Stone color = Stone.None;
        switch (turn)
        {
            case GameBoard.Turn.Black:
                color = Stone.Black;
                break;
            case GameBoard.Turn.White:
                color = Stone.White;
                break;
        }
        var cells = GameBoard.Instance.GetEndCell(this, color);
        for (int i = 0; i < (int)GameBoard.Direction.MAX; ++i)
        {
            // ������ł����Ԃ����Ƃ��ł���Δz�u�\
            if (cells[i] != null)
                return true;
        }

        return false;
    }


    public void Activate()
    {
        var colors = _button.colors;
        colors.normalColor = _activateColor;
        colors.disabledColor = _activateColor;
        _button.colors = colors;
        _button.interactable = true;
    }

    public void InActivate()
    {
        var colors = _button.colors;
        colors.normalColor = _defaultColor;
        colors.disabledColor = _defaultColor;
        _button.colors = colors;
        _button.interactable = false;
    }
}
