using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class TranslateWords : MonoBehaviour
{
    [SerializeField] private string _deeplApiKey = "547b5191-724a-4565-8381-f7f4f2951cd5:fx";
    [SerializeField] private string _targetLanguage = "IT"; // Change to your target language (e.g., "ES", "FR", "DE")
    
    private const string DEEPL_API_URL = "https://api-free.deepl.com/v2/translate"; // Use "https://api.deepl.com/v2/translate" for paid plans
    
    public bool IsTranslationEnabled { get; private set; } = false;

    public void ToggleTranslation()
    {
        IsTranslationEnabled = !IsTranslationEnabled;
        Debug.Log($"Translation {(IsTranslationEnabled ? "enabled" : "disabled")}");
    }

    public void SetTranslationEnabled(bool enabled)
    {
        IsTranslationEnabled = enabled;
    }

    public void TranslateText(string text, Action<string> onSuccess, Action<string> onError)
    {
        if (!IsTranslationEnabled)
        {
            onSuccess?.Invoke(text);
            return;
        }

        if (string.IsNullOrEmpty(_deeplApiKey) || _deeplApiKey == "YOUR_DEEPL_API_KEY_HERE")
        {
            Debug.LogWarning("DeepL API key not set!");
            onError?.Invoke("API key not configured");
            return;
        }

        StartCoroutine(TranslateCoroutine(text, onSuccess, onError));
    }

    private IEnumerator TranslateCoroutine(string text, Action<string> onSuccess, Action<string> onError)
    {
        WWWForm form = new WWWForm();
        form.AddField("auth_key", _deeplApiKey);
        form.AddField("text", text);
        form.AddField("target_lang", _targetLanguage);

        using (UnityWebRequest request = UnityWebRequest.Post(DEEPL_API_URL, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    DeepLResponse response = JsonUtility.FromJson<DeepLResponse>(jsonResponse);
                    
                    if (response.translations != null && response.translations.Length > 0)
                    {
                        onSuccess?.Invoke(response.translations[0].text);
                    }
                    else
                    {
                        onError?.Invoke("No translation found");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to parse DeepL response: {e.Message}");
                    onError?.Invoke("Failed to parse response");
                }
            }
            else
            {
                Debug.LogError($"DeepL API Error: {request.error}");
                onError?.Invoke(request.error);
            }
        }
    }

    [System.Serializable]
    private class DeepLResponse
    {
        public Translation[] translations;
    }

    [System.Serializable]
    private class Translation
    {
        public string detected_source_language;
        public string text;
    }
}
