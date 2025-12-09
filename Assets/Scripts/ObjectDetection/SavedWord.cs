using System;

[Serializable]
public class SavedWord
{
    public string category;                  // English name (e.g., "chair")
    public string savedTranslation;          // Target language translation at time of first detection
    public TargetLanguage savedLanguage;     // Target language when first detected
    public string sourceTranslation;         // Source language translation
    public TargetLanguage sourceLanguage;    // Source language

    // Store as ISO 8601 strings for JsonUtility compatibility
    public string firstDetectedTime;         // When first detected
    public string lastSeenTime;              // Most recent detection
    public int timesDetected;                // How many times seen

    // Helper properties to work with DateTime
    public DateTime FirstDetectedDateTime
    {
        get => DateTime.Parse(firstDetectedTime);
        set => firstDetectedTime = value.ToString("o"); // ISO 8601 format
    }

    public DateTime LastSeenDateTime
    {
        get => DateTime.Parse(lastSeenTime);
        set => lastSeenTime = value.ToString("o");
    }

    public SavedWord(string category, string translation, TargetLanguage language, string sourceTranslation = null, TargetLanguage sourceLanguage = TargetLanguage.English)
    {
        this.category = category;
        this.savedTranslation = translation;
        this.savedLanguage = language;
        this.sourceTranslation = sourceTranslation;
        this.sourceLanguage = sourceLanguage;
        this.firstDetectedTime = DateTime.Now.ToString("o");
        this.lastSeenTime = DateTime.Now.ToString("o");
        this.timesDetected = 1;
    }
}
