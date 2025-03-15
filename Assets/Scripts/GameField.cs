using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public GameObject cellPrefab;
    
    public GameObject emptyCellPrefab;
    
    private List<Cell> _cells = new List<Cell>();

    private int _fieldSize = 4;

    private void Start()
    {
        InitializeField();
        CreateCell(0.1f);
        CreateCell(0.1f);
        
        // Debug
        // InitializeField();
        // CreateCell(0);
        // CreateCell(1);
        // CreateCell(2);
        // CreateCell(3);
        // CreateCell(4);
        // CreateCell(5);
        // CreateCell(6);
        // CreateCell(7);
        // CreateCell(8);
        // CreateCell(9);
        // CreateCell(10);
        // CreateCell(11);
        // CreateCell(12);
    }
    
    private void InitializeField()
    {
        GameObject emptyCells = GameObject.Find("EmptyCells");
        
        for (var x = 0; x < _fieldSize; x++)
        {
            for (var y = 0; y < _fieldSize; y++)
            {
                Vector3 positionVector = new Vector3(x * 280 - 420, y * 280 - 620, 0);
                GameObject emptyCellObject = Instantiate(emptyCellPrefab, positionVector, Quaternion.identity);
                Cell cell = emptyCellObject.AddComponent<Cell>();
                cell.Initialize(0, new Vector2Int(x, y), x * 4 + y);
                
                _cells.Add(cell);
            
                emptyCellObject.transform.SetParent(emptyCells.transform, false);
                emptyCellObject.transform.name = "EmptyCell";
            }
        }
    }
    
    public Vector2Int GetEmptyPosition()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        foreach (Cell cell in _cells)
        {
            if (cell.Value == 0)
            {
                emptyPositions.Add(cell.Position);
            }
        }

        if (emptyPositions.Count > 0)
        {
            return emptyPositions[Random.Range(0, emptyPositions.Count)];
        }
        
        return new Vector2Int(-1, -1);
    }

    public void CreateCell(float probability)
    {
        Vector2Int position = GetEmptyPosition();

        if (position.x != -1 && position.y != -1)
        {
            int value = Random.value <= probability ? 2 : 1;
            
            Cell cell = _cells.Find(cell => cell.Position == position);
            
            cell.Value = value;

            Vector3 positionVector = new Vector3(position.x * 280 - 420, position.y * 280 - 620, 0);
            GameObject cellViewObject = Instantiate(cellPrefab, positionVector, Quaternion.identity);
            CellView cellView = cellViewObject.GetComponent<CellView>();
            cellView.Init(cell);
            
            GameObject cellViews = GameObject.Find("CellViews");
            cellViewObject.transform.SetParent(cellViews.transform, false);
            cellViewObject.transform.name = "CellView" + cell.CellIndex.ToString();
        }
    }
    
    private void CreateCell(int value) // For debug
    {
        Vector2Int position = GetEmptyPosition();

        if (position.x != -1 && position.y != -1)
        {
            Cell cell = _cells.Find(cell => cell.Position == position);
            
            cell.Value = value;

            Vector3 positionVector = new Vector3(position.x * 280 - 420, position.y * 280 - 620, 0);
            GameObject cellViewObject = Instantiate(cellPrefab, positionVector, Quaternion.identity);
            CellView cellView = cellViewObject.GetComponent<CellView>();
            cellView.Init(cell);
            
            GameObject cellViews = GameObject.Find("CellViews");
            cellViewObject.transform.SetParent(cellViews.transform, false);
            cellViewObject.transform.name = "CellView";
        }
    }
    
    public void MoveCells(Vector2 direction)
    {
        bool reverseOrder = (direction == Vector2.right || direction == Vector2.down);
        
        List<Cell> sortedCells = GetSortedCells(direction, reverseOrder);
        
        foreach (var cell in sortedCells)
        {
            MoveCell(cell, direction);
        }

        foreach (var cell in sortedCells)
        {
            cell.Merged = false;
        }
        
        CreateCell(0.2f);
    }
    
    private void MoveCell(Cell cell, Vector2 direction)
    {
        if (cell.Value == 0)
        {
            return;
        }
        
        Vector2Int newPosition = cell.Position;
        
        while (true)
        {
            Vector2Int nextPosition = newPosition + new Vector2Int((int)direction.x, (int)direction.y);
            
            if (nextPosition.x < 0 || nextPosition.x >= _fieldSize || nextPosition.y < 0 || nextPosition.y >= _fieldSize)
            {
                break;
            }
            
            Cell nextCell = _cells.Find(nextCell => nextCell.Position == nextPosition);

            if (nextCell.Value == 0)
            {
                newPosition = nextPosition;
            }
            else if (nextCell.Value == cell.Value && !nextCell.Merged)
            {
                newPosition = nextPosition;
                cell.Merged = true;
                nextCell.Value = 0;
                break;
            }
            else
            {
                break;
            }
        }
        
        if (newPosition != cell.Position)
        {
            if (cell.Merged)
            {
                GameObject cellView = GameObject.Find("CellView" + cell.CellIndex.ToString());
                if (cellView != null)
                {
                    Destroy(cellView);
                }
                
                cell.Value++;
            }
            cell.Position = newPosition;
            Debug.Log($"Cell position {cell.Position}, {new Vector3(cell.Position.x * 280 - 420, cell.Position.y * 280 - 620, 0)}");
        }
    }
    
    private List<Cell> GetSortedCells(Vector2 direction, bool reverseOrder)
    {
        List<Cell> sortedCells = new List<Cell>(_cells);

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