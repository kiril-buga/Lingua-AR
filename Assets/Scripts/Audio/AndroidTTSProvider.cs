using System;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR

/// <summary>
/// Android Text-to-Speech provider using Android TextToSpeech API.
/// </summary>
public class AndroidTTSProvider : ITTSProvider
{
    private AndroidJavaObject _ttsObject;
    private bool _isInitialized = false;

    public AndroidTTSProvider()
    {
        Initialize();
    }

    private void Initialize()
    {
        try
        {
            // Get Unity activity
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Get TTSPlugin class
            AndroidJavaClass pluginClass = new AndroidJavaClass("com.linguaar.ttsplugin.TTSPlugin");

            // Initialize TTS
            _ttsObject = pluginClass.CallStatic<AndroidJavaObject>("getInstance", activity);

            _isInitialized = true;
            Debug.Log("[AndroidTTSProvider] Initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"[AndroidTTSProvider] Initialization failed: {e.Message}");
            _isInitialized = false;
        }
    }

    public void Speak(string text, string languageCode, float rate)
    {
        if (!_isInitialized || _ttsObject == null)
        {
            Debug.LogWarning("[AndroidTTSProvider] TTS not initialized");
            return;
        }

        try
        {
            _ttsObject.Call("speak", text, languageCode, rate);
        }
        catch (Exception e)
        {
            Debug.LogError($"[AndroidTTSProvider] Failed to speak: {e.Message}");
        }
    }

    public void Stop()
    {
        if (!_isInitialized || _ttsObject == null)
            return;

        try
        {
            _ttsObject.Call("stop");
        }
        catch (Exception e)
        {
            Debug.LogError($"[AndroidTTSProvider] Failed to stop: {e.Message}");
        }
    }

    public bool IsSpeaking()
    {
        if (!_isInitialized || _ttsObject == null)
            return false;

        try
        {
            return _ttsObject.Call<bool>("isSpeaking");
        }
        catch (Exception e)
        {
            Debug.LogError($"[AndroidTTSProvider] Failed to check speaking status: {e.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        if (_ttsObject != null)
        {
            try
            {
                _ttsObject.Call("shutdown");
                _ttsObject.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogError($"[AndroidTTSProvider] Disposal failed: {e.Message}");
            }
        }
    }
}

#endif
