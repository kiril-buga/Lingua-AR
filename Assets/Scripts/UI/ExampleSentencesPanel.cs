using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Panel that displays example sentences for a selected word.
/// </summary>
public class ExampleSentencesPanel : MonoBehaviour
{
    // ===== SINGLETON =====
    public static ExampleSentencesPanel Instance { get; private set; }

    // ===== SERIALIZED FIELDS =====
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Transform _sentencesContainer;
    [SerializeField] private GameObject _sentenceItemPrefab;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TMP_Text _noExamplesText;

    [Header("References")]
    [SerializeField] private ExampleSentencesDatabaseSO _sentencesDatabase;

    // ===== PRIVATE FIELDS =====
    private List<GameObject> _sentenceItems = new List<GameObject>();
    private bool _isVisible = false;

    // ===== UNITY LIFECYCLE =====
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        // Initially hidden
        Hide(instant: true);
    }

    private void OnEnable()
    {
        if (_closeButton != null)
            _closeButton.onClick.AddListener(() => Hide());
    }




    private void OnDisable()
    {
        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(() => Hide());
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // ===== PUBLIC METHODS =====

    /// <summary>
    /// Shows example sentences for the given word.
    /// </summary>
    public void ShowExamples(string objectClass, string translation = null)
    {
        Debug.Log($"[ExampleSentencesPanel] ShowExamples called for: '{objectClass}'");

        if (_sentencesDatabase == null)
        {
            Debug.LogWarning("[ExampleSentencesPanel] Sentences database not assigned");
            return;
        }

        // Clear previous sentences
        ClearSentences();

        // Update title
        if (_titleText != null)
        {
            string title = $"Examples: {objectClass}";
            if (!string.IsNullOrEmpty(translation))
            {
                title += $" ({translation})";
            }
            _titleText.text = title;
        }

        // Get examples
        var examples = _sentencesDatabase.GetExamples(objectClass);
        Debug.Log($"[ExampleSentencesPanel] Found {examples.Count} examples for '{objectClass}'");

        if (examples.Count == 0)
        {
            Debug.LogWarning($"[ExampleSentencesPanel] No examples found for '{objectClass}' - check database!");
            // Show "no examples" message
            if (_noExamplesText != null)
            {
                _noExamplesText.gameObject.SetActive(true);
                _noExamplesText.text = $"No example sentences available for '{objectClass}'.";
            }
        }
        else
        {
            // Hide "no examples" message
            if (_noExamplesText != null)
                _noExamplesText.gameObject.SetActive(false);

            // Create sentence items
            foreach (var example in examples)
            {
                CreateSentenceItem(example);
            }

            // Force layout rebuild to ensure proper positioning
            Canvas.ForceUpdateCanvases();
            if (_sentencesContainer != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_sentencesContainer as RectTransform);
            }
        }

        // Show panel
        Show();
    }

    /// <summary>
    /// Hides the panel.
    /// </summary>
    public void Hide(bool instant = false)
    {
        _isVisible = false;

        if (instant)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(FadeOut());
        }
    }

    // ===== PRIVATE METHODS =====

    private void Show()
    {
        Debug.Log("[ExampleSentencesPanel] Show() called - making panel visible");
        _isVisible = true;
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    private void CreateSentenceItem(ExampleSentencesDatabaseSO.SentenceExample example)
    {
        if (_sentenceItemPrefab == null || _sentencesContainer == null)
            return;

        GameObject item = Instantiate(_sentenceItemPrefab, _sentencesContainer);
        _sentenceItems.Add(item);

        // Reset RectTransform to ensure proper layout
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
        }

        // Find text components (assumes prefab has two TMP_Text children)
        TMP_Text[] texts = item.GetComponentsInChildren<TMP_Text>();
        if (texts.Length >= 2)
        {
            texts[0].text = example.englishSentence;
            texts[1].text = example.translatedSentence;
        }
        else if (texts.Length == 1)
        {
            texts[0].text = $"{example.englishSentence}\n\n{example.translatedSentence}";
        }

        item.SetActive(true);

        Debug.Log($"[ExampleSentencesPanel] Created sentence item: '{example.englishSentence}'");
    }

    private void ClearSentences()
    {
        foreach (var item in _sentenceItems)
        {
            if (item != null)
                Destroy(item);
        }

        _sentenceItems.Clear();
    }

    private System.Collections.IEnumerator FadeIn()
    {
        // Enable interaction immediately at start of fade-in
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        // Disable interaction immediately at start of fade-out
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        float duration = 0.2f;
        float elapsed = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
