using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.ObjectDetection;
using UnityEngine;

public class ObjectDetectionSample : MonoBehaviour
{
    [SerializeField] private float _probabilityThreshold = .6f;
    [SerializeField] private float _minTimeBetweenUpdates = 0.15f;

    [SerializeField] private ARObjectDetectionManager _objectDetectionManager;

    private float _lastUpdateTime = 0f;

    private Color[] colors = new[]
    {
        Color.skyBlue,
        Color.seaGreen,
        Color.yellowNice,
        Color.magenta,
        Color.cyan,
        Color.white,
        Color.azure,
        Color.coral,
        Color.limeGreen,
    };

    [SerializeField] private ListSpawnObjectToObjectClassSO _objectToObjectClassSo;
    public static event Action<(string category, Vector2 rectPosition)> OnFoundItemAtPosition;

    private List<string> validChannels = new();

    [SerializeField] private DrawRect _drawRect;
    [SerializeField] private TranslateWords _translateWords; // DEPRECATED: Kept for backward compatibility

    [Header("Offline Translation System")]
    [SerializeField] private TranslationDatabaseSO _translationDatabase;
    [SerializeField] private LanguageSettings _languageSettings;

    private Canvas _canvas;
    private bool _isPaused = false;


    private void Awake()
    {
        _canvas = FindAnyObjectByType<Canvas>();
    }
    
    

    // Start is called before the first frame update
    void Start()
    {
        _objectDetectionManager.enabled = true;
        _objectDetectionManager.MetadataInitialized += ObjectDetectionManagerOnMetadataInitialized;
        SetObjectDetectionChannels();
    }
    
    private void OnDestroy()
    {
        _objectDetectionManager.MetadataInitialized -= ObjectDetectionManagerOnMetadataInitialized;
        _objectDetectionManager.ObjectDetectionsUpdated -= ObjectDetectionManagerOnObjectDetectionsUpdated;
    }

    private void ObjectDetectionManagerOnMetadataInitialized(ARObjectDetectionModelEventArgs obj)
    {
        _objectDetectionManager.ObjectDetectionsUpdated += ObjectDetectionManagerOnObjectDetectionsUpdated;
    }

    private void ObjectDetectionManagerOnObjectDetectionsUpdated(ARObjectDetectionsUpdatedEventArgs obj)
    {
        if (_isPaused)
            return;

        // Throttle updates to improve smoothness
        if (Time.time - _lastUpdateTime < _minTimeBetweenUpdates)
            return;

        _lastUpdateTime = Time.time;

        string resultString = "";
        float confidence = 0;
        string name = "";
        var result = obj.Results;
        
        if(result == null)
            return;
        
        _drawRect.ClearRects();

        for (int i = 0; i < result.Count; i++)
        {
            var detection = result[i];
            var categorization = detection.GetConfidentCategorizations(_probabilityThreshold);

            if (categorization.Count <= 0)
            {
                continue; // skip this detection and move to the next one
            }

            categorization.Sort((a,b) => b.Confidence.CompareTo(a.Confidence));


            var categoryToDisplay = categorization[0];

            // if (validChannels.Contains(categoryToDisplay.CategoryName))
            // { // uncomment if you want to filter by specific categories
            confidence = categoryToDisplay.Confidence;
            name = categoryToDisplay.CategoryName;

            int h = Mathf.FloorToInt(_canvas.GetComponent<RectTransform>().rect.height);
            int w = Mathf.FloorToInt(_canvas.GetComponent<RectTransform>().rect.width);

            var rect = result[i].CalculateRect(w, h, Screen.orientation);
        
            resultString = $"{name}: {confidence:F2}";

            // Get instant translation from offline database
            string translatedText = null;
            if (_translationDatabase != null && _languageSettings != null)
            {
                translatedText = _translationDatabase.GetTranslation(name, _languageSettings.CurrentLanguage);
            }
            else if (_translateWords != null && _translateWords.IsTranslationEnabled)
            {
                // FALLBACK: Legacy DeepL API (async, slower, requires internet)
                // This path is kept for backward compatibility but not recommended
                int capturedIndex = i;
                string capturedCategory = categoryToDisplay.CategoryName;
                float capturedConfidence = confidence;
                _translateWords.TranslateText(
                    name,
                    (legacyTranslatedText) =>
                    {
                        _drawRect.CreateRect(rect, colors[capturedIndex % colors.Length], resultString, legacyTranslatedText, capturedCategory, capturedConfidence);
                    },
                    (error) =>
                    {
                        Debug.LogWarning($"Translation failed: {error}");
                        _drawRect.CreateRect(rect, colors[capturedIndex % colors.Length], resultString, null, capturedCategory, capturedConfidence);
                    }
                );
                continue; // Skip the synchronous CreateRect call below
            }

            // Create rectangle with metadata (translation will show on click)
            _drawRect.CreateRect(rect, colors[i % colors.Length], resultString, translatedText, categoryToDisplay.CategoryName, confidence);
            
            OnFoundItemAtPosition?.Invoke((categoryToDisplay.CategoryName, rect.position));
                
            // } 
        }
    }

    void SetObjectDetectionChannels()
    {
        foreach (var spawnObjectToObjectClass in _objectToObjectClassSo._SpawnObjectToObjectClassSos)
        {
            validChannels.Add(spawnObjectToObjectClass.detectionClass);
        }
    }

    public void PauseDetection(float duration)
    {
        if (!_isPaused)
        {
            StartCoroutine(PauseDetectionCoroutine(duration));
        }
    }

    private IEnumerator PauseDetectionCoroutine(float duration)
    {
        _isPaused = true;
        Debug.Log($"Object detection paused for {duration} seconds");
        yield return new WaitForSeconds(duration);
        _isPaused = false;
        Debug.Log("Object detection resumed");
    }

    /// <summary>
    /// Toggles object detection on/off. Connect this to your Toggle button in the Inspector.
    /// </summary>
    public void ToggleObjectDetection()
    {
        _isPaused = !_isPaused;

        // Clear existing rectangles when disabling detection
        if (_isPaused && _drawRect != null)
        {
            _drawRect.ClearRects();
        }

        Debug.Log($"Object detection {(_isPaused ? "disabled" : "enabled")}");
    }

    /// <summary>
    /// Enables object detection
    /// </summary>
    public void EnableObjectDetection()
    {
        _isPaused = false;
        Debug.Log("Object detection enabled");
    }

    /// <summary>
    /// Disables object detection and clears all UI rectangles
    /// </summary>
    public void DisableObjectDetection()
    {
        _isPaused = true;

        if (_drawRect != null)
        {
            _drawRect.ClearRects();
        }

        Debug.Log("Object detection disabled");
    }

    /// <summary>
    /// Pauses object detection without clearing rectangles (used for focus mode)
    /// </summary>
    public void PauseDetectionOnly()
    {
        _isPaused = true;
        Debug.Log("Object detection paused (rectangles preserved)");
    }
}