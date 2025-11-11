using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Action menu that appears when an object is focused.
/// Provides options: Hear Pronunciation, See Examples, Save Word (future).
/// </summary>
public class ActionMenuPanel : MonoBehaviour
{
    // ===== SERIALIZED FIELDS =====
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panelTransform;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _infoText;

    [Header("Buttons")]
    [SerializeField] private Button _pronunciationButton;
    [SerializeField] private Button _examplesButton;
    [SerializeField] private Button _closeButton;

    [Header("Positioning")]
    [SerializeField] private Vector2 _offsetFromObject = new Vector2(0, -120);
    [SerializeField] private float _fadeInDuration = 0.2f;

    [Header("References")]
    [SerializeField] private Canvas _canvas;

    // ===== PRIVATE FIELDS =====
    private DetectedObjectData _currentObject;
    private bool _isVisible = false;

    // ===== UNITY LIFECYCLE =====
    private void Awake()
    {
        Debug.Log("[ActionMenuPanel] Awake() called");

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvas == null)
            _canvas = GetComponentInParent<Canvas>();

        Debug.Log("[ActionMenuPanel] Awake() complete - initialized");
    }

    private void Start()
    {
        Debug.Log("[ActionMenuPanel] Start() - subscribing to events and hiding panel");

        // Subscribe to events
        ObjectSelectionManager.OnObjectFocused += ShowMenu;
        ObjectSelectionManager.OnObjectUnfocused += HideMenu;

        // Setup button listeners
        if (_pronunciationButton != null)
            _pronunciationButton.onClick.AddListener(OnPronunciationClicked);

        if (_examplesButton != null)
            _examplesButton.onClick.AddListener(OnExamplesClicked);

        if (_closeButton != null)
            _closeButton.onClick.AddListener(OnCloseClicked);

        // Initially hidden (do this AFTER subscribing to events)
        Hide(instant: true);

        Debug.Log("[ActionMenuPanel] Start() complete - ready to show menu on focus");
    }

    private void OnDestroy()
    {
        ObjectSelectionManager.OnObjectFocused -= ShowMenu;
        ObjectSelectionManager.OnObjectUnfocused -= HideMenu;

        if (_pronunciationButton != null)
            _pronunciationButton.onClick.RemoveListener(OnPronunciationClicked);

        if (_examplesButton != null)
            _examplesButton.onClick.RemoveListener(OnExamplesClicked);

        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(OnCloseClicked);
    }

    // ===== PUBLIC METHODS =====

    public void Show()
    {
        Debug.Log("[ActionMenuPanel] Show() called - making panel visible");
        _isVisible = true;
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public void Hide(bool instant = false)
    {
        Debug.Log("[ActionMenuPanel] Hide() called");
        _isVisible = false;

        if (instant)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        else
        {
            StartCoroutine(FadeOut());
        }
    }

    // ===== PRIVATE METHODS =====

    private void ShowMenu(DetectedObjectData data)
    {
        Debug.Log($"[ActionMenuPanel] ShowMenu() called for: {data.category} at position: {data.screenPosition}");

        _currentObject = data;

        // Update title and info
        if (_titleText != null)
        {
            string displayText = $"{data.category}";
            if (!string.IsNullOrEmpty(data.translation))
            {
                displayText += $" → {data.translation}";
            }
            _titleText.text = displayText;
        }

        if (_infoText != null)
        {
            _infoText.text = $"Confidence: {data.confidence:F2}";
        }

        // Position menu
        PositionMenu(data.screenPosition);

        // Show
        Show();
    }

    private void HideMenu()
    {
        Hide();
    }

    private void PositionMenu(Vector2 rectBottomCenter)
    {
        // rectBottomCenter is in the detection rectangle's coordinate space
        // We need to use this position directly since both UI elements share the same canvas parent

        // Calculate target position: place menu below the detection rectangle
        Vector2 targetPos = rectBottomCenter + _offsetFromObject;

        // Log anchor info for debugging
        Debug.Log($"[ActionMenuPanel] Panel anchors: min={_panelTransform.anchorMin}, max={_panelTransform.anchorMax}, pivot={_panelTransform.pivot}");
        Debug.Log($"[ActionMenuPanel] Target position before clamping: {targetPos}, offset: {_offsetFromObject}");

        // Clamp to canvas bounds to ensure menu stays on screen
        if (_canvas != null)
        {
            RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
            float halfWidth = _panelTransform.rect.width / 2f;
            float halfHeight = _panelTransform.rect.height / 2f;

            // Clamp position to keep menu fully visible
            targetPos.x = Mathf.Clamp(targetPos.x, halfWidth, canvasRect.rect.width - halfWidth);
            targetPos.y = Mathf.Clamp(targetPos.y, halfHeight, canvasRect.rect.height - halfHeight);

            Debug.Log($"[ActionMenuPanel] Final position: {targetPos} (rect at {rectBottomCenter}, canvas: {canvasRect.rect.size})");
        }

        _panelTransform.anchoredPosition = targetPos;
    }

    private System.Collections.IEnumerator FadeIn()
    {
        // Enable interaction immediately at start of fade-in
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        float elapsed = 0f;

        while (elapsed < _fadeInDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / _fadeInDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        // Disable interaction immediately at start of fade-out
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < _fadeInDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeInDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    // ===== BUTTON HANDLERS =====

    private void OnPronunciationClicked()
    {
        if (_currentObject == null || string.IsNullOrEmpty(_currentObject.translation))
        {
            Debug.LogWarning("[ActionMenuPanel] No translation available for pronunciation");
            return;
        }

        Debug.Log($"[ActionMenuPanel] Playing pronunciation: {_currentObject.translation}");

        // Play pronunciation via TTS
        if (TTSManager.Instance != null)
        {
            TTSManager.Instance.Speak(_currentObject.translation);
        }
        else
        {
            Debug.LogWarning("[ActionMenuPanel] TTSManager not found");
        }
    }

    private void OnExamplesClicked()
    {
        if (_currentObject == null)
            return;

        Debug.Log($"[ActionMenuPanel] Showing examples for: {_currentObject.category}");

        // Show example sentences panel
        if (ExampleSentencesPanel.Instance != null)
        {
            ExampleSentencesPanel.Instance.ShowExamples(_currentObject.category, _currentObject.translation);
        }
        else
        {
            Debug.LogWarning("[ActionMenuPanel] ExampleSentencesPanel not found");
        }
    }

    private void OnCloseClicked()
    {
        // Unfocus the object
        if (ObjectSelectionManager.Instance != null)
        {
            ObjectSelectionManager.Instance.UnfocusObject();
        }
    }
}
