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

    public TextMeshProUGUI text;

    [SerializeField]
    private RectTransform rectTransform = null;
    [SerializeField]
    private Cell _cellProt = null;
    [SerializeField]
    private GameObject _blackStone = null;
    [SerializeField]
    private GameObject _whiteStone = null;
    [SerializeField, Min(4)]
    private int _grid = 8;

    private Cell[,] _cells = null;
    private Turn _turn = Turn.Black;

    public GameObject blackStone
    {
        get { return _blackStone; }
    }
    public GameObject whiteStone
    {
        get { return _whiteStone; }
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
        float width = Mathf.Max(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        float boardSize = Mathf.Min(aspectData.targetResolution.width, aspectData.targetResolution.height);
        boardSize /= width;
        rectTransform.localScale = new Vector3(boardSize, boardSize, 1.0f);
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

        Cell.Index center;
        center.x = _grid / 2;
        center.y = _grid / 2;
        _cells[center.x, center.y].PlaceStone(Turn.Black);
        _cells[center.x - 1, center.y].PlaceStone(Turn.White);
        _cells[center.x, center.y - 1].PlaceStone(Turn.White);
        _cells[center.x - 1, center.y - 1].PlaceStone(Turn.Black);
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
#if false
    /// <summary>
    /// 隣接したセルの情報群
    /// </summary>
    public class AdjacentCellInfo
    {
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
        public Cell[] cells = new Cell[(int)Direction.MAX];
    }

    /// <summary>
    /// 隣接したセルの情報を取得
    /// </summary>
    /// <param name="self">隣接を調べたいセル</param>
    /// <returns>隣接したセルの情報</returns>
    public AdjacentCellInfo GetAdjacentCell(Cell self)
    {
        AdjacentCellInfo adjacent = new AdjacentCellInfo();
        int x, y;

        // 上を調べる
        y = self.index.y + 1;
        if (y < _grid)
        {
            // 左上
            x = self.index.x - 1;
            if (x > 0)
                adjacent.cells[(int)AdjacentCellInfo.Direction.LeftUp] = _cells[x, y];
            // 真上
            x = self.index.x;
            adjacent.cells[(int)AdjacentCellInfo.Direction.Up] = _cells[x, y];
            // 右上
            x = self.index.x + 1;
            if (x > _grid)
                adjacent.cells[(int)AdjacentCellInfo.Direction.RightUp] = _cells[x, y];
        }

        y = self.index.y;
        // 真左
        x = self.index.x - 1;
        if (x > 0)
            adjacent.cells[(int)AdjacentCellInfo.Direction.Left] = _cells[x, y];
        // 真右
        x = self.index.x + 1;
        if (x < _grid)
            adjacent.cells[(int)AdjacentCellInfo.Direction.Right] = _cells[x, y];

        // 下を調べる
        y = self.index.y - 1;
        if (y > 0)
        {
            // 左下
            x = self.index.x - 1;
            if (x > 0)
                adjacent.cells[(int)AdjacentCellInfo.Direction.LeftDown] = _cells[x, y];
            // 真下
            x = self.index.x;
            adjacent.cells[(int)AdjacentCellInfo.Direction.Down] = _cells[x, y];
            // 右下
            x = self.index.x + 1;
            if (x < _grid)
                adjacent.cells[(int)AdjacentCellInfo.Direction.RightDown] = _cells[x, y];
        }

        return adjacent;
    }
#endif

    public void ReverseStones(Cell start)
    {
        var startStoneColor = start.stone;
        Cell.Index endID;
        //bool reverce = false;

        var endCells = GetEndCell(start, start.stone);

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
#if false
        // 左上探索
        endID = start.index;
        endID.x -= 1;
        endID.y += 1;
        while (endID.x >= 0 && endID.y < _grid)
        {
            var leftUpStone = _cells[endID.x, endID.y].stone;
            // 石がない
            if (leftUpStone == Cell.Stone.None)
            {
                // 裏返し失敗
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (leftUpStone == startStoneColor)
            {
                // 裏返し成功
                reverce = true;
                break;
            }
            // 左上へ
            --endID.x;
            ++endID.y;
        }
        // 裏返し実行
        if (reverce)
        {
            int x = start.index.x - 1;
            for (int y = start.index.y + 1; y < endID.y; ++y)
            {
                _cells[x, y].Reverse();
                --x;
            }
        }

        // 上探索
        endID = start.index;
        endID.y += 1;
        reverce = false;
        while (endID.y < _grid)
        {
            var upStone = _cells[endID.x, endID.y].stone;
            // 石がない
            if (upStone == Cell.Stone.None)
            {
                // 裏返し失敗
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (upStone == startStoneColor)
            {
                // 裏返し成功
                reverce = true;
                break;
            }
            // 一つ上へ
            ++endID.y;
        }
        // 裏返し実行
        if (reverce)
        {
            for (int y = start.index.y + 1; y < endID.y; ++y)
            {
                _cells[endID.x, y].Reverse();
            }
        }

        // 右上探索
        endID = start.index;
        endID.x += 1;
        endID.y += 1;
        reverce = false;
        while (endID.x < _grid && endID.y < _grid)
        {
            var rightUpStone = _cells[endID.x, endID.y].stone;
            // 石がない
            if (rightUpStone == Cell.Stone.None)
            {
                // 裏返し失敗
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightUpStone == startStoneColor)
            {
                // 裏返し成功
                reverce = true;
                break;
            }
            // 右上へ
            ++endID.x;
            ++endID.y;
        }
        // 裏返し実行
        if (reverce)
        {
            int x = start.index.x + 1;
            for (int y = start.index.y + 1; y < endID.y; ++y)
            {
                _cells[x, y].Reverse();
                ++x;
            }
        }

        // 左探索
        endID = start.index;
        endID.x -= 1;
        reverce = false;
        while (endID.x >= 0)
        {
            var leftStone = _cells[endID.x, endID.y].stone;
            // 石がない
            if (leftStone == Cell.Stone.None)
            {
                reverce = false;
                break;
            }
            //同じ色に到達
            if (leftStone == startStoneColor)
            {
                // 裏返し成功
                reverce = true;
                break;
            }
            // 左へ
            --endID.x;
        }
        // 裏返し実行
        if (reverce)
        {
            for (int x = start.index.x - 1; x > endID.x; --x)
            {
                _cells[x, endID.y].Reverse();
            }
        }

        // 右探索
        endID = start.index;
        endID.x += 1;
        reverce = false;
        while (endID.x < _grid)
        {
            var rightStone = _cells[endID.x, endID.y].stone;
            // 石がない
            if (rightStone == Cell.Stone.None)
            {
                // 裏返し失敗
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightStone == startStoneColor)
            {
                // 裏返し成功
                reverce = true;
                break;
            }
            // 右へ
            ++endID.x;
        }
        // 裏返し実行
        if (reverce)
        {
            for (int x = start.index.x + 1; x < endID.x; ++x)
            {
                _cells[x, endID.y].Reverse();
            }
        }


        // 左下探索
        endID = start.index;
        endID.x -= 1;
        endID.y -= 1;
        reverce = false;
        while (endID.x >= 0 && endID.y >= 0)
        {
            var leftDownStone = _cells[endID.x, endID.y].stone;
            // 石がない
            if (leftDownStone == Cell.Stone.None)
            {
                // 裏返し失敗
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (leftDownStone == startStoneColor)
            {
                // 裏返し成功
                reverce = true;
                break;
            }
            // 左下へ
            --endID.x;
            --endID.y;
        }
        // 裏返し実行
        if (reverce)
        {
            int x = start.index.x - 1;
            for (int y = start.index.y - 1; y > endID.y; --y)
            {
                _cells[x, y].Reverse();
                --x;
            }
        }

        // 下探索
        endID = start.index;
        endID.y -= 1;
        reverce = false;
        while (endID.y >= 0)
        {
            var downStone = _cells[endID.x, endID.y].stone;
            // 石がない
            if (downStone == Cell.Stone.None)
            {
                // 裏返し失敗
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (downStone == startStoneColor)
            {
                // 裏返し成功
                reverce = true;
                break;
            }
            // 一つ下へ
            --endID.y;
        }
        // 裏返し実行
        if (reverce)
        {
            for (int y = start.index.y - 1; y > endID.y; --y)
            {
                _cells[endID.x, y].Reverse();
            }
        }

        // 右下探索
        endID = start.index;
        endID.x += 1;
        endID.y -= 1;
        reverce = false;
        while (endID.x < _grid && endID.y >= 0)
        {
            var rightDownStone = _cells[endID.x, endID.y].stone;
            // 石がない
            if (rightDownStone == Cell.Stone.None)
            {
                // 裏返し失敗
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightDownStone == startStoneColor)
            {
                // 裏返し成功
                reverce = true;
                break;
            }
            // 右下へ
            ++endID.x;
            --endID.y;
        }
        // 裏返し実行
        if (reverce)
        {
            int x = start.index.x + 1;
            for (int y = start.index.y - 1; y > endID.y; --y)
            {
                _cells[x, y].Reverse();
                ++x;
            }
        }
#endif
    }


    /// <summary>
    /// 石を置いた際に反転する終端を取得
    /// </summary>
    /// <param name="start"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public Cell[] GetEndCell(Cell start, Cell.Stone color)
    {
        Cell[] ret = new Cell[(int)Direction.MAX];
        for (int i = 0; i < (int)Direction.MAX; ++i)
            ret[i] = null;

        var startStoneColor = color;
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
            if (leftUpStone == Cell.Stone.None ||
                 (leftUpStone == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (leftUpStone == startStoneColor)
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
            if (upStone == Cell.Stone.None ||
                 (upStone == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (upStone == startStoneColor)
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
            if (rightUpStone == Cell.Stone.None ||
                 (rightUpStone == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightUpStone == startStoneColor)
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
            if (leftStone == Cell.Stone.None ||
                (leftStone == startStoneColor && cnt == 0))
            {
                reverce = false;
                break;
            }
            //同じ色に到達
            if (leftStone == startStoneColor)
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
            if (rightStone == Cell.Stone.None ||
                 (rightStone == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightStone == startStoneColor)
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
            if (leftDownStone == Cell.Stone.None ||
                 (leftDownStone == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (leftDownStone == startStoneColor)
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
            if (downStone == Cell.Stone.None ||
                 (downStone == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (downStone == startStoneColor)
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
            if (rightDownStone == Cell.Stone.None ||
                 (rightDownStone == startStoneColor && cnt == 0))
            {
                // 裏返し不可
                reverce = false;
                break;
            }
            // 同じ色に到達
            if (rightDownStone == startStoneColor)
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
