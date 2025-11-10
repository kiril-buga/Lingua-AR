using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ScriptableObject database containing example sentences for vocabulary words.
/// </summary>
[CreateAssetMenu(fileName = "ExampleSentencesDatabase", menuName = "LinguaAR/Example Sentences/Sentences Database")]
public class ExampleSentencesDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public class SentenceExample
    {
        [Tooltip("Original sentence in English")]
        public string englishSentence;

        [Tooltip("Translated sentence in target language")]
        public string translatedSentence;

        [Tooltip("Source attribution (optional)")]
        public string source = "Tatoeba";
    }

    [System.Serializable]
    public class WordExamples
    {
        [Tooltip("Object class name (e.g., 'chair', 'table')")]
        public string objectClass;

        [Tooltip("List of example sentences")]
        public List<SentenceExample> examples = new List<SentenceExample>();
    }

    [SerializeField]
    private List<WordExamples> _wordExamples = new List<WordExamples>();

    private Dictionary<string, List<SentenceExample>> _examplesCache;

    /// <summary>
    /// Gets example sentences for a given word.
    /// </summary>
    public List<SentenceExample> GetExamples(string objectClass)
    {
        if (_examplesCache == null)
            BuildCache();

        if (_examplesCache.TryGetValue(objectClass, out var examples))
        {
            return examples;
        }

        return new List<SentenceExample>();
    }

    /// <summary>
    /// Checks if examples exist for a given word.
    /// </summary>
    public bool HasExamples(string objectClass)
    {
        if (_examplesCache == null)
            BuildCache();

        return _examplesCache.ContainsKey(objectClass) && _examplesCache[objectClass].Count > 0;
    }

    /// <summary>
    /// Gets a random example sentence for a word.
    /// </summary>
    public SentenceExample GetRandomExample(string objectClass)
    {
        var examples = GetExamples(objectClass);
        if (examples.Count == 0)
            return null;

        int randomIndex = Random.Range(0, examples.Count);
        return examples[randomIndex];
    }

    private void BuildCache()
    {
        _examplesCache = new Dictionary<string, List<SentenceExample>>();

        foreach (var wordExample in _wordExamples)
        {
            if (string.IsNullOrEmpty(wordExample.objectClass))
                continue;

            _examplesCache[wordExample.objectClass] = wordExample.examples;
        }

        Debug.Log($"[ExampleSentencesDatabase] Cached examples for {_examplesCache.Count} words");
    }

    public void ClearCache()
    {
        _examplesCache = null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor utility: Adds example sentences for a word.
    /// </summary>
    public void AddExamples(string objectClass, List<SentenceExample> examples)
    {
        var existing = _wordExamples.FirstOrDefault(w => w.objectClass == objectClass);
        if (existing != null)
        {
            existing.examples.AddRange(examples);
        }
        else
        {
            _wordExamples.Add(new WordExamples
            {
                objectClass = objectClass,
                examples = examples
            });
        }

        ClearCache();
        UnityEditor.EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Editor utility: Sorts words alphabetically.
    /// </summary>
    [ContextMenu("Sort Words Alphabetically")]
    public void SortWords()
    {
        _wordExamples.Sort((a, b) => string.Compare(a.objectClass, b.objectClass, System.StringComparison.Ordinal));
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("[ExampleSentencesDatabase] Words sorted");
    }
#endif
}
