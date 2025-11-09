using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawRect : MonoBehaviour
{
    [SerializeField] private GameObject _rectanglePrefab;

    private List<UIRectObject> _rectObjects = new();
    private List<int> _openIndices = new();

    public void CreateRect(Rect rect, Color color, string text, string translatedText = null)
    {
        if (_openIndices.Count == 0)
        {
            var newRect = Instantiate(_rectanglePrefab, parent: transform).GetComponent<UIRectObject>();
            
            _rectObjects.Add(newRect);
            _openIndices.Add(_rectObjects.Count-1);
        }


        int index = _openIndices[0];
        _openIndices.RemoveAt(0);


        UIRectObject rectObject = _rectObjects[index];
        rectObject.SetRectTransform(rect);
        rectObject.SetColor(color);
        
        string displayText = text;
        if (!string.IsNullOrEmpty(translatedText))
        {
            displayText += $"\n{translatedText}";
        }
        
        rectObject.SetText(displayText);
        rectObject.gameObject.SetActive(true);
    }

    public void ClearRects()
    {
        for (int i = 0; i < _rectObjects.Count; i++)
        {
            _rectObjects[i].gameObject.SetActive(false);
            _openIndices.Add(i);
        }
    }
    
}