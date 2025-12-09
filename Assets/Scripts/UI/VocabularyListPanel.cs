using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Panel that displays the user's saved vocabulary list.
/// Allows viewing and deleting saved words.
/// </summary>
public class VocabularyListPanel : MonoBehaviour
{
    // ===== SINGLETON =====
    public static VocabularyListPanel Instance { get; private set; }

    // ===== SERIALIZED FIELDS =====
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _wordCountText;
    [SerializeField] private Transform _contentContainer;
    [SerializeField] private GameObject _wordItemPrefab;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TMP_Text _emptyStateText;

    [Header("Animation")]
    [SerializeField] private float _fadeDuration = 0.3f;

    // ===== PRIVATE FIELDS =====
    private List<VocabularyWordItem> _wordItems = new List<VocabularyWordItem>();
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

        Debug.Log("[VocabularyListPanel] Initialized");
    }

    private void OnEnable()
    {
        if (_closeButton != null)
            _closeButton.onClick.AddListener(() => Hide());

        // Subscribe to vocabulary updates
        VocabularyManager.OnVocabularyUpdated += OnVocabularyChanged;
    }

    private void OnDisable()
    {
        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(() => Hide());

        // Unsubscribe
        VocabularyManager.OnVocabularyUpdated -= OnVocabularyChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // ===== PUBLIC METHODS =====

    public void Show()
    {
        Debug.Log("[VocabularyListPanel] Showing panel");
        RefreshList();
        gameObject.SetActive(true);
        _isVisible = true;
        StartCoroutine(FadeIn());
    }

    public void Hide(bool instant = false)
    {
        Debug.Log("[VocabularyListPanel] Hiding panel");
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

    public void ToggleVisibility()
    {
        if (_isVisible)
            Hide();
        else
            Show();
    }

    // ===== PRIVATE METHODS =====

    private void OnVocabularyChanged()
    {
        Debug.Log("[VocabularyListPanel] Vocabulary updated - refreshing list");
        if (_isVisible)
        {
            RefreshList();
        }
    }

    private void RefreshList()
    {
        if (VocabularyManager.Instance == null)
        {
            Debug.LogWarning("[VocabularyListPanel] VocabularyManager not found");
            return;
        }

        // Get all saved words
        var words = VocabularyManager.Instance.GetAllWords();

        // Update word count
        if (_wordCountText != null)
        {
            _wordCountText.text = $"{words.Count} word{(words.Count != 1 ? "s" : "")} saved";
        }

        // Show/hide empty state
        if (_emptyStateText != null)
        {
            _emptyStateText.gameObject.SetActive(words.Count == 0);
        }

        // Deactivate all existing items
        foreach (var item in _wordItems)
        {
            if (item != null)
                item.gameObject.SetActive(false);
        }

        if (words.Count == 0)
        {
            Debug.Log("[VocabularyListPanel] No words to display");
            return;
        }

        // Sort by most recent first
        words.Sort((a, b) => b.firstDetectedTime.CompareTo(a.firstDetectedTime));

        // Populate list with object pooling
        for (int i = 0; i < words.Count; i++)
        {
            VocabularyWordItem item = GetOrCreateWordItem(i);
            item.Setup(words[i], OnDeleteClicked);
            item.gameObject.SetActive(true);
        }

        Debug.Log($"[VocabularyListPanel] Displayed {words.Count} words");
    }

    private VocabularyWordItem GetOrCreateWordItem(int index)
    {
        // Reuse existing item if available
        if (index < _wordItems.Count && _wordItems[index] != null)
        {
            return _wordItems[index];
        }

        // Create new item
        GameObject go = Instantiate(_wordItemPrefab, _contentContainer);
        VocabularyWordItem item = go.GetComponent<VocabularyWordItem>();

        if (item == null)
        {
            Debug.LogError("[VocabularyListPanel] Word item prefab missing VocabularyWordItem component!");
            item = go.AddComponent<VocabularyWordItem>();
        }

        _wordItems.Add(item);
        return item;
    }

    private void OnDeleteClicked(SavedWord word)
    {
        Debug.Log($"[VocabularyListPanel] Delete clicked for: {word.category}");

        if (VocabularyManager.Instance != null)
        {
            VocabularyManager.Instance.DeleteWord(word.category);
            // List will refresh automatically via OnVocabularyUpdated event
        }
    }

    // ===== ANIMATION COROUTINES =====

    private System.Collections.IEnumerator FadeIn()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
