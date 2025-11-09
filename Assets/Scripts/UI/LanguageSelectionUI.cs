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

        // Setup individual buttons if present
        SetupButtons();

        // Update UI to show current language
        UpdateUI();

        // Subscribe to language change events
        LanguageSettings.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        LanguageSettings.OnLanguageChanged -= OnLanguageChanged;
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

    private void SetLanguage(TargetLanguage language)
    {
        _languageSettings.CurrentLanguage = language;
        Debug.Log($"[LanguageSelectionUI] Language set to: {language}");
    }

    private void OnLanguageChanged(TargetLanguage newLanguage)
    {
        UpdateUI();
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

        // Highlight selected button
        HighlightSelectedButton();
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
