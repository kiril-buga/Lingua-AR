using System;
using UnityEngine;

/// <summary>
/// Cross-platform Text-to-Speech manager.
/// Uses native platform APIs for high-quality pronunciation.
/// </summary>
public class TTSManager : MonoBehaviour
{
    // ===== SINGLETON =====
    public static TTSManager Instance { get; private set; }

    // ===== SERIALIZED FIELDS =====
    [Header("Configuration")]
    [SerializeField] private LanguageSettings _languageSettings;
    [SerializeField] private bool _autoPlayOnFocus = false;
    [SerializeField] private float _speechRate = 0.5f; // 0.0 (slow) to 1.0 (fast)

    // ===== PRIVATE FIELDS =====
    private ITTSProvider _ttsProvider;
    private bool _isInitialized = false;

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
        DontDestroyOnLoad(gameObject); // Persist across scenes

        InitializeTTS();
    }

    private void OnEnable()
    {
        if (_autoPlayOnFocus)
        {
            ObjectSelectionManager.OnObjectFocused += OnObjectFocused;
        }
    }

    private void OnDisable()
    {
        ObjectSelectionManager.OnObjectFocused -= OnObjectFocused;
    }

    private void OnDestroy()
    {
        _ttsProvider?.Dispose();

        if (Instance == this)
            Instance = null;
    }

    // ===== PUBLIC METHODS =====

    /// <summary>
    /// Speaks the given text in the current language.
    /// </summary>
    public void Speak(string text)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("[TTSManager] TTS not initialized");
            return;
        }

        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("[TTSManager] Cannot speak empty text");
            return;
        }

        string languageCode = GetLanguageCode();
        Debug.Log($"[TTSManager] Speaking '{text}' in {languageCode}");

        _ttsProvider.Speak(text, languageCode, _speechRate);
    }

    /// <summary>
    /// Stops any ongoing speech.
    /// </summary>
    public void Stop()
    {
        _ttsProvider?.Stop();
    }

    /// <summary>
    /// Checks if TTS is currently speaking.
    /// </summary>
    public bool IsSpeaking()
    {
        return _ttsProvider?.IsSpeaking() ?? false;
    }

    // ===== PRIVATE METHODS =====

    private void InitializeTTS()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _ttsProvider = new IOSTTSProvider();
            Debug.Log("[TTSManager] Initialized iOS TTS");
        #elif UNITY_ANDROID && !UNITY_EDITOR
            _ttsProvider = new AndroidTTSProvider();
            Debug.Log("[TTSManager] Initialized Android TTS");
        #else
            _ttsProvider = new EditorTTSProvider();
            Debug.Log("[TTSManager] Initialized Editor TTS (logs only)");
        #endif

        _isInitialized = true;
    }

    private string GetLanguageCode()
    {
        if (_languageSettings == null)
            return "en-US";

        return _languageSettings.CurrentLanguage switch
        {
            TargetLanguage.English => "en-US",
            TargetLanguage.French => "fr-FR",
            TargetLanguage.German => "de-DE",
            TargetLanguage.Italian => "it-IT",
            _ => "en-US"
        };
    }

    private void OnObjectFocused(DetectedObjectData data)
    {
        if (_autoPlayOnFocus && !string.IsNullOrEmpty(data.translation))
        {
            Speak(data.translation);
        }
    }
}

// ===== INTERFACE =====

/// <summary>
/// Interface for platform-specific TTS implementations.
/// </summary>
public interface ITTSProvider : IDisposable
{
    void Speak(string text, string languageCode, float rate);
    void Stop();
    bool IsSpeaking();
}

// ===== EDITOR IMPLEMENTATION =====

/// <summary>
/// Editor fallback - logs to console instead of speaking.
/// </summary>
public class EditorTTSProvider : ITTSProvider
{
    public void Speak(string text, string languageCode, float rate)
    {
        Debug.Log($"[EditorTTS] 🔊 Speaking: '{text}' (Language: {languageCode}, Rate: {rate})");
    }

    public void Stop()
    {
        Debug.Log("[EditorTTS] ⏹️ Stopped");
    }

    public bool IsSpeaking()
    {
        return false;
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}
