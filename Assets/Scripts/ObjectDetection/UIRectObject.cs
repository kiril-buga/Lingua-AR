using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class UIRectObject : MonoBehaviour, IPointerClickHandler
{
    // ===== EVENTS =====
    public static event Action<UIRectObject> OnRectClicked;

    // ===== ENUMS =====
    public enum SelectionState
    {
        Normal,      // Default unselected state
        Focused,     // User has selected this object
        Saved        // Word is in saved vocabulary (future)
    }

    // ===== COMPONENTS =====
    private RectTransform _rectangleRectTransform;
    private Image _rectangleImage;
    private TMP_Text _text;

    // ===== PUBLIC PROPERTIES =====
    public SelectionState CurrentState { get; private set; } = SelectionState.Normal;
    public string Category { get; private set; }
    public string Translation { get; private set; }
    public float Confidence { get; private set; }
    public Vector2 ScreenPosition { get; private set; }

    // ===== PRIVATE FIELDS =====
    private Color _originalColor;
    private Color _normalColor = new Color(1f, 1f, 1f, 0.5f);
    private Color _focusedColor = new Color(0f, 0.8f, 1f, 0.9f); // Cyan highlight
    private Vector3 _normalScale = Vector3.one;
    private Vector3 _focusedScale = Vector3.one * 1.15f;

    public void Awake()
    {
        _rectangleRectTransform = GetComponent<RectTransform>();
        _rectangleImage = GetComponent<Image>();
        _text = GetComponentInChildren<TMP_Text>();

        // Ensure image is raycast target for click detection
        _rectangleImage.raycastTarget = true;
    }

    /// <summary>
    /// Sets all detection metadata for this rectangle.
    /// </summary>
    public void SetDetectionData(string category, string translation, float confidence, Vector2 screenPos)
    {
        Category = category;
        Translation = translation;
        Confidence = confidence;
        ScreenPosition = screenPos;
    }

    public void SetRectTransform(Rect rect)
    {
        _rectangleRectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
        _rectangleRectTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }

    public void SetColor(Color color)
    {
        _originalColor = color;
        _normalColor = color;

        // Apply color based on current state
        if (CurrentState == SelectionState.Normal)
        {
            _rectangleImage.color = color;
        }
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public RectTransform getRectTransform()
    {
        return _rectangleRectTransform;
    }

    /// <summary>
    /// Changes the visual state of the rectangle.
    /// </summary>
    public void SetState(SelectionState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        switch (newState)
        {
            case SelectionState.Normal:
                _rectangleImage.color = _normalColor;
                transform.localScale = _normalScale;
                break;

            case SelectionState.Focused:
                _rectangleImage.color = _focusedColor;
                transform.localScale = _focusedScale;
                transform.SetAsLastSibling(); // Draw on top
                break;

            case SelectionState.Saved:
                _rectangleImage.color = new Color(0.2f, 1f, 0.2f, 0.7f); // Green
                break;
        }
    }

    /// <summary>
    /// Called by Unity's EventSystem when user taps/clicks this rectangle.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[UIRectObject] Clicked: {Category} ({Translation})");
        OnRectClicked?.Invoke(this);
    }
}