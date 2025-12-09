using UnityEngine;
using TMPro;

/// <summary>
/// Updates the badge on the "My Vocabulary" button to show word count.
/// </summary>
public class VocabularyBadge : MonoBehaviour
{
    [SerializeField] private TMP_Text _badgeText;
    [SerializeField] private GameObject _badgeContainer;

    private void Start()
    {
        // Update immediately on start
        UpdateBadge();
    }

    private void OnEnable()
    {
        // Subscribe to vocabulary changes
        VocabularyManager.OnVocabularyUpdated += UpdateBadge;
    }

    private void OnDisable()
    {
        // Unsubscribe
        VocabularyManager.OnVocabularyUpdated -= UpdateBadge;
    }

    private void UpdateBadge()
    {
        if (VocabularyManager.Instance == null)
        {
            // Hide badge if no manager
            if (_badgeContainer != null)
                _badgeContainer.SetActive(false);
            return;
        }

        int wordCount = VocabularyManager.Instance.GetAllWords().Count;

        // Update badge text
        if (_badgeText != null)
        {
            _badgeText.text = wordCount.ToString();
        }

        // Show/hide badge based on count
        if (_badgeContainer != null)
        {
            _badgeContainer.SetActive(wordCount > 0);  // Hide badge when 0 words
        }

        Debug.Log($"[VocabularyBadge] Updated badge: {wordCount} words");
    }
}
