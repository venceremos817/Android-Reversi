using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameBoard : SingletonMonoBehaviour<GameBoard>
{
    public enum Turn
    {
        Black,
        White,

        MAX
    };


    #region Field
    [SerializeField]
    private RectTransform _rectTransform = null;
    [SerializeField]
    private Cell _cellProt = null;
    [SerializeField]
    private Stone _stoneProt = null;
    [SerializeField, Min(4)]
    private int _grid = 8;
    [SerializeField]
    private TextMeshProUGUI text;

    private Cell[,] _cells = null;
    private Turn _turn = Turn.Black;

    public Stone Stone
    {
        get { return _stoneProt; }
    }
    private int gird
    {
        set
        {
            _grid = value;
            if (_grid % 2 == 1) ++_grid;
        }
    }
    public Turn turn
    {
        get { return _turn; }
    }
    #endregion

    private void Start()
    {
        if (_cells == null)
            _cells = new Cell[_grid, _grid];

        var aspectData = CameraStableAspectData.Entity;
        FitGameBoard(aspectData);
        PlaceCells(aspectData);
    }

    /// <summary>
    /// �Q�[���Ղ̑傫�����Q�[����ʂɍ��킹��
    /// </summary>
    /// <param name="aspectData"></param>
    private void FitGameBoard(CameraStableAspectData aspectData)
    {
        float width = Mathf.Max(_rectTransform.sizeDelta.x, _rectTransform.sizeDelta.y);
        float boardSize = Mathf.Min(aspectData.targetResolution.width, aspectData.targetResolution.height);
        boardSize /= width;
        _rectTransform.localScale = new Vector3(boardSize, boardSize, 1.0f);
    }

    /// <summary>
    /// �Z����z�u
    /// </summary>
    /// <param name="aspectData"></param>
    [ContextMenu("PlaceCells")]
    private void PlaceCells(CameraStableAspectData aspectData)
    {
        float cellSize = 1.0f / _grid;
        Vector3 anchor = new Vector3(-cellSize * (_grid - 1) * 0.5f, -cellSize * (_grid - 1) * 0.5f, 0.0f);

        ClearCells();

        for (int y = 0; y < _grid; ++y)
        {
            for (int x = 0; x < _grid; ++x)
            {
                var cell = Instantiate(_cellProt, transform);
                cell.rectTransform.localScale = new Vector3(cellSize, cellSize, 1.0f);
                cell.rectTransform.localPosition = new Vector3(x * cellSize, y * cellSize, cell.rectTransform.localPosition.z) + anchor;
                cell.index = new Cell.Index(x, y);
                cell.name = "cell " + x + "," + y;
                _cells[x, y] = cell;
            }
        }

        // �����΂�z�u
        Cell.Index center;
        center.x = _grid / 2;
        center.y = _grid / 2;
        Debug.Log("ohira 0");
        _cells[center.x, center.y].PlaceStone(Turn.Black);
        Debug.Log("ohira 1");
        _cells[center.x - 1, center.y].PlaceStone(Turn.White);
        Debug.Log("ohira 2");
        _cells[center.x - 1, center.y - 1].PlaceStone(Turn.Black);
        Debug.Log("ohira 3");
        _cells[center.x, center.y - 1].PlaceStone(Turn.White);
        Debug.Log("ohira 4");
    }

    /// <summary>
    /// �Z�����N���A
    /// </summary>
    private void ClearCells()
    {
        for (int y = 0; y < _grid; ++y)
            for (int x = 0; x < _grid; ++x)
            {
                if (_cells[x, y] != null)
                    Destroy(_cells[x, y].gameObject);
                _cells[x, y] = null;
            }
    }

    /// <summary>
    /// �^�[�����
    /// </summary>
    public void SwitchTurn()
    {
        _turn = (Turn)(((int)_turn + 1) % (int)Turn.MAX);
        switch (_turn)
        {
            case Turn.Black:
                text.text = "Black Turn";
                text.color = Color.black;
                break;
            case Turn.White:
                text.text = "White Turn";
                text.color = Color.white;
                break;
            case Turn.MAX:
                text.text = "Error";
                text.color = Color.red;
                break;
        }

        foreach (var cell in _cells)
        {
            cell.InActivate();
            if (cell.IsDeployable(turn))
                cell.Activate();
        }
    }

    public enum Direction
    {
        LeftUp,
        Up,
        RightUp,
        Left,
        Right,
        LeftDown,
        Down,
        RightDown,

        MAX
    };

    public void ReverseStones(Cell start)
    {
        // �΂��u����Ă��Ȃ���ΏI��
        if (start.stone == null)
            return;

        var startStoneColor = start.stone;
        Cell.Index endID;
        //bool reverce = false;

        var endCells = GetEndCell(start);

        // ����
        if (endCells[(int)Direction.LeftUp] != null)
        {
            endID = endCells[(int)Direction.LeftUp].index;
            // ���Ԃ����s
            int x = start.index.x - 1;
            for (int y = start.index.y + 1; y < endID.y; ++y)
            {
                _cells[x, y].Reverse();
                --x;
            }
        }

        // ��
        if (endCells[(int)Direction.Up] != null)
        {
            endID = endCells[(int)Direction.Up].index;
            for (int y = start.index.y + 1; y < endID.y; ++y)
            {
                _cells[endID.x, y].Reverse();
            }
        }

        // �E��
        if (endCells[(int)Direction.RightUp] != null)
        {
            endID = endCells[(int)Direction.RightUp].index;
            int x = start.index.x + 1;
            for (int y = start.index.y + 1; y < endID.y; ++y)
            {
                _cells[x, y].Reverse();
                ++x;
            }
        }

        // ��
        if (endCells[(int)Direction.Left] != null)
        {
            endID = endCells[(int)Direction.Left].index;
            for (int x = start.index.x - 1; x > endID.x; --x)
            {
                _cells[x, endID.y].Reverse();
            }
        }

        //�@�E
        if (endCells[(int)Direction.Right] != null)
        {
            endID = endCells[(int)Direction.Right].index;
            for (int x = start.index.x + 1; x < endID.x; ++x)
            {
                _cells[x, endID.y].Reverse();
            }
        }

        // ����
        if (endCells[(int)Direction.LeftDown] != null)
        {
            endID = endCells[(int)Direction.LeftDown].index;
            int x = start.index.x - 1;
            for (int y = start.index.y - 1; y > endID.y; --y)
            {
                _cells[x, y].Reverse();
                --x;
            }
        }

        // ��
        if (endCells[(int)Direction.Down] != null)
        {
            endID = endCells[(int)Direction.Down].index;
            for (int y = start.index.y - 1; y > endID.y; --y)
            {
                _cells[endID.x, y].Reverse();
            }
        }

        // �E��
        if (endCells[(int)Direction.RightDown] != null)
        {
            endID = endCells[(int)Direction.RightDown].index;
            int x = start.index.x + 1;
            for (int y = start.index.y - 1; y > endID.y; --y)
            {
                _cells[x, y].Reverse();
                ++x;
            }
        }
    }


    /// <summary>
    /// �΂�u�����ۂɔ��]����I�[���擾
    /// </summary>
    /// <param name="start"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public Cell[] GetEndCell(Cell start)
    {
        Cell[] ret = new Cell[(int)Direction.MAX];
        for (int i = 0; i < (int)Direction.MAX; ++i)
            ret[i] = null;

        Stone.StoneColor startStoneColor = Stone.StoneColor.Black;
        if (start.stone != null)
        {
            startStoneColor = start.stone.color;
        }
        else
        {
            switch (turn)
            {
                case Turn.Black:
                    startStoneColor = Stone.StoneColor.Black;
                    break;
                case Turn.White:
                    startStoneColor = Stone.StoneColor.White;
                    break;
            }
        }
        Cell.Index endIndex;
        bool reverce = false;
        int cnt = 0;

        // ����T��
        endIndex = start.index;
        endIndex.x -= 1;
        endIndex.y += 1;
        while (endIndex.x >= 0 && endIndex.y < _grid)
        {
            var leftUpStone = _cells[endIndex.x, endIndex.y].stone;
            // �΂��Ȃ�
            if (leftUpStone == null ||
                 (leftUpStone.color == startStoneColor && cnt == 0))
            {
                // ���Ԃ��s��
                reverce = false;
                break;
            }
            // �����F�ɓ��B
            if (leftUpStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // ���Ԃ��\
                    reverce = true;
                    break;
                }
            }
            // �����
            --endIndex.x;
            ++endIndex.y;
            ++cnt;
        }
        // ���Ԃ��\
        if (reverce)
        {
            ret[(int)Direction.LeftUp] = _cells[endIndex.x, endIndex.y];
        }

        // ��T��
        endIndex = start.index;
        endIndex.y += 1;
        reverce = false;
        cnt = 0;
        while (endIndex.y < _grid)
        {
            var upStone = _cells[endIndex.x, endIndex.y].stone;
            // �΂��Ȃ�
            if (upStone == null ||
                 (upStone.color == startStoneColor && cnt == 0))
            {
                // ���Ԃ��s��
                reverce = false;
                break;
            }
            // �����F�ɓ��B
            if (upStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // ���Ԃ��\
                    reverce = true;
                    break;
                }
            }
            // ����
            ++endIndex.y;
            ++cnt;
        }
        // ���Ԃ��\
        if (reverce)
        {
            ret[(int)Direction.Up] = _cells[endIndex.x, endIndex.y];
        }

        // �E��T��
        endIndex = start.index;
        endIndex.x += 1;
        endIndex.y += 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x < _grid && endIndex.y < _grid)
        {
            var rightUpStone = _cells[endIndex.x, endIndex.y].stone;
            // �΂��Ȃ�
            if (rightUpStone == null ||
                 (rightUpStone.color == startStoneColor && cnt == 0))
            {
                // ���Ԃ��s��
                reverce = false;
                break;
            }
            // �����F�ɓ��B
            if (rightUpStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // ���Ԃ��\
                    reverce = true;
                    break;
                }
            }
            // �E���
            ++endIndex.x;
            ++endIndex.y;
            ++cnt;
        }
        // ���Ԃ��\
        if (reverce)
        {
            ret[(int)Direction.RightUp] = _cells[endIndex.x, endIndex.y];
        }

        // ���T��
        endIndex = start.index;
        endIndex.x -= 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x >= 0)
        {
            var leftStone = _cells[endIndex.x, endIndex.y].stone;
            // �΂��Ȃ�
            if (leftStone == null ||
                (leftStone.color == startStoneColor && cnt == 0))
            {
                reverce = false;
                break;
            }
            //�����F�ɓ��B
            if (leftStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // ���Ԃ��\
                    reverce = true;
                    break;
                }
            }
            // ����
            --endIndex.x;
            ++cnt;
        }
        // ���Ԃ��\
        if (reverce)
        {
            ret[(int)Direction.Left] = _cells[endIndex.x, endIndex.y];
        }

        // �E�T��
        endIndex = start.index;
        endIndex.x += 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x < _grid)
        {
            var rightStone = _cells[endIndex.x, endIndex.y].stone;
            // �΂��Ȃ�
            if (rightStone == null ||
                 (rightStone.color == startStoneColor && cnt == 0))
            {
                // ���Ԃ��s��
                reverce = false;
                break;
            }
            // �����F�ɓ��B
            if (rightStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // ���Ԃ��\
                    reverce = true;
                    break;
                }
            }
            // �E��
            ++endIndex.x;
            ++cnt;
        }
        // ���Ԃ��\
        if (reverce)
        {
            ret[(int)Direction.Right] = _cells[endIndex.x, endIndex.y];
        }


        // �����T��
        endIndex = start.index;
        endIndex.x -= 1;
        endIndex.y -= 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x >= 0 && endIndex.y >= 0)
        {
            var leftDownStone = _cells[endIndex.x, endIndex.y].stone;
            // �΂��Ȃ�
            if (leftDownStone == null ||
                 (leftDownStone.color == startStoneColor && cnt == 0))
            {
                // ���Ԃ��s��
                reverce = false;
                break;
            }
            // �����F�ɓ��B
            if (leftDownStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // ���Ԃ��\
                    reverce = true;
                    break;
                }
            }
            // ������
            --endIndex.x;
            --endIndex.y;
            ++cnt;
        }
        // ���Ԃ��\
        if (reverce)
        {
            ret[(int)Direction.LeftDown] = _cells[endIndex.x, endIndex.y];
        }

        // ���T��
        endIndex = start.index;
        endIndex.y -= 1;
        reverce = false;
        cnt = 0;
        while (endIndex.y >= 0)
        {
            var downStone = _cells[endIndex.x, endIndex.y].stone;
            // �΂��Ȃ�
            if (downStone == null ||
                 (downStone.color == startStoneColor && cnt == 0))
            {
                // ���Ԃ��s��
                reverce = false;
                break;
            }
            // �����F�ɓ��B
            if (downStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // ���Ԃ��\
                    reverce = true;
                    break;
                }
            }
            // �����
            --endIndex.y;
            ++cnt;
        }
        // ���Ԃ��\
        if (reverce)
        {
            ret[(int)Direction.Down] = _cells[endIndex.x, endIndex.y];
        }

        // �E���T��
        endIndex = start.index;
        endIndex.x += 1;
        endIndex.y -= 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x < _grid && endIndex.y >= 0)
        {
            var rightDownStone = _cells[endIndex.x, endIndex.y].stone;
            // �΂��Ȃ�
            if (rightDownStone == null ||
                 (rightDownStone.color == startStoneColor && cnt == 0))
            {
                // ���Ԃ��s��
                reverce = false;
                break;
            }
            // �����F�ɓ��B
            if (rightDownStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // ���Ԃ��\
                    reverce = true;
                    break;
                }
            }
            // �E����
            ++endIndex.x;
            --endIndex.y;
            ++cnt;
        }
        // ���Ԃ��\
        if (reverce)
        {
            ret[(int)Direction.RightDown] = _cells[endIndex.x, endIndex.y];
        }

        return ret;
    }
}
