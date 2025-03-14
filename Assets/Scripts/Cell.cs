using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public event Action<int> OnValueChanged;
    
    public event Action<Vector2Int> OnPositionChanged;
    
    private int _value;
    
    private Vector2Int _position;
    
    private bool _merged;

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

    public bool Merged
    {
        get => _merged;
        set => _merged = value;
    }
    
    public void Initialize(int initialValue, Vector2Int position)
    {
        Value = initialValue;
        Position = position;
    }
}