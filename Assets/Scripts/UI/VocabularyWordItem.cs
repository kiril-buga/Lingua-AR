using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for a single word in the vocabulary list.
/// Displays category, translation, date, and has a delete button.
/// </summary>
public class VocabularyWordItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _categoryText;
    [SerializeField] private TMP_Text _translationText;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private Button _deleteButton;

    private SavedWord _word;
    private Action<SavedWord> _onDeleteCallback;

    public void Setup(SavedWord word, Action<SavedWord> onDeleteCallback)
    {
        _word = word;
        _onDeleteCallback = onDeleteCallback;

        // Set texts - display source → target language format
        _categoryText.text = word.category;
        
        // Build translation text with both source and target languages
        string translationDisplay = word.savedTranslation;
        if (!string.IsNullOrEmpty(word.sourceTranslation))
        {
            translationDisplay = $"{word.sourceTranslation} → {word.savedTranslation}";
        }
        _translationText.text = $"{translationDisplay} ({GetLanguageName(word.savedLanguage)})";
        
        _dateText.text = $"Saved {GetTimeAgo(word.FirstDetectedDateTime)}";

        // Setup delete button
        _deleteButton.onClick.RemoveAllListeners();
        _deleteButton.onClick.AddListener(() => _onDeleteCallback?.Invoke(_word));
    }

    private string GetLanguageName(TargetLanguage language)
    {
        return language switch
        {
            TargetLanguage.English => "English",
            TargetLanguage.French => "French",
            TargetLanguage.German => "German",
            TargetLanguage.Italian => "Italian",
            _ => language.ToString()
        };
    }

    private string GetTimeAgo(DateTime time)
    {
        TimeSpan span = DateTime.Now - time;

        if (span.TotalMinutes < 1)
            return "just now";
        if (span.TotalHours < 1)
            return $"{(int)span.TotalMinutes}m ago";
        if (span.TotalDays < 1)
            return $"{(int)span.TotalHours}h ago";
        if (span.TotalDays < 7)
            return $"{(int)span.TotalDays}d ago";
        if (span.TotalDays < 30)
            return $"{(int)(span.TotalDays / 7)}w ago";

        return time.ToString("MMM d");
    }
}
