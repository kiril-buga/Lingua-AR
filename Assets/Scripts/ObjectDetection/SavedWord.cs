using System;

[Serializable]
public class SavedWord
{
    public string category;              // English name (e.g., "chair")
    public string savedTranslation;      // Translation at time of first detection
    public TargetLanguage savedLanguage; // Language when first detected
    public DateTime firstDetectedTime;   // When first detected
    public DateTime lastSeenTime;        // Most recent detection
    public int timesDetected;            // How many times seen

    public SavedWord(string category, string translation, TargetLanguage language)
    {
        this.category = category;
        this.savedTranslation = translation;
        this.savedLanguage = language;
        this.firstDetectedTime = DateTime.Now;
        this.lastSeenTime = DateTime.Now;
        this.timesDetected = 1;
    }
}
