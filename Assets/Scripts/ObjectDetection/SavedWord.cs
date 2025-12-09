using System;

[Serializable]
public class SavedWord
{
    public string category;              // English name (e.g., "chair")
    public string savedTranslation;      // Translation at time of first detection
    public TargetLanguage savedLanguage; // Language when first detected

    // Store as ISO 8601 strings for JsonUtility compatibility
    public string firstDetectedTime;     // When first detected
    public string lastSeenTime;          // Most recent detection
    public int timesDetected;            // How many times seen

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

    public SavedWord(string category, string translation, TargetLanguage language)
    {
        this.category = category;
        this.savedTranslation = translation;
        this.savedLanguage = language;
        this.firstDetectedTime = DateTime.Now.ToString("o");
        this.lastSeenTime = DateTime.Now.ToString("o");
        this.timesDetected = 1;
    }
}
