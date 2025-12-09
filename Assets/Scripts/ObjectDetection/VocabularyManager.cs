using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VocabularyManager : MonoBehaviour
{
    public static VocabularyManager Instance { get; private set; }
    public static event Action OnVocabularyUpdated;

    private VocabularyData _vocabulary;
    private const string SAVE_FILE = "vocabulary.json";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVocabulary();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SaveVocabulary();
        }
    }

    public void SaveWord(string category, string translation, TargetLanguage language)
    {
        if (!_vocabulary.Contains(category))
        {
            var newWord = new SavedWord(category, translation, language);
            _vocabulary.AddOrUpdate(newWord);
            SaveVocabulary();
            OnVocabularyUpdated?.Invoke();
            Debug.Log($"[VocabularyManager] Saved new word: {category} ({translation})");
        }
        else
        {
            Debug.Log($"[VocabularyManager] Word already saved: {category}");
        }
    }

    public void DeleteWord(string category)
    {
        _vocabulary.savedWords.RemoveAll(w => w.category == category);
        SaveVocabulary();
        OnVocabularyUpdated?.Invoke();
        Debug.Log($"[VocabularyManager] Deleted word: {category}");
    }

    public List<SavedWord> GetAllWords()
    {
        return _vocabulary.savedWords;
    }

    public bool IsWordSaved(string category)
    {
        return _vocabulary.Contains(category);
    }

    public void ClearVocabulary()
    {
        _vocabulary = new VocabularyData();
        SaveVocabulary();
        OnVocabularyUpdated?.Invoke();
        Debug.Log("[VocabularyManager] Vocabulary cleared");
    }

    private void LoadVocabulary()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                _vocabulary = JsonUtility.FromJson<VocabularyData>(json);
                Debug.Log($"[VocabularyManager] Loaded {_vocabulary.savedWords.Count} words from {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[VocabularyManager] Error loading vocabulary: {e.Message}");
                _vocabulary = new VocabularyData();
            }
        }
        else
        {
            _vocabulary = new VocabularyData();
            Debug.Log($"[VocabularyManager] No save file found, starting fresh at {path}");
        }
    }

    private void SaveVocabulary()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
        try
        {
            string json = JsonUtility.ToJson(_vocabulary, prettyPrint: true);
            File.WriteAllText(path, json);
            Debug.Log($"[VocabularyManager] Saved {_vocabulary.savedWords.Count} words to {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[VocabularyManager] Error saving vocabulary: {e.Message}");
        }
    }
}
