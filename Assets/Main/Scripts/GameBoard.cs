using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform = null;
    [SerializeField]
    private Cell cellProt = null;
    [SerializeField, Min(4)]
    private int _grid = 8;
    private int gird
    {
        set
        {
            _grid = value;
            if (_grid % 2 == 1) ++_grid;
        }
    }

    private Cell[,] cells = null;


    private void Start()
    {
        if (cells == null)
            cells = new Cell[_grid, _grid];

        var aspectData = CameraStableAspectData.Entity;
        FitGameBoard(aspectData);
        PlaceCells(aspectData);
    }

    private void FitGameBoard(CameraStableAspectData aspectData)
    {
        float width = Mathf.Max(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        Debug.Log("ohira width" + width);
        float boardSize = Mathf.Min(aspectData.targetResolution.width, aspectData.targetResolution.height);
        boardSize /= width;
        rectTransform.localScale = new Vector3(boardSize, boardSize, 1.0f);
        Debug.Log("ohira boardSIze" + boardSize);
    }

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
                var cell = Instantiate(cellProt, transform);
                cell.rectTransform.localScale = new Vector3(cellSize, cellSize, 1.0f);
                cell.rectTransform.localPosition = new Vector3(x * cellSize, y * cellSize, cell.rectTransform.localPosition.z) + anchor;
                cell.id = new Cell.ID(x, y);
                cell.name = "cell " + x + "," + y;
                cells[x, y] = cell;
            }
        }
    }

    private void ClearCells()
    {
        foreach (var cell in cells)
        {
            if (cell != null)
                Destroy(cell.gameObject);
        }
    }



    class AdjacentCellInfo
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

    AdjacentCellInfo GetAdjacentCell(Cell self)
    {
        AdjacentCellInfo adjacent = new AdjacentCellInfo();
        int x, y;

        // 上を調べる
        y = self.id.y + 1;
        if (y < _grid)
        {
            // 左上
            x = self.id.x - 1;
            if (x > 0)
                adjacent.cells[(int)AdjacentCellInfo.Direction.LeftUp] = cells[x, y];
            // 真上
            x = self.id.x;
            adjacent.cells[(int)AdjacentCellInfo.Direction.Up] = cells[x, y];
            // 右上
            x = self.id.x + 1;
            if (x > _grid)
                adjacent.cells[(int)AdjacentCellInfo.Direction.RightUp] = cells[x, y];
        }

        y = self.id.y;
        // 真左
        x = self.id.x - 1;
        if (x > 0)
            adjacent.cells[(int)AdjacentCellInfo.Direction.Left] = cells[x, y];
        // 真右
        x = self.id.x + 1;
        if (x < _grid)
            adjacent.cells[(int)AdjacentCellInfo.Direction.Right] = cells[x, y];

        // 下を調べる
        y = self.id.y - 1;
        if (y > 0)
        {
            // 左下
            x = self.id.x - 1;
            if (x > 0)
                adjacent.cells[(int)AdjacentCellInfo.Direction.LeftDown] = cells[x, y];
            // 真下
            x = self.id.x;
            adjacent.cells[(int)AdjacentCellInfo.Direction.Down] = cells[x, y];
            // 右下
            x = self.id.x + 1;
            if (x < _grid)
                adjacent.cells[(int)AdjacentCellInfo.Direction.RightDown] = cells[x, y];
        }

        return adjacent;
    }
}
