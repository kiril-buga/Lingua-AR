using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR

/// <summary>
/// iOS Text-to-Speech provider using AVSpeechSynthesizer.
/// </summary>
public class IOSTTSProvider : ITTSProvider
{
    // Native plugin imports
    [DllImport("__Internal")]
    private static extern void _iOSTTSSpeak(string text, string languageCode, float rate);

    [DllImport("__Internal")]
    private static extern void _iOSTTSStop();

    [DllImport("__Internal")]
    private static extern bool _iOSTTSIsSpeaking();

    public void Speak(string text, string languageCode, float rate)
    {
        try
        {
            _iOSTTSSpeak(text, languageCode, rate);
        }
        catch (Exception e)
        {
            Debug.LogError($"[IOSTTSProvider] Failed to speak: {e.Message}");
        }
    }

    public void Stop()
    {
        try
        {
            _iOSTTSStop();
        }
        catch (Exception e)
        {
            Debug.LogError($"[IOSTTSProvider] Failed to stop: {e.Message}");
        }
    }

    public bool IsSpeaking()
    {
        try
        {
            return _iOSTTSIsSpeaking();
        }
        catch (Exception e)
        {
            Debug.LogError($"[IOSTTSProvider] Failed to check speaking status: {e.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        Stop();
    }
}

#endif
