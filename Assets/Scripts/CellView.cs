using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    private Cell _cell;
    
    private Image _cellImage;
    
    public TextMeshProUGUI valueText;

    void Awake()
    {
        _cellImage = GetComponent<Image>();
    }
    
    public void Init(Cell cell)
    {
        this._cell = cell;

        cell.OnValueChanged += UpdateValue;
        cell.OnPositionChanged += UpdatePosition;

        UpdateValue(_cell.Value);
        UpdatePosition(_cell.Position);
    }

    private void UpdateValue(int value)
    {
        valueText.text = ((int) Math.Pow(2, value)).ToString();
        UpdateColor();
        UpdateTextStyle();
    }

    private void UpdatePosition(Vector2Int position)
    {
        transform.position = new Vector3(position.x * 280 - 420, position.y * 280 - 620, 0);
    }
    
    private void OnDestroy()
    {
        if (_cell != null)
        {
            _cell.OnValueChanged -= UpdateValue;
            _cell.OnPositionChanged -= UpdatePosition;
        }
    }

    private void UpdateColor()
    {
        switch (_cell.Value)
        {
            case 1:
                _cellImage.color = new Color(238 / 255f, 228 / 255f, 218 / 255f);
                break;
            case 2:
                _cellImage.color = new Color(237 / 255f, 224 / 255f, 200 / 255f);
                break;
            case 3:
                _cellImage.color = new Color(242 / 255f, 177 / 255f, 121 / 255f);
                break;
            case 4:
                _cellImage.color = new Color(245 / 255f, 149 / 255f, 99 / 255f);
                break;
            case 5:
                _cellImage.color = new Color(246 / 255f, 124 / 255f, 95 / 255f);
                break;
            case 6:
                _cellImage.color = new Color(246 / 255f, 94 / 255f, 59 / 255f);
                break;
            case 7:
                _cellImage.color = new Color(237 / 255f, 207 / 255f, 114 / 255f);
                break;
            case 8:
                _cellImage.color = new Color(237 / 255f, 204 / 255f, 97 / 255f);
                break;
            case 9:
                _cellImage.color = new Color(237 / 255f, 200 / 255f, 80 / 255f);
                break;
            case 10:
                _cellImage.color = new Color(237 / 255f, 197 / 255f, 63 / 255f);
                break;
            case 11:
                _cellImage.color = new Color(237 / 255f, 194 / 255f, 46 / 255f);
                break;
            default:
                _cellImage.color = Color.clear;
                break;
        }
    }
    
    private void UpdateTextStyle()
    {
        switch (_cell.Value)
        {
            case 1:
            case 2:
                valueText.color = new Color(143 / 255f, 132 / 255f, 111 / 255f);
                valueText.fontSize = 85;
                break;
            case 3:
            case 4:
            case 5:
            case 6:
                valueText.color = new Color(253 / 255f, 231 / 255f, 210 / 255f);
                valueText.fontSize = 85;
                break;
            case 7:
            case 8:
            case 9:
                valueText.color = new Color(253 / 255f, 231 / 255f, 210 / 255f);
                valueText.fontSize = 70;
                break;
            case 10:
            case 11:
                valueText.color = new Color(253 / 255f, 231 / 255f, 210 / 255f);
                valueText.fontSize = 50;
                break;
            default:
                valueText.color = Color.clear;
                valueText.fontSize = 0;
                break;
        }
    }
}
