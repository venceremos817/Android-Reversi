using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public struct ID
    {
        public ID(int x, int y)
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
    public ID _id;
    private Stone _stone = Stone.None;
    private GameObject _blackStone = null;
    private GameObject _whiteStone = null;

    public RectTransform rectTransform { get => _rectTransform; }
    public ID id { get => _id; set => _id = value; }
    public Stone stone { get => _stone; set => _stone = value; }
    #endregion


    private void Start()
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
    private void PlaceStone(GameBoard.Turn turn)
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
}
