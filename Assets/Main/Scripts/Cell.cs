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
    private Stone _stone = null;

    public RectTransform rectTransform { get => _rectTransform; }
    public Index index { get => _index; set => _index = value; }
    public Stone stone { get => _stone; private set => _stone = value; }
    #endregion


    private void Awake()
    {
        // �F�ݒ�
        var colors = _button.colors;
        colors.normalColor = _defaultColor;
        colors.disabledColor = _defaultColor;
        _button.colors = colors;
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
                stone = Stone.Place(this, Stone.StoneColor.Black);
                break;
            case GameBoard.Turn.White:
                stone = Stone.Place(this, Stone.StoneColor.White);
                break;
        }
        _button.interactable = false;
        GameBoard.Instance.ReverseStones(this);
        Debug.Log("ohira aaaaa");
        GameBoard.Instance.SwitchTurn();
    }

    /// <summary>
    /// �΂𔽓]������
    /// </summary>
    public void Reverse()
    {
        _stone.Reverse();
    }

    /// <summary>
    /// �΂�u���邩�ǂ���
    /// </summary>
    /// <returns></returns>
    public bool IsDeployable(GameBoard.Turn turn)
    {
        // ��������΂��u����Ă�Ɣz�u�s��
        if (stone != null)
            return false;

        var cells = GameBoard.Instance.GetEndCell(this);
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
