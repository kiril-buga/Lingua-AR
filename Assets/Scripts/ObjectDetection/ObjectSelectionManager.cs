using System;
using UnityEngine;

/// <summary>
/// Manages object selection and focus state.
/// Singleton pattern for global access.
/// </summary>
public class ObjectSelectionManager : MonoBehaviour
{
    // ===== SINGLETON =====
    public static ObjectSelectionManager Instance { get; private set; }

    // ===== EVENTS =====
    public static event Action<DetectedObjectData> OnObjectFocused;
    public static event Action OnObjectUnfocused;

    // ===== SERIALIZED FIELDS =====
    [Header("Configuration")]
    [SerializeField] private bool _pauseDetectionWhenFocused = true;
    [SerializeField] private ObjectDetectionSample _detectionController;
    [SerializeField] private DrawRect _drawRect;

    // ===== PUBLIC PROPERTIES =====
    public DetectedObjectData FocusedObject { get; private set; }
    public bool HasFocusedObject => FocusedObject != null;

    // ===== PRIVATE FIELDS =====
    private UIRectObject _focusedRectObject;

    // ===== UNITY LIFECYCLE =====
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        // Subscribe to rectangle click events
        UIRectObject.OnRectClicked += OnRectClicked;
    }

    private void OnDestroy()
    {
        UIRectObject.OnRectClicked -= OnRectClicked;

        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        // Press Escape or right-click to unfocus
        if (HasFocusedObject && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            UnfocusObject();
        }
    }

    // ===== PUBLIC METHODS =====

    /// <summary>
    /// Focuses on a detected object, showing its translation and pausing detection.
    /// </summary>
    public void FocusObject(UIRectObject rectObject)
    {
        if (rectObject == null)
        {
            Debug.LogWarning("[ObjectSelectionManager] Attempted to focus null object");
            return;
        }

        // Unfocus previous object first
        if (_focusedRectObject != null && _focusedRectObject != rectObject)
        {
            _focusedRectObject.SetState(UIRectObject.SelectionState.Normal);
        }

        // Set new focused object
        _focusedRectObject = rectObject;
        _focusedRectObject.SetState(UIRectObject.SelectionState.Focused);

        // Create detection data
        FocusedObject = new DetectedObjectData
        {
            category = rectObject.Category,
            translation = rectObject.Translation,
            confidence = rectObject.Confidence,
            screenPosition = rectObject.ScreenPosition,
            detectionTime = DateTime.Now
        };

        // Update text to show translation
        string displayText = $"{rectObject.Category}: {rectObject.Confidence:F2}";
        if (!string.IsNullOrEmpty(rectObject.Translation))
        {
            displayText += $"\n{rectObject.Translation}";
        }
        rectObject.SetText(displayText);

        // Hide all other rectangles except the focused one
        if (_drawRect != null)
        {
            _drawRect.HideAllExcept(_focusedRectObject);
        }

        // Pause detection to freeze UI on this object (without clearing rectangles)
        if (_pauseDetectionWhenFocused && _detectionController != null)
        {
            _detectionController.PauseDetectionOnly();
        }

        Debug.Log($"[ObjectSelectionManager] Focused on: {FocusedObject.category} ({FocusedObject.translation})");
        OnObjectFocused?.Invoke(FocusedObject);
    }

    /// <summary>
    /// Clears the focused object and resumes detection.
    /// </summary>
    public void UnfocusObject()
    {
        if (!HasFocusedObject)
            return;

        // Reset visual state
        if (_focusedRectObject != null)
        {
            _focusedRectObject.SetState(UIRectObject.SelectionState.Normal);
        }

        FocusedObject = null;
        _focusedRectObject = null;

        // Resume detection
        if (_pauseDetectionWhenFocused && _detectionController != null)
        {
            _detectionController.EnableObjectDetection();
        }

        Debug.Log("[ObjectSelectionManager] Unfocused object");
        OnObjectUnfocused?.Invoke();
    }

    // ===== PRIVATE METHODS =====

    private void OnRectClicked(UIRectObject rectObject)
    {
        // If clicking the same object, unfocus it
        if (_focusedRectObject == rectObject)
        {
            UnfocusObject();
        }
        else
        {
            FocusObject(rectObject);
        }
    }
}

// ===== DATA CLASSES =====

/// <summary>
/// Represents a detected object with all relevant metadata.
/// </summary>
[System.Serializable]
public class DetectedObjectData
{
    public string category;
    public string translation;
    public float confidence;
    public Vector2 screenPosition;
    public DateTime detectionTime;
}
