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
    /// ゲーム盤の大きさをゲーム画面に合わせる
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
    /// セルを配置
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

        // 初期石を配置
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
    /// セルをクリア
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
    /// ターン交代
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
        // 石が置かれていなければ終了
        if (start.stone == null)
            return;

        var startStoneColor = start.stone;
        Cell.Index endID;
        //bool reverce = false;

        var endCells = GetEndCell(start);

        // 左上
        if (endCells[(int)Direction.LeftUp] != null)
        {
            endID = endCells[(int)Direction.LeftUp].index;
            // 裏返し実行
            int x = start.index.x - 1;
            for (int y = start.index.y + 1; y < endID.y; ++y)
            {
                _cells[x, y].Reverse();
                --x;
            }
        }

        // 上
        if (endCells[(int)Direction.Up] != null)
        {
            endID = endCells[(int)Direction.Up].index;
            for (int y = start.index.y + 1; y < endID.y; ++y)
            {
                _cells[endID.x, y].Reverse();
            }
        }

        // 右上
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

        // 左
        if (endCells[(int)Direction.Left] != null)
        {
            endID = endCells[(int)Direction.Left].index;
            for (int x = start.index.x - 1; x > endID.x; --x)
            {
                _cells[x, endID.y].Reverse();
            }
        }

        //　右
        if (endCells[(int)Direction.Right] != null)
        {
            endID = endCells[(int)Direction.Right].index;
            for (int x = start.index.x + 1; x < endID.x; ++x)
            {
                _cells[x, endID.y].Reverse();
            }
        }

        // 左下
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

        // 下
        if (endCells[(int)Direction.Down] != null)
        {
            endID = endCells[(int)Direction.Down].index;
            for (int y = start.index.y - 1; y > endID.y; --y)
            {
                _cells[endID.x, y].Reverse();
            }
        }

        // 右下
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
    /// 石を置いた際に反転する終端を取得
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

        // 左上探索
        endIndex = start.index;
        endIndex.x -= 1;
        endIndex.y += 1;
        while (endIndex.x >= 0 && endIndex.y < _grid)
        {
            var leftUpStone = _cells[endIndex.x, endIndex.y].stone;
            // 石がない
            if (leftUpStone == null ||
                 (leftUpStone.color == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (leftUpStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // 裏返し可能
                    reverce = true;
                    break;
                }
            }
            // 左上へ
            --endIndex.x;
            ++endIndex.y;
            ++cnt;
        }
        // 裏返し可能
        if (reverce)
        {
            ret[(int)Direction.LeftUp] = _cells[endIndex.x, endIndex.y];
        }

        // 上探索
        endIndex = start.index;
        endIndex.y += 1;
        reverce = false;
        cnt = 0;
        while (endIndex.y < _grid)
        {
            var upStone = _cells[endIndex.x, endIndex.y].stone;
            // 石がない
            if (upStone == null ||
                 (upStone.color == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (upStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // 裏返し可能
                    reverce = true;
                    break;
                }
            }
            // 一つ上へ
            ++endIndex.y;
            ++cnt;
        }
        // 裏返し可能
        if (reverce)
        {
            ret[(int)Direction.Up] = _cells[endIndex.x, endIndex.y];
        }

        // 右上探索
        endIndex = start.index;
        endIndex.x += 1;
        endIndex.y += 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x < _grid && endIndex.y < _grid)
        {
            var rightUpStone = _cells[endIndex.x, endIndex.y].stone;
            // 石がない
            if (rightUpStone == null ||
                 (rightUpStone.color == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightUpStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // 裏返し可能
                    reverce = true;
                    break;
                }
            }
            // 右上へ
            ++endIndex.x;
            ++endIndex.y;
            ++cnt;
        }
        // 裏返し可能
        if (reverce)
        {
            ret[(int)Direction.RightUp] = _cells[endIndex.x, endIndex.y];
        }

        // 左探索
        endIndex = start.index;
        endIndex.x -= 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x >= 0)
        {
            var leftStone = _cells[endIndex.x, endIndex.y].stone;
            // 石がない
            if (leftStone == null ||
                (leftStone.color == startStoneColor && cnt == 0))
            {
                reverce = false;
                break;
            }
            //同じ色に到達
            if (leftStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // 裏返し可能
                    reverce = true;
                    break;
                }
            }
            // 左へ
            --endIndex.x;
            ++cnt;
        }
        // 裏返し可能
        if (reverce)
        {
            ret[(int)Direction.Left] = _cells[endIndex.x, endIndex.y];
        }

        // 右探索
        endIndex = start.index;
        endIndex.x += 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x < _grid)
        {
            var rightStone = _cells[endIndex.x, endIndex.y].stone;
            // 石がない
            if (rightStone == null ||
                 (rightStone.color == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // 裏返し可能
                    reverce = true;
                    break;
                }
            }
            // 右へ
            ++endIndex.x;
            ++cnt;
        }
        // 裏返し可能
        if (reverce)
        {
            ret[(int)Direction.Right] = _cells[endIndex.x, endIndex.y];
        }


        // 左下探索
        endIndex = start.index;
        endIndex.x -= 1;
        endIndex.y -= 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x >= 0 && endIndex.y >= 0)
        {
            var leftDownStone = _cells[endIndex.x, endIndex.y].stone;
            // 石がない
            if (leftDownStone == null ||
                 (leftDownStone.color == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (leftDownStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // 裏返し可能
                    reverce = true;
                    break;
                }
            }
            // 左下へ
            --endIndex.x;
            --endIndex.y;
            ++cnt;
        }
        // 裏返し可能
        if (reverce)
        {
            ret[(int)Direction.LeftDown] = _cells[endIndex.x, endIndex.y];
        }

        // 下探索
        endIndex = start.index;
        endIndex.y -= 1;
        reverce = false;
        cnt = 0;
        while (endIndex.y >= 0)
        {
            var downStone = _cells[endIndex.x, endIndex.y].stone;
            // 石がない
            if (downStone == null ||
                 (downStone.color == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (downStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // 裏返し可能
                    reverce = true;
                    break;
                }
            }
            // 一つ下へ
            --endIndex.y;
            ++cnt;
        }
        // 裏返し可能
        if (reverce)
        {
            ret[(int)Direction.Down] = _cells[endIndex.x, endIndex.y];
        }

        // 右下探索
        endIndex = start.index;
        endIndex.x += 1;
        endIndex.y -= 1;
        reverce = false;
        cnt = 0;
        while (endIndex.x < _grid && endIndex.y >= 0)
        {
            var rightDownStone = _cells[endIndex.x, endIndex.y].stone;
            // 石がない
            if (rightDownStone == null ||
                 (rightDownStone.color == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightDownStone.color == startStoneColor)
            {
                if (cnt > 0)
                {
                    // 裏返し可能
                    reverce = true;
                    break;
                }
            }
            // 右下へ
            ++endIndex.x;
            --endIndex.y;
            ++cnt;
        }
        // 裏返し可能
        if (reverce)
        {
            ret[(int)Direction.RightDown] = _cells[endIndex.x, endIndex.y];
        }

        return ret;
    }
}
