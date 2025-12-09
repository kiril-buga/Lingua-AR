using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class VocabularyData
{
    public List<SavedWord> savedWords = new List<SavedWord>();

    public bool Contains(string category)
    {
        return savedWords.Any(w => w.category == category);
    }

    public SavedWord GetWord(string category)
    {
        return savedWords.FirstOrDefault(w => w.category == category);
    }

    public void AddOrUpdate(SavedWord word)
    {
        var existing = GetWord(word.category);
        if (existing != null)
        {
            existing.LastSeenDateTime = DateTime.Now;
            existing.timesDetected++;
        }
        else
        {
            savedWords.Add(word);
        }
    }
}
