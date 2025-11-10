#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

// Global speech synthesizer instance
static AVSpeechSynthesizer* speechSynthesizer = nil;

// Initialize synthesizer on first use
void EnsureSynthesizerInitialized() {
    if (speechSynthesizer == nil) {
        speechSynthesizer = [[AVSpeechSynthesizer alloc] init];
    }
}

// C interface for Unity
extern "C" {

    /// Speaks the given text using iOS native TTS
    void _iOSTTSSpeak(const char* text, const char* languageCode, float rate) {
        if (text == NULL || languageCode == NULL) {
            NSLog(@"[TTSPlugin] Invalid parameters");
            return;
        }

        EnsureSynthesizerInitialized();

        NSString* nsText = [NSString stringWithUTF8String:text];
        NSString* nsLanguageCode = [NSString stringWithUTF8String:languageCode];

        // Stop any ongoing speech
        if ([speechSynthesizer isSpeaking]) {
            [speechSynthesizer stopSpeakingAtBoundary:AVSpeechBoundaryImmediate];
        }

        // Create speech utterance
        AVSpeechUtterance* utterance = [[AVSpeechUtterance alloc] initWithString:nsText];

        // Set language
        utterance.voice = [AVSpeechSynthesisVoice voiceWithLanguage:nsLanguageCode];

        // Set speech rate (0.0 = slowest, 1.0 = fastest, default = 0.5)
        utterance.rate = rate * AVSpeechUtteranceDefaultSpeechRate;

        // Set pitch (1.0 = normal)
        utterance.pitchMultiplier = 1.0;

        // Set volume (1.0 = maximum)
        utterance.volume = 1.0;

        // Speak
        [speechSynthesizer speakUtterance:utterance];

        NSLog(@"[TTSPlugin] Speaking: %@ (Language: %@, Rate: %.2f)", nsText, nsLanguageCode, rate);
    }

    /// Stops any ongoing speech
    void _iOSTTSStop() {
        EnsureSynthesizerInitialized();

        if ([speechSynthesizer isSpeaking]) {
            [speechSynthesizer stopSpeakingAtBoundary:AVSpeechBoundaryImmediate];
            NSLog(@"[TTSPlugin] Stopped speaking");
        }
    }

    /// Returns true if currently speaking
    bool _iOSTTSIsSpeaking() {
        EnsureSynthesizerInitialized();
        return [speechSynthesizer isSpeaking];
    }
}
