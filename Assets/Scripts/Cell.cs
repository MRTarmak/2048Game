using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public event Action<int> OnValueChanged;
    
    public event Action<Vector2Int> OnPositionChanged;
    
    private int _value;
    
    private Vector2Int _position;

    public int Value
    {
        get => _value;
        set
        {
            if (this._value != value)
            {
                this._value = value;
                OnValueChanged?.Invoke(this._value);
            }
        }
    }

    public Vector2Int Position
    {
        get => _position;
        set
        {
            if (this._position != value)
            {
                this._position = value;
                OnPositionChanged?.Invoke(this._position);
            }
        }
    }
    
    public void Initialize(int initialValue, Vector2Int position)
    {
        Value = initialValue;
        Position = position;
    }
}