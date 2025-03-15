using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    private Cell _cell;
    
    private Image _cellImage;
    
    public TextMeshProUGUI valueText;

    private void Awake()
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
        valueText.text = ((int)Mathf.Pow(2, value)).ToString();
        UpdateColor();
        UpdateTextStyle();
    }

    private void UpdatePosition(Vector2Int position)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(position.x * 280 - 420, position.y * 280 - 620);
    }
    
    private void OnDestroy()
    {
        if (_cell != null)
        {
            _cell.OnValueChanged -= UpdateValue;
            _cell.OnPositionChanged -= UpdatePosition;
        }
    }

    private Color ColorLerp(Color colorA, Color colorB, float t)
    {
        var r = Mathf.Lerp(colorA.r, colorB.r, t);
        var g = Mathf.Lerp(colorA.g, colorB.g, t);
        var b = Mathf.Lerp(colorA.b, colorB.b, t);
        
        return new Color(r, g, b);
    }

    private void UpdateColor()
    {
        if (_cell.Value > 0 && _cell.Value <= 11)
        {
            Color colorA = new Color(240 / 255f, 230 / 255f, 218 / 255f);
            Color colorB = new Color(255 / 255f, 65 / 255f, 29 / 255f);
            _cellImage.color = ColorLerp(colorA, colorB, (float)(_cell.Value - 1) / 10);
        }
        else
        {
            _cellImage.color = Color.clear;
        }
    }
    
    private void UpdateTextStyle()
    {
        switch (_cell.Value)
        {
            case 1:
            case 2:
            case 3:
                valueText.color = new Color(143 / 255f, 132 / 255f, 111 / 255f);
                valueText.fontSize = 85;
                break;
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
                valueText.fontSize = 65;
                break;
            case 10:
            case 11:
                valueText.color = new Color(253 / 255f, 231 / 255f, 210 / 255f);
                valueText.fontSize = 45;
                break;
            default:
                valueText.color = Color.clear;
                valueText.fontSize = 0;
                break;
        }
    }
}
