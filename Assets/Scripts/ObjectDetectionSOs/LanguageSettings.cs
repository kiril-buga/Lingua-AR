using System;
using UnityEngine;

/// <summary>
/// Global settings for the current target language.
/// Use this to switch languages at runtime.
/// </summary>
[CreateAssetMenu(fileName = "LanguageSettings", menuName = "LinguaAR/Settings/Language Settings")]
public class LanguageSettings : ScriptableObject
{
    [SerializeField]
    [Tooltip("Currently selected target language for translations and TTS")]
    private TargetLanguage _currentLanguage = TargetLanguage.Italian;

    /// <summary>
    /// Event fired when the language is changed.
    /// Subscribe to this to update UI or reload translations.
    /// </summary>
    public static event Action<TargetLanguage> OnLanguageChanged;

    /// <summary>
    /// Gets or sets the current target language.
    /// </summary>
    public TargetLanguage CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                SaveLanguagePreference();
                OnLanguageChanged?.Invoke(_currentLanguage);
                Debug.Log($"[LanguageSettings] Language changed to: {_currentLanguage}");
            }
        }
    }

    /// <summary>
    /// Gets the display name for a language.
    /// </summary>
    public static string GetLanguageDisplayName(TargetLanguage language)
    {
        return language switch
        {
            TargetLanguage.English => "English",
            TargetLanguage.French => "Français",
            TargetLanguage.German => "Deutsch",
            TargetLanguage.Italian => "Italiano",
            _ => language.ToString()
        };
    }

    /// <summary>
    /// Gets the flag emoji for a language (optional, for UI).
    /// </summary>
    public static string GetLanguageFlag(TargetLanguage language)
    {
        return language switch
        {
            TargetLanguage.English => "🇬🇧",
            TargetLanguage.French => "🇫🇷",
            TargetLanguage.German => "🇩🇪",
            TargetLanguage.Italian => "🇮🇹",
            _ => ""
        };
    }

    /// <summary>
    /// Gets the ISO 639-1 language code (for TTS and other APIs).
    /// </summary>
    public static string GetLanguageCode(TargetLanguage language)
    {
        return language switch
        {
            TargetLanguage.English => "en",
            TargetLanguage.French => "fr",
            TargetLanguage.German => "de",
            TargetLanguage.Italian => "it",
            _ => "en"
        };
    }

    /// <summary>
    /// Gets the DeepL API language code (for compatibility, if still needed).
    /// </summary>
    public static string GetDeepLLanguageCode(TargetLanguage language)
    {
        return language switch
        {
            TargetLanguage.English => "EN",
            TargetLanguage.French => "FR",
            TargetLanguage.German => "DE",
            TargetLanguage.Italian => "IT",
            _ => "EN"
        };
    }

    /// <summary>
    /// Initializes language settings on first load.
    /// Call this from a startup script or main menu.
    /// </summary>
    public void Initialize()
    {
        LoadLanguagePreference();
        Debug.Log($"[LanguageSettings] Initialized with language: {_currentLanguage}");
    }

    /// <summary>
    /// Saves the current language preference to PlayerPrefs.
    /// </summary>
    private void SaveLanguagePreference()
    {
        PlayerPrefs.SetInt("LinguaAR_TargetLanguage", (int)_currentLanguage);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the language preference from PlayerPrefs.
    /// </summary>
    private void LoadLanguagePreference()
    {
        if (PlayerPrefs.HasKey("LinguaAR_TargetLanguage"))
        {
            _currentLanguage = (TargetLanguage)PlayerPrefs.GetInt("LinguaAR_TargetLanguage");
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor utility: Resets to default language.
    /// </summary>
    [ContextMenu("Reset to Italian")]
    private void ResetToItalian()
    {
        CurrentLanguage = TargetLanguage.Italian;
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
