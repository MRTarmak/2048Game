using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    private Cell _cell;
    
    private Image _cellImage;
    
    [SerializeField] private TextMeshProUGUI valueText;

    private void Awake()
    {
        _cellImage = GetComponent<Image>();
    }
    
    public void Init(Cell cell)
    {
        _cell = cell;

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
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Cell.GetWorldPosition(position);
    }
    
    private void OnDestroy()
    {
        if (_cell != null)
        {
            _cell.OnValueChanged -= UpdateValue;
            _cell.OnPositionChanged -= UpdatePosition;
        }
    }

    private static Color ColorLerp(Color colorA, Color colorB, float t)
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
    
    public static IEnumerator MoveCellAnimation(Cell cell, Vector2Int newPosition)
    {
        Vector3 startPos = Cell.GetWorldPosition(cell.Position);
        Vector3 endPos = Cell.GetWorldPosition(newPosition);
        var duration = 0.1f;
        var elapsed = 0f;
        var rectTransform = cell.CellView.GetComponent<RectTransform>();
        
        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        rectTransform.anchoredPosition = endPos;
    }
    
    public static IEnumerator MergeCellAnimation(Cell disappearingCell, Cell targetCell)
    {
        var duration = 0.1f;
        var elapsed = 0f;
        
        var rectTransform = disappearingCell.CellView.GetComponent<RectTransform>();
        var startScale = rectTransform.localScale;
    
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(disappearingCell.gameObject);
        
        rectTransform = targetCell.CellView.GetComponent<RectTransform>();
        var targetStartScale = rectTransform.localScale;
        elapsed = 0f;
    
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(targetStartScale, targetStartScale * 1.2f, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    
        elapsed = 0f;
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(targetStartScale * 1.2f, targetStartScale, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    public static IEnumerator SpawnCellAnimation(Cell cell)
    {
        var rectTransform = cell.CellView.GetComponent<RectTransform>();
    
        var duration = 0.15f;
        var elapsed = 0f;
        var startScale = Vector3.zero;
        var endScale = new Vector3(2, 2, 2);
    
        rectTransform.localScale = startScale;
    
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    
        rectTransform.localScale = endScale;
    
        elapsed = 0f;
        var bounceDuration = 0.1f;
        var bounceScale = endScale * 1.1f;
    
        while (elapsed < bounceDuration)
        {
            rectTransform.localScale = Vector3.Lerp(endScale, bounceScale, elapsed/bounceDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    
        elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            rectTransform.localScale = Vector3.Lerp(bounceScale, endScale, elapsed/bounceDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    
        rectTransform.localScale = endScale;
    }
}
