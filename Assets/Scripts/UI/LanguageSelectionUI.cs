using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for language selection.
/// Add this to your Main Menu or Settings screen.
/// </summary>
public class LanguageSelectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LanguageSettings _languageSettings;

    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private TMP_Text _currentLanguageText;
    [SerializeField] private TMP_Dropdown _sourceLanguageDropdown;
    [SerializeField] private TMP_Text _currentSourceLanguageText;

    [Header("Optional: Individual Language Buttons")]
    [SerializeField] private Button _englishButton;
    [SerializeField] private Button _frenchButton;
    [SerializeField] private Button _germanButton;
    [SerializeField] private Button _italianButton;

    private void Start()
    {
        if (_languageSettings == null)
        {
            Debug.LogError("[LanguageSelectionUI] LanguageSettings not assigned!");
            return;
        }

        // Initialize language settings
        _languageSettings.Initialize();

        // Setup dropdown if present
        if (_languageDropdown != null)
        {
            SetupDropdown();
        }

        // Setup source language dropdown if present
        if (_sourceLanguageDropdown != null)
        {
            SetupSourceLanguageDropdown();
        }

        // Setup individual buttons if present
        SetupButtons();

        // Update UI to show current language
        UpdateUI();

        // Subscribe to language change events
        LanguageSettings.OnLanguageChanged += OnLanguageChanged;
        LanguageSettings.OnSourceLanguageChanged += OnSourceLanguageChanged;
    }

    private void OnDestroy()
    {
        LanguageSettings.OnLanguageChanged -= OnLanguageChanged;
        LanguageSettings.OnSourceLanguageChanged -= OnSourceLanguageChanged;
    }

    private void SetupDropdown()
    {
        _languageDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData(LanguageSettings.GetLanguageDisplayName(TargetLanguage.English)));
        options.Add(new TMP_Dropdown.OptionData(LanguageSettings.GetLanguageDisplayName(TargetLanguage.French)));
        options.Add(new TMP_Dropdown.OptionData(LanguageSettings.GetLanguageDisplayName(TargetLanguage.German)));
        options.Add(new TMP_Dropdown.OptionData(LanguageSettings.GetLanguageDisplayName(TargetLanguage.Italian)));

        _languageDropdown.AddOptions(options);
        _languageDropdown.value = (int)_languageSettings.CurrentLanguage;
        _languageDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void SetupSourceLanguageDropdown()
    {
        _sourceLanguageDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData(LanguageSettings.GetLanguageDisplayName(TargetLanguage.English)));
        options.Add(new TMP_Dropdown.OptionData(LanguageSettings.GetLanguageDisplayName(TargetLanguage.French)));
        options.Add(new TMP_Dropdown.OptionData(LanguageSettings.GetLanguageDisplayName(TargetLanguage.German)));
        options.Add(new TMP_Dropdown.OptionData(LanguageSettings.GetLanguageDisplayName(TargetLanguage.Italian)));

        _sourceLanguageDropdown.AddOptions(options);
        _sourceLanguageDropdown.value = (int)_languageSettings.CurrentSourceLanguage;
        _sourceLanguageDropdown.onValueChanged.AddListener(OnSourceLanguageDropdownValueChanged);
    }

    private void SetupButtons()
    {
        if (_englishButton != null)
            _englishButton.onClick.AddListener(() => SetLanguage(TargetLanguage.English));

        if (_frenchButton != null)
            _frenchButton.onClick.AddListener(() => SetLanguage(TargetLanguage.French));

        if (_germanButton != null)
            _germanButton.onClick.AddListener(() => SetLanguage(TargetLanguage.German));

        if (_italianButton != null)
            _italianButton.onClick.AddListener(() => SetLanguage(TargetLanguage.Italian));
    }

    private void OnDropdownValueChanged(int index)
    {
        SetLanguage((TargetLanguage)index);
    }

    private void OnSourceLanguageDropdownValueChanged(int index)
    {
        SetSourceLanguage((TargetLanguage)index);
    }

    private void SetLanguage(TargetLanguage language)
    {
        // Prevent target and source from being the same
        if (language == _languageSettings.CurrentSourceLanguage)
        {
            Debug.LogWarning($"[LanguageSelectionUI] Cannot set target language to {language} - it's already the source language.");

            // Reset dropdown to previous valid value
            if (_languageDropdown != null)
            {
                _languageDropdown.SetValueWithoutNotify((int)_languageSettings.CurrentLanguage);
            }
            return;
        }

        _languageSettings.CurrentLanguage = language;
        Debug.Log($"[LanguageSelectionUI] Language set to: {language}");
    }

    private void SetSourceLanguage(TargetLanguage language)
    {
        // Prevent source and target from being the same
        if (language == _languageSettings.CurrentLanguage)
        {
            Debug.LogWarning($"[LanguageSelectionUI] Cannot set source language to {language} - it's already the target language.");

            // Reset dropdown to previous valid value
            if (_sourceLanguageDropdown != null)
            {
                _sourceLanguageDropdown.SetValueWithoutNotify((int)_languageSettings.CurrentSourceLanguage);
            }
            return;
        }

        _languageSettings.CurrentSourceLanguage = language;
        Debug.Log($"[LanguageSelectionUI] Source language set to: {language}");
    }

    private void OnLanguageChanged(TargetLanguage newLanguage)
    {
        UpdateUI();
    }

    private void OnSourceLanguageChanged(TargetLanguage newSourceLanguage)
    {
        UpdateSourceLanguageUI();
    }

    private void UpdateUI()
    {
        if (_currentLanguageText != null)
        {
            string flag = LanguageSettings.GetLanguageFlag(_languageSettings.CurrentLanguage);
            string name = LanguageSettings.GetLanguageDisplayName(_languageSettings.CurrentLanguage);
            _currentLanguageText.text = $"{flag} {name}";
        }

        if (_languageDropdown != null)
        {
            _languageDropdown.value = (int)_languageSettings.CurrentLanguage;
        }

        // Update source language UI as well
        UpdateSourceLanguageUI();

        // Highlight selected button
        HighlightSelectedButton();
    }

    private void UpdateSourceLanguageUI()
    {
        if (_currentSourceLanguageText != null)
        {
            string flag = LanguageSettings.GetLanguageFlag(_languageSettings.CurrentSourceLanguage);
            string name = LanguageSettings.GetLanguageDisplayName(_languageSettings.CurrentSourceLanguage);
            _currentSourceLanguageText.text = $"{flag} {name}";
        }

        if (_sourceLanguageDropdown != null)
        {
            _sourceLanguageDropdown.SetValueWithoutNotify((int)_languageSettings.CurrentSourceLanguage);
        }
    }

    private void HighlightSelectedButton()
    {
        // Reset all buttons to normal color
        ResetButtonColor(_englishButton);
        ResetButtonColor(_frenchButton);
        ResetButtonColor(_germanButton);
        ResetButtonColor(_italianButton);

        // Highlight the selected button
        Button selectedButton = _languageSettings.CurrentLanguage switch
        {
            TargetLanguage.English => _englishButton,
            TargetLanguage.French => _frenchButton,
            TargetLanguage.German => _germanButton,
            TargetLanguage.Italian => _italianButton,
            _ => null
        };

        if (selectedButton != null)
        {
            var colors = selectedButton.colors;
            colors.normalColor = new Color(0.3f, 0.8f, 1f); // Light blue
            selectedButton.colors = colors;
        }
    }

    private void ResetButtonColor(Button button)
    {
        if (button != null)
        {
            var colors = button.colors;
            colors.normalColor = Color.white;
            button.colors = colors;
        }
    }
}
