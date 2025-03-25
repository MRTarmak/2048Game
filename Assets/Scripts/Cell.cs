using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public event Action<int> OnValueChanged;
    
    public event Action<Vector2Int> OnPositionChanged;
    
    public GameObject cellView;
    
    private int _value;
    
    private Vector2Int _position;
    
    private bool _merged = false;

    public int Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    public Vector2Int Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPositionChanged?.Invoke(_position);
            }
        }
    }

    public GameObject CellView { get; set; }

    public bool Merged { get; set; }
    
    public void Initialize(int initialValue, Vector2Int position, GameObject cellView)
    {
        Value = initialValue;
        Position = position;
        CellView = cellView;
    }

    public static Vector2 GetWorldPosition(Vector2Int position)
    {
        return new Vector2(position.x * 280 - 420, position.y * 280 - 620);
    }
}