using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GameField : MonoBehaviour
{
    public GameObject cellPrefab;
    
    private List<Cell> _cells = new List<Cell>();

    private int _fieldSize = 4;
    
    private bool[,] _grid;

    private void Start()
    {
        InitializeField();
        StartCoroutine(CreateCell(0.1f));
        StartCoroutine(CreateCell(0.1f));
    }
    
    private void InitializeField()
    {
        _grid = new bool[_fieldSize, _fieldSize];
    }
    
    private Vector2Int GetEmptyPosition()
    {
        var emptyPositions = new List<Vector2Int>();

        for (var i = 0; i < _fieldSize; i++)
        {
            for (var j = 0; j < _fieldSize; j++)
            {
                if (!_grid[i, j])
                {
                    emptyPositions.Add(new Vector2Int(i, j));
                }
            }
        }

        if (emptyPositions.Count > 0)
        {
            return emptyPositions[Random.Range(0, emptyPositions.Count)];
        }
        
        return new Vector2Int(-1, -1);
    }

    private IEnumerator CreateCell(float probability)
    {
        var position = GetEmptyPosition();

        if (position.x != -1 && position.y != -1)
        {
            Vector3 positionVector = Cell.GetWorldPosition(position);
            var cellViewObject = Instantiate(cellPrefab, positionVector, Quaternion.identity);
        
            var cell = cellViewObject.GetComponent<Cell>();
            if (cell == null)
            {
                cell = cellViewObject.AddComponent<Cell>();
            }
        
            var value = Random.value <= probability ? 2 : 1;
            cell.Initialize(value, position, cellViewObject);
        
            _cells.Add(cell);
            _grid[position.x, position.y] = true;
        
            var cellView = cellViewObject.GetComponent<CellView>();
            if (cellView != null)
            {
                cellView.Init(cell);
            }
        
            var cellViews = GameObject.Find("CellViews");
            if (cellViews != null)
            {
                cellViewObject.transform.SetParent(cellViews.transform, false);
            }
            cellViewObject.name = "CellView";
            
            yield return StartCoroutine(CellView.SpawnCellAnimation(cell));
        }
    }
    
    public IEnumerator MoveCells(Vector2 direction)
    {
        var reverseOrder = (direction == Vector2.right || direction == Vector2.down);
    
        foreach (var cell in _cells)
        {
            cell.Merged = false;
        }
    
        var sortedCells = GetSortedCells(direction, reverseOrder);
    
        foreach (var cell in sortedCells)
        {
            yield return StartCoroutine(MoveCell(cell, direction));
        }
        
        StartCoroutine(CreateCell(0.2f));
    }
    
    private IEnumerator MoveCell(Cell cell, Vector2 direction)
    {
        var newPosition = cell.Position;
        Cell mergedCell = null;
    
        while (true)
        {
            var nextPosition = newPosition + new Vector2Int((int)direction.x, (int)direction.y);
        
            if ((nextPosition.x < 0 || nextPosition.x >= _fieldSize || nextPosition.y < 0 || nextPosition.y >= _fieldSize))
            {
                break;
            }
        
            var nextCell = _cells.Find(nextCell => nextCell.Position == nextPosition);
        
            if (nextCell == null)
            {
                newPosition = nextPosition;
            }
            else if (nextCell.Value == cell.Value && !nextCell.Merged)
            {
                mergedCell = nextCell;
                newPosition = nextPosition;
                break;
            }
            else
            {
                break;
            }
        }
    
        if (newPosition != cell.Position)
        {
            yield return StartCoroutine(CellView.MoveCellAnimation(cell, newPosition));
        
            if (mergedCell != null)
            {
                yield return StartCoroutine(CellView.MergeCellAnimation(cell, mergedCell));
            
                mergedCell.Value++;
                _cells.Remove(cell);
                _grid[cell.Position.x, cell.Position.y] = false;
                mergedCell.Merged = true;
            }
        
            _grid[cell.Position.x, cell.Position.y] = false;
            cell.Position = newPosition;
            _grid[newPosition.x, newPosition.y] = true;
        }
    }
    
    private List<Cell> GetSortedCells(Vector2 direction, bool reverseOrder)
    {
        var sortedCells = new List<Cell>(_cells);

        if (direction == Vector2.up || direction == Vector2.down)
        {
            sortedCells.Sort((a, b) => a.Position.y.CompareTo(b.Position.y));
        }
        else if (direction == Vector2.left || direction == Vector2.right)
        {
            sortedCells.Sort((a, b) => a.Position.x.CompareTo(b.Position.x));
        }

        if (reverseOrder)
        {
            sortedCells.Reverse();
        }

        return sortedCells;
    }
}