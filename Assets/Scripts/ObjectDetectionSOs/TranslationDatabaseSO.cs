using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject database containing pre-translated object class names.
/// Supports offline translation for all 206 Lightship detectable objects.
/// </summary>
[CreateAssetMenu(fileName = "TranslationDatabase", menuName = "LinguaAR/Translation/Translation Database")]
public class TranslationDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public class MultiLanguageTranslation
    {
        public string english;
        public string french;
        public string german;
        public string italian;

        public string GetTranslation(TargetLanguage language)
        {
            return language switch
            {
                TargetLanguage.French => french,
                TargetLanguage.German => german,
                TargetLanguage.Italian => italian,
                TargetLanguage.English => english,
                _ => english
            };
        }
    }

    [System.Serializable]
    public class TranslationEntry
    {
        [Tooltip("Lightship object class name (e.g., 'chair', 'table')")]
        public string objectClass;

        [Tooltip("Translations in all supported languages")]
        public MultiLanguageTranslation translations;
    }

    [SerializeField]
    private List<TranslationEntry> _translations = new List<TranslationEntry>();

    private Dictionary<string, MultiLanguageTranslation> _translationCache;

    /// <summary>
    /// Gets the translation for a given object class in the specified language.
    /// </summary>
    /// <param name="objectClass">The Lightship object class name (e.g., "chair")</param>
    /// <param name="language">Target language</param>
    /// <returns>Translated string, or the original objectClass if not found</returns>
    public string GetTranslation(string objectClass, TargetLanguage language)
    {
        if (_translationCache == null)
            BuildCache();

        if (!_translationCache.TryGetValue(objectClass, out var translation))
        {
            Debug.LogWarning($"[TranslationDatabase] No translation found for '{objectClass}', using original name");
            return FormatObjectClassName(objectClass);
        }

        return translation.GetTranslation(language);
    }

    /// <summary>
    /// Checks if a translation exists for the given object class.
    /// </summary>
    public bool HasTranslation(string objectClass)
    {
        if (_translationCache == null)
            BuildCache();

        return _translationCache.ContainsKey(objectClass);
    }

    /// <summary>
    /// Gets all available object classes.
    /// </summary>
    public List<string> GetAllObjectClasses()
    {
        if (_translationCache == null)
            BuildCache();

        return new List<string>(_translationCache.Keys);
    }

    /// <summary>
    /// Builds the internal dictionary cache for fast lookups.
    /// </summary>
    private void BuildCache()
    {
        _translationCache = new Dictionary<string, MultiLanguageTranslation>();

        foreach (var entry in _translations)
        {
            if (string.IsNullOrEmpty(entry.objectClass))
                continue;

            if (_translationCache.ContainsKey(entry.objectClass))
            {
                Debug.LogWarning($"[TranslationDatabase] Duplicate entry for '{entry.objectClass}', skipping");
                continue;
            }

            _translationCache[entry.objectClass] = entry.translations;
        }

        Debug.Log($"[TranslationDatabase] Cached {_translationCache.Count} translations");
    }

    /// <summary>
    /// Formats object class name for display (converts underscores to spaces, capitalizes).
    /// </summary>
    private string FormatObjectClassName(string objectClass)
    {
        if (string.IsNullOrEmpty(objectClass))
            return objectClass;

        // Replace underscores with spaces and capitalize first letter
        string formatted = objectClass.Replace('_', ' ');
        if (formatted.Length > 0)
        {
            formatted = char.ToUpper(formatted[0]) + formatted.Substring(1);
        }

        return formatted;
    }

    /// <summary>
    /// Clears the cache (useful for editor refreshes).
    /// </summary>
    public void ClearCache()
    {
        _translationCache = null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor utility: Adds a new translation entry.
    /// </summary>
    public void AddTranslation(string objectClass, string english, string french, string german, string italian)
    {
        var entry = new TranslationEntry
        {
            objectClass = objectClass,
            translations = new MultiLanguageTranslation
            {
                english = english,
                french = french,
                german = german,
                italian = italian
            }
        };

        _translations.Add(entry);
        ClearCache();
        UnityEditor.EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Editor utility: Sorts translations alphabetically.
    /// </summary>
    [ContextMenu("Sort Translations Alphabetically")]
    public void SortTranslations()
    {
        _translations.Sort((a, b) => string.Compare(a.objectClass, b.objectClass, StringComparison.Ordinal));
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("[TranslationDatabase] Translations sorted");
    }

    /// <summary>
    /// Editor utility: Validates all entries.
    /// </summary>
    [ContextMenu("Validate Translations")]
    public void ValidateTranslations()
    {
        int emptyCount = 0;
        int duplicateCount = 0;
        HashSet<string> seen = new HashSet<string>();

        foreach (var entry in _translations)
        {
            if (string.IsNullOrEmpty(entry.objectClass))
            {
                emptyCount++;
                continue;
            }

            if (seen.Contains(entry.objectClass))
            {
                duplicateCount++;
                Debug.LogWarning($"Duplicate: {entry.objectClass}");
            }
            else
            {
                seen.Add(entry.objectClass);
            }
        }

        Debug.Log($"[TranslationDatabase] Validation complete: {_translations.Count} total, {emptyCount} empty, {duplicateCount} duplicates");
    }
#endif
}

/// <summary>
/// Supported target languages for translation.
/// </summary>
public enum TargetLanguage
{
    English = 0,
    French = 1,
    German = 2,
    Italian = 3
}
