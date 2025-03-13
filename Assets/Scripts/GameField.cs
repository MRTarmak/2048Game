using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public GameObject cellPrefab;
    
    public GameObject emptyCellPrefab;
    
    private List<Cell> _cells = new List<Cell>();

    private int _fieldSize = 4;

    void Start()
    {
        // InitializeField();
        // CreateCell();
        // CreateCell();
        
        InitializeField();
        CreateCell(0);
        CreateCell(1);
        CreateCell(2);
        CreateCell(3);
        CreateCell(4);
        CreateCell(5);
        CreateCell(6);
        CreateCell(7);
        CreateCell(8);
        CreateCell(9);
        CreateCell(10);
        CreateCell(11);
        CreateCell(12);
    }
    
    private void InitializeField()
    {
        GameObject emptyCells = GameObject.Find("EmptyCells");
        
        for (int x = 0; x < _fieldSize; x++)
        {
            for (int y = 0; y < _fieldSize; y++)
            {
                Vector3 positionVector = new Vector3(x * 280 - 420, y * 280 - 620, 0);
                GameObject emptyCellObject = Instantiate(emptyCellPrefab, positionVector, Quaternion.identity);
                Cell cell = emptyCellObject.AddComponent<Cell>();
                cell.Initialize(0, new Vector2Int(x, y));
                
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

    public void CreateCell()
    {
        Vector2Int position = GetEmptyPosition();

        if (position.x != -1 && position.y != -1)
        {
            int value = Random.value <= 0.1f ? 2 : 1;
            
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
}
