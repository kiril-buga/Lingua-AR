using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawRect : MonoBehaviour
{
    [SerializeField] private GameObject _rectanglePrefab;

    private List<UIRectObject> _rectObjects = new();
    private List<int> _openIndices = new();

    public void CreateRect(Rect rect, Color color, string text, string translatedText = null, string category = null, float confidence = 0f)
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

        // Set detection metadata (for click handling)
        rectObject.SetDetectionData(category ?? text, translatedText, confidence, rect.position);

        // Initially show only category name (not translation)
        // Translation will appear when user clicks the rectangle
        rectObject.SetText(text);
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

    /// <summary>
    /// Hides all rectangles except the specified one (used for focus mode)
    /// </summary>
    public void HideAllExcept(UIRectObject exception)
    {
        for (int i = 0; i < _rectObjects.Count; i++)
        {
            if (_rectObjects[i] != exception && _rectObjects[i].gameObject.activeSelf)
            {
                _rectObjects[i].gameObject.SetActive(false);
                if (!_openIndices.Contains(i))
                {
                    _openIndices.Add(i);
                }
            }
        }
    }

    /// <summary>
    /// Shows all rectangles that were previously hidden
    /// </summary>
    public void ShowAll()
    {
        // This will naturally happen when detection resumes and creates new rectangles
    }
}