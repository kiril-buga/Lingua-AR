package com.linguaar.ttsplugin;

import android.app.Activity;
import android.speech.tts.TextToSpeech;
import android.util.Log;
import java.util.Locale;

/**
 * Android Text-to-Speech plugin for Unity.
 * Provides native TTS functionality using Android TextToSpeech API.
 */
public class TTSPlugin {
    private static final String TAG = "TTSPlugin";
    private static TTSPlugin instance;

    private TextToSpeech tts;
    private boolean isInitialized = false;

    private TTSPlugin(Activity activity) {
        // Initialize TextToSpeech
        tts = new TextToSpeech(activity, new TextToSpeech.OnInitListener() {
            @Override
            public void onInit(int status) {
                if (status == TextToSpeech.SUCCESS) {
                    isInitialized = true;
                    Log.d(TAG, "TTS initialized successfully");
                } else {
                    Log.e(TAG, "TTS initialization failed");
                }
            }
        });
    }

    /**
     * Gets or creates the singleton instance.
     */
    public static TTSPlugin getInstance(Activity activity) {
        if (instance == null) {
            instance = new TTSPlugin(activity);
        }
        return instance;
    }

    /**
     * Speaks the given text in the specified language.
     *
     * @param text         Text to speak
     * @param languageCode Language code (e.g., "en-US", "fr-FR", "de-DE", "it-IT")
     * @param rate         Speech rate (0.0 to 1.0, where 0.5 is normal)
     */
    public void speak(String text, String languageCode, float rate) {
        if (!isInitialized) {
            Log.w(TAG, "TTS not initialized yet");
            return;
        }

        if (text == null || text.isEmpty()) {
            Log.w(TAG, "Cannot speak empty text");
            return;
        }

        try {
            // Parse language code (e.g., "en-US" -> Locale.US)
            Locale locale = parseLocale(languageCode);

            // Check if language is available
            int result = tts.isLanguageAvailable(locale);
            if (result == TextToSpeech.LANG_MISSING_DATA || result == TextToSpeech.LANG_NOT_SUPPORTED) {
                Log.w(TAG, "Language not supported: " + languageCode + ", falling back to default");
                locale = Locale.getDefault();
            }

            // Set language
            tts.setLanguage(locale);

            // Set speech rate (Android expects 0.5-2.0, where 1.0 is normal)
            // Our rate is 0.0-1.0, so we map it: rate * 1.5 + 0.5 = 0.5 to 2.0
            float androidRate = rate * 1.5f + 0.5f;
            tts.setSpeechRate(androidRate);

            // Set pitch (1.0 = normal)
            tts.setPitch(1.0f);

            // Stop any ongoing speech
            tts.stop();

            // Speak
            tts.speak(text, TextToSpeech.QUEUE_FLUSH, null, null);

            Log.d(TAG, "Speaking: " + text + " (Language: " + languageCode + ", Rate: " + rate + ")");
        } catch (Exception e) {
            Log.e(TAG, "Failed to speak: " + e.getMessage());
        }
    }

    /**
     * Stops any ongoing speech.
     */
    public void stop() {
        if (tts != null && tts.isSpeaking()) {
            tts.stop();
            Log.d(TAG, "Stopped speaking");
        }
    }

    /**
     * Checks if TTS is currently speaking.
     */
    public boolean isSpeaking() {
        return tts != null && tts.isSpeaking();
    }

    /**
     * Shuts down the TTS engine.
     */
    public void shutdown() {
        if (tts != null) {
            tts.stop();
            tts.shutdown();
            Log.d(TAG, "TTS shut down");
        }
    }

    /**
     * Parses a language code string into a Locale.
     */
    private Locale parseLocale(String languageCode) {
        if (languageCode == null || languageCode.isEmpty()) {
            return Locale.getDefault();
        }

        String[] parts = languageCode.split("-");
        if (parts.length == 2) {
            return new Locale(parts[0], parts[1]);
        } else if (parts.length == 1) {
            return new Locale(parts[0]);
        } else {
            return Locale.getDefault();
        }
    }
}
