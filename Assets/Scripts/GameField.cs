using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GameField : MonoBehaviour
{
    public GameObject cellPrefab;
    
    private List<Cell> _cells = new List<Cell>();

    private int _fieldSize = 4;
    
    private bool[,] _grid;
    
    [SerializeField] private TextMeshProUGUI currentScoreText;
    
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int _currentScore = 0;
    
    private string _savePath;
    
    private GameSaveData _saveData;
    
    private bool _isAnimating = false;
    
    public bool IsAnimating => _isAnimating;
    
    public bool gameOver = false;

    private void Start()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "game_save.dat");
        LoadGame();
    }
    
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    
    private void InitializeField()
    {
        _grid = new bool[_fieldSize, _fieldSize];
    }
    
    private void UpdateScoreText()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = _currentScore.ToString();
        }
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
            
            _currentScore += (int)Mathf.Pow(2, value);
            UpdateScoreText();
        }
    }
    
    public IEnumerator MoveCells(Vector2 direction)
    {
        if (_isAnimating) yield break;
        _isAnimating = true;
        
        var reverseOrder = (direction == Vector2.right || direction == Vector2.up);
    
        foreach (var cell in _cells)
        {
            cell.Merged = false;
        }
    
        var sortedCells = GetSortedCells(direction, reverseOrder);
    
        foreach (var cell in sortedCells)
        {
            yield return StartCoroutine(MoveCell(cell, direction));
        }
        
        yield return StartCoroutine(CreateCell(0.2f));
        
        CheckGameOver();
        
        _isAnimating = false;
    }
    
    private IEnumerator MoveCell(Cell cell, Vector2 direction)
    {
        var newPosition = cell.Position;
        Cell mergedCell = null;
    
        while (true)
        {
            var nextPosition = newPosition + new Vector2Int((int)direction.x, (int)direction.y);
        
            if (nextPosition.x < 0 || nextPosition.x >= _fieldSize || nextPosition.y < 0 || nextPosition.y >= _fieldSize)
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
                
                UpdateScoreText();
            
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
    
    private IEnumerator CreateCellAtPosition(Vector2Int position, int value)
    {
        if (position.x != -1 && position.y != -1)
        {
            Vector3 positionVector = Cell.GetWorldPosition(position);
            var cellViewObject = Instantiate(cellPrefab, positionVector, Quaternion.identity);
        
            var cell = cellViewObject.GetComponent<Cell>();
            if (cell == null)
            {
                cell = cellViewObject.AddComponent<Cell>();
            }
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
            
            _currentScore += (int)Mathf.Pow(2, value);
            UpdateScoreText();
        }
    }
    
    private void LoadGame()
    {
        if (File.Exists(_savePath))
        {
            try 
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using FileStream stream = new FileStream(_savePath, FileMode.Open);
                _saveData = (GameSaveData)formatter.Deserialize(stream);
                InitializeField();
                
                for (int x = 0; x < _fieldSize; x++)
                {
                    for (int y = 0; y < _fieldSize; y++)
                    {
                        if (_saveData.CellValues[x,y] > 0)
                        {
                            StartCoroutine(CreateCellAtPosition(new Vector2Int(x, y), _saveData.CellValues[x, y]));
                        }
                    }
                }
                
                UpdateScoreText();
                
                if (highScoreText != null)
                {
                    highScoreText.text = _saveData.highScore.ToString();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading game: " + e.Message);
                StartNewGame();
            }
        }
        else
        {
            StartNewGame();
        }
    }

    private void SaveGame()
    {
        _saveData.currentScore = _currentScore;
        
        _saveData.CellValues = new int[_fieldSize, _fieldSize];
        foreach (Cell cell in _cells)
        {
            _saveData.CellValues[cell.Position.x, cell.Position.y] = cell.Value;
        }
        
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream stream = new FileStream(_savePath, FileMode.Create);
            formatter.Serialize(stream, _saveData);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving game: " + e.Message);
        }
    }
    
    private void CheckGameOver()
    {
        StartCoroutine(DelayedGameOverCheck());
    }
    
    private IEnumerator DelayedGameOverCheck()
    {
        yield return new WaitForEndOfFrame();
        
        bool gameOverCondition = true;

        if (GetEmptyPosition().x != -1)
        {
            gameOverCondition = false;
        }
        else
        {
            foreach (Cell cell in _cells)
            {
                Vector2Int[] directions =
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right
                };

                foreach (var dir in directions)
                {
                    Vector2Int neighborPos = cell.Position + dir;
                    if (!(neighborPos.x < 0 || neighborPos.x >= _fieldSize || neighborPos.y < 0 ||
                          neighborPos.y >= _fieldSize))
                    {
                        Cell neighbor = _cells.Find(nextCell => nextCell.Position == neighborPos);
                        if (neighbor != null && neighbor.Value == cell.Value)
                        {
                            gameOverCondition = false;
                        }
                    }
                }
            }
        }
        
        if (gameOverCondition)
        {
            gameOver = true;
            Debug.Log("Game Over");
            
            if (_currentScore > _saveData.highScore)
            {
                _saveData.highScore = _currentScore;
            
                if (highScoreText != null)
                {
                    highScoreText.text = _saveData.highScore.ToString();
                }
            
                SaveGame();
            }
        }
    }
    
    private void StartNewGame()
    {
        gameOver = false;

        _currentScore = 0;
        
        foreach (Cell cell in _cells)
        {
            Destroy(cell.gameObject);
        }
        _cells.Clear();
        
        _saveData = new GameSaveData 
        {
            CellValues = new int[_fieldSize, _fieldSize],
            currentScore = 0,
            highScore = _saveData?.highScore ?? 0
        };
        
        InitializeField();
        StartCoroutine(CreateCell(0.1f));
        StartCoroutine(CreateCell(0.1f));
        UpdateScoreText();
    }
    
    public void ResetGame()
    {
        if (_currentScore > _saveData.highScore)
        {
            _saveData.highScore = _currentScore;
            
            if (highScoreText != null)
            {
                highScoreText.text = _saveData.highScore.ToString();
            }
        }
        
        Debug.Log("Game reset");
        StartNewGame();
    }
}