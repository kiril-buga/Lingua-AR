# Lingua AR

**Augmented Reality Language Learning Application**

Lingua AR is an immersive AR mobile application that helps users learn new languages by detecting real-world objects and providing instant translations, example sentences, and audio pronunciation. Built with Unity and powered by Niantic Lightship ARDK.

---

## 📱 Overview

Lingua AR enables bilingual language learning where users can:
- Learn a **target language** (e.g., Italian) while speaking their **source language** (e.g., French)
- Point their camera at real-world objects to get instant translations
- Save vocabulary words for later review
- Listen to native pronunciation
- View contextual example sentences

**Supported Languages:** English, French, German, Italian

**Platforms:** iOS (ARKit), Android (ARCore), VR (OpenXR), Unity Editor (XR Simulation)

---

## 🎯 Key Features

### 1. **Real-Time Object Detection**
- Uses Niantic Lightship AR for advanced object recognition
- Detects 80+ common object categories (furniture, animals, vehicles, etc.)
- Confidence-based filtering (0.6 threshold)
- Visual bounding boxes with color-coded labels

### 2. **Bilingual Translation System**
- **Source Language**: Language you already speak
- **Target Language**: Language you want to learn
- Offline translation database with 4 languages
- Displays: "Source → Target" format (e.g., "la chaise → la sedia")

### 3. **Vocabulary Management**
- Manual save: Tap objects to add to vocabulary
- Persistent storage via JSON
- Delete unwanted words
- Track detection count and last seen time
- Word count badge indicator

### 4. **Example Sentences**
- Context-rich example sentences for each word
- Bilingual display (source → target)
- Database of pre-written sentences

### 5. **Audio Pronunciation**
- Text-to-speech for target language
- Native platform integration (iOS/Android)
- Instant playback on demand

---

## 🏗️ System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         LINGUA AR                                │
│                    (Unity 6000.2.10f1)                          │
└─────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
   ┌────▼────┐          ┌────▼────┐          ┌────▼────┐
   │   AR    │          │   UI    │          │  Data   │
   │ Systems │          │ Layer   │          │ Layer   │
   └─────────┘          └─────────┘          └─────────┘
        │                     │                     │
        └─────────────────────┴─────────────────────┘
                              │
                    ┌─────────┴─────────┐
                    │                   │
              ┌─────▼─────┐       ┌────▼────┐
              │  Niantic  │       │Platform │
              │ Lightship │       │ Services│
              └───────────┘       └─────────┘
```

### Detailed Component Architecture

```
┌───────────────────────────────────────────────────────────────────────┐
│                          AR DETECTION LAYER                            │
├───────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │          ARObjectDetectionManager (Niantic Lightship)        │    │
│  │                    - Hardware camera input                    │    │
│  │                    - ML object recognition                    │    │
│  │                    - 80+ object categories                    │    │
│  └──────────────────────┬────────────────────────────────────────┘    │
│                         │                                             │
│  ┌──────────────────────▼─────────────────────────────────────┐      │
│  │          ObjectDetectionSample.cs (Controller)              │      │
│  │  - Filters by confidence (0.6 threshold)                   │      │
│  │  - Fetches translations (source + target)                  │      │
│  │  - Emits OnFoundItemAtPosition event                       │      │
│  │  - Can pause/resume detection                              │      │
│  └─────────┬──────────────────────────────────────────────────┘      │
│            │                                                           │
└────────────┼───────────────────────────────────────────────────────────┘
             │
             ├──────────────────────────────┐
             │                              │
┌────────────▼──────────┐    ┌─────────────▼──────┐
│  UI VISUALIZATION     │    │  TRANSLATION        │
│                       │    │                     │
│  DrawRect.cs          │    │  TranslationDB      │
│  - Object pooling     │    │  - Offline lookup   │
│  - Bounding boxes     │    │  - 4 languages      │
│  - Labels             │    │  - 80+ words        │
│  - Color coding       │    │                     │
└───────────────────────┘    └─────────────────────┘


┌───────────────────────────────────────────────────────────────────────┐
│                       USER INTERACTION LAYER                           │
├───────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  USER TAPS DETECTED OBJECT                                            │
│         │                                                              │
│         ▼                                                              │
│  ┌──────────────────────────────────────────────────────────┐        │
│  │          UIRectObject.cs (Clickable Rectangle)            │        │
│  │  - Category, Translation, SourceTranslation              │        │
│  │  - Confidence, ScreenPosition                            │        │
│  │  - OnPointerClick handler                                │        │
│  └──────────────────┬───────────────────────────────────────┘        │
│                     │                                                  │
│                     ▼                                                  │
│  ┌──────────────────────────────────────────────────────────┐        │
│  │       ObjectSelectionManager.cs (Focus Handler)           │        │
│  │  - Focuses clicked object                                │        │
│  │  - Hides other rectangles                                │        │
│  │  - Fires OnObjectFocused event                           │        │
│  │  - Creates DetectedObjectData                            │        │
│  └──────────────────┬───────────────────────────────────────┘        │
│                     │                                                  │
│                     ▼                                                  │
│  ┌──────────────────────────────────────────────────────────┐        │
│  │            ActionMenuPanel.cs (Action Menu)               │        │
│  │                                                           │        │
│  │  ┌────────────────┐  ┌──────────────┐  ┌────────────┐  │        │
│  │  │ Pronunciation  │  │  Examples    │  │ Save Word  │  │        │
│  │  │   Button       │  │   Button     │  │   Button   │  │        │
│  │  └────────┬───────┘  └──────┬───────┘  └─────┬──────┘  │        │
│  │           │                  │                │          │        │
│  └───────────┼──────────────────┼────────────────┼──────────┘        │
│              │                  │                │                    │
└──────────────┼──────────────────┼────────────────┼────────────────────┘
               │                  │                │
               ▼                  ▼                ▼
       ┌───────────────┐  ┌──────────────┐  ┌──────────────────┐
       │  TTS Manager  │  │  Examples    │  │    Vocabulary    │
       │  (Platform)   │  │    Panel     │  │     Manager      │
       └───────────────┘  └──────────────┘  └──────────────────┘


┌───────────────────────────────────────────────────────────────────────┐
│                      VOCABULARY SYSTEM                                 │
├───────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  ┌──────────────────────────────────────────────────────────┐        │
│  │         VocabularyManager.cs (Singleton)                  │        │
│  │  - SaveWord(category, translation, languages)            │        │
│  │  - DeleteWord(category)                                  │        │
│  │  - GetAllWords() → List<SavedWord>                       │        │
│  │  - IsWordSaved(category) → bool                          │        │
│  │  - Fires OnVocabularyUpdated event                       │        │
│  └──────────────────┬───────────────────────────────────────┘        │
│                     │                                                  │
│         ┌───────────┴───────────┐                                     │
│         │                       │                                     │
│         ▼                       ▼                                     │
│  ┌──────────────┐      ┌────────────────┐                           │
│  │VocabularyData│      │   SavedWord    │                           │
│  │              │      │   - category    │                           │
│  │- Contains()  │      │   - translation │                           │
│  │- GetWord()   │      │   - source/tgt  │                           │
│  │- AddOrUpdate()│     │   - timestamps  │                           │
│  └──────────────┘      │   - timesDetect │                           │
│                        └────────────────┘                             │
│         │                                                              │
│         ▼                                                              │
│  ┌──────────────────────────────────────┐                            │
│  │      JSON Persistence                │                            │
│  │  Application.persistentDataPath       │                            │
│  │  /vocabulary.json                     │                            │
│  └──────────────────────────────────────┘                            │
│                                                                        │
│  Subscribers to OnVocabularyUpdated:                                  │
│  ┌────────────────────┐    ┌──────────────────┐                     │
│  │ VocabularyListPanel│    │  VocabularyBadge │                     │
│  │ - Refreshes list   │    │  - Updates count │                     │
│  │ - Object pooling   │    │  - Shows/hides   │                     │
│  └────────────────────┘    └──────────────────┘                     │
│                                                                        │
└────────────────────────────────────────────────────────────────────────┘


┌───────────────────────────────────────────────────────────────────────┐
│                      LANGUAGE SETTINGS SYSTEM                          │
├───────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  ┌──────────────────────────────────────────────────────────┐        │
│  │      LanguageSettings.cs (ScriptableObject)               │        │
│  │                                                           │        │
│  │  Properties:                                              │        │
│  │  - CurrentLanguage (Target: e.g., Italian)               │        │
│  │  - CurrentSourceLanguage (Source: e.g., French)          │        │
│  │                                                           │        │
│  │  Events:                                                  │        │
│  │  - OnLanguageChanged                                     │        │
│  │  - OnSourceLanguageChanged                               │        │
│  │                                                           │        │
│  │  Persistence:                                             │        │
│  │  - PlayerPrefs: "LinguaAR_TargetLanguage"                │        │
│  │  - PlayerPrefs: "LinguaAR_SourceLanguage"                │        │
│  │                                                           │        │
│  │  Static Helpers:                                          │        │
│  │  - GetLanguageDisplayName() → "Français"                 │        │
│  │  - GetLanguageFlag() → "🇫🇷"                             │        │
│  │  - GetLanguageCode() → "fr"                              │        │
│  └──────────────────┬───────────────────────────────────────┘        │
│                     │                                                  │
│         ┌───────────┴──────────┐                                      │
│         │                      │                                      │
│         ▼                      ▼                                      │
│  ┌──────────────┐      ┌────────────────────┐                       │
│  │  Main Menu   │      │   AR Scene Uses    │                       │
│  │              │      │                    │                       │
│  │- Target      │      │- Object Detection  │                       │
│  │  Dropdown    │      │- Translation       │                       │
│  │- Source      │      │- Examples Panel    │                       │
│  │  Dropdown    │      │- Vocabulary Save   │                       │
│  │- Validation  │      └────────────────────┘                       │
│  └──────────────┘                                                     │
│                                                                        │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Data Flow Diagrams

### 1. Object Detection → Translation Flow

```
Camera Feed
    │
    ▼
ARObjectDetectionManager (Lightship)
    │
    ├─► Detects: "chair" (confidence: 0.85)
    │
    ▼
ObjectDetectionSample.cs
    │
    ├─► Filters: confidence >= 0.6? ✓
    │
    ├─► Fetches Translation (Source):
    │       TranslationDatabase.GetTranslation("chair", French)
    │       → "la chaise"
    │
    ├─► Fetches Translation (Target):
    │       TranslationDatabase.GetTranslation("chair", Italian)
    │       → "la sedia"
    │
    ▼
DrawRect.CreateRect(...)
    │
    ├─► Creates UIRectObject with:
    │       - Category: "chair"
    │       - SourceTranslation: "la chaise"
    │       - Translation: "la sedia"
    │       - Confidence: 0.85
    │
    ▼
Screen displays: "la chaise" label over detected chair
```

### 2. User Interaction → Vocabulary Save Flow

```
User taps detection rectangle
    │
    ▼
UIRectObject.OnPointerClick()
    │
    ▼
ObjectSelectionManager.FocusObject(rect)
    │
    ├─► Creates DetectedObjectData:
    │       {
    │         category: "chair",
    │         translation: "la sedia",
    │         sourceTranslation: "la chaise",
    │         confidence: 0.85
    │       }
    │
    ├─► Fires: OnObjectFocused event
    │
    ▼
ActionMenuPanel.ShowMenu(data)
    │
    ├─► Displays: "la chaise → la sedia"
    │
    ├─► Shows 4 buttons:
    │       [🔊 Pronunciation] [📝 Examples] [💾 Save] [❌ Close]
    │
    ▼
User clicks "Save Word"
    │
    ▼
ActionMenuPanel.OnSaveWordClicked()
    │
    ▼
VocabularyManager.SaveWord(
    "chair",
    "la sedia",
    Italian,
    "la chaise",
    French
)
    │
    ├─► Creates/Updates SavedWord:
    │       {
    │         category: "chair",
    │         savedTranslation: "la sedia",
    │         savedLanguage: Italian,
    │         sourceTranslation: "la chaise",
    │         sourceLanguage: French,
    │         firstDetectedTime: "2025-01-15T10:30:00",
    │         lastSeenTime: "2025-01-15T10:30:00",
    │         timesDetected: 1
    │       }
    │
    ├─► Writes to: vocabulary.json
    │
    ├─► Fires: OnVocabularyUpdated event
    │
    ▼
VocabularyBadge.UpdateBadge()
    │
    └─► Updates badge count: "1"
```

### 3. Language Change Propagation Flow

```
User changes language in Main Menu
    │
    ▼
LanguageSelectionUI.OnDropdownValueChanged()
    │
    ├─► Validation: Source ≠ Target? ✓
    │
    ▼
LanguageSettings.CurrentSourceLanguage = French
    │
    ├─► Saves to PlayerPrefs
    │
    ├─► Fires: OnSourceLanguageChanged event
    │
    ▼
All Subscribers React:
    │
    ├─► ObjectDetectionSample:
    │       - Fetches new source translations
    │       - Updates detection labels
    │
    ├─► ExampleSentencesPanel:
    │       - Displays examples in French
    │
    ├─► ActionMenuPanel:
    │       - Shows "French → Italian" format
    │
    └─► VocabularyManager:
            - Saves words with French source
```

---

## 🛠️ Core Components Reference

### AR Detection Components

| Component | Responsibility | Key Methods |
|-----------|----------------|-------------|
| **ARObjectDetectionManager** | Hardware camera access, ML inference | (Niantic Lightship API) |
| **ObjectDetectionSample** | Detection controller, filtering, translation | `OnFrameReceived()`, `Pause()`, `Resume()` |
| **DrawRect** | UI visualization, object pooling | `CreateRect()`, `ClearRects()` |
| **UIRectObject** | Clickable detection rectangle | `OnPointerClick()`, `SetDetectionData()` |
| **Depth_ScreenToWorldPosition** | 2D→3D coordinate conversion | `GetWorldPosition()` |

### User Interaction Components

| Component | Responsibility | Key Methods |
|-----------|----------------|-------------|
| **ObjectSelectionManager** | Focus management, event emission | `FocusObject()`, `UnfocusObject()` |
| **ActionMenuPanel** | Action menu UI, button handlers | `ShowMenu()`, `Hide()`, `OnSaveWordClicked()` |
| **ExampleSentencesPanel** | Example sentences display | `ShowExamples()`, `CreateSentenceItem()` |
| **TTSManager** | Text-to-speech (platform-specific) | `Speak()`, `Stop()` |

### Vocabulary System Components

| Component | Responsibility | Key Methods |
|-----------|----------------|-------------|
| **VocabularyManager** | CRUD operations, event emission | `SaveWord()`, `DeleteWord()`, `GetAllWords()` |
| **VocabularyListPanel** | List UI, object pooling, search | `RefreshList()`, `Show()`, `Hide()` |
| **VocabularyWordItem** | List item UI component | `Setup()`, `OnDeleteClicked()` |
| **VocabularyBadge** | Word count indicator | `UpdateBadge()` |

### Language & Translation Components

| Component | Responsibility | Key Methods |
|-----------|----------------|-------------|
| **LanguageSettings** | Global language state, persistence | Properties: `CurrentLanguage`, `CurrentSourceLanguage` |
| **TranslationDatabaseSO** | Offline translation lookup | `GetTranslation()`, `GetAllWords()` |
| **ExampleSentencesDatabaseSO** | Example sentences storage | `GetExamples()`, `GetTranslation()` |
| **LanguageSelectionUI** | Language selection UI, validation | `SetLanguage()`, `SetSourceLanguage()` |

---

## 🎨 Design Patterns Used

### 1. **Singleton Pattern**
Used for global access to managers:
```csharp
public class VocabularyManager : MonoBehaviour
{
    public static VocabularyManager Instance { get; private set; }
}
```
**Components:** VocabularyManager, ExampleSentencesPanel, ObjectSelectionManager, SpawnObjectsAroundObjectDetected

### 2. **Observer/Event Pattern**
Decouples components via events:
```csharp
public static event Action OnVocabularyUpdated;
public static event Action<TargetLanguage> OnLanguageChanged;
```
**Events:**
- `OnVocabularyUpdated` → VocabularyBadge, VocabularyListPanel
- `OnLanguageChanged` → ObjectDetectionSample, UI components
- `OnObjectFocused` → ActionMenuPanel

### 3. **Object Pooling**
Reduces GC pressure for frequently created/destroyed objects:
```csharp
// DrawRect maintains pool of UIRectObject instances
private List<GameObject> rects = new List<GameObject>();
```
**Components:** DrawRect, VocabularyListPanel

### 4. **ScriptableObject Configuration**
Data-driven design for game data:
```csharp
[CreateAssetMenu(fileName = "TranslationDB", menuName = "...")]
public class TranslationDatabaseSO : ScriptableObject
```
**Assets:** LanguageSettings, TranslationDatabaseSO, ExampleSentencesDatabaseSO, SpawnObjectToObjectClassSO

### 5. **Strategy Pattern**
Platform-specific implementations:
```csharp
#if UNITY_IOS
    private IOSTTSManager _tts;
#elif UNITY_ANDROID
    private AndroidTTSManager _tts;
#endif
```
**Components:** TTSManager with iOS/Android variants

---

## 🚀 Technical Stack

### Unity & AR Frameworks
- **Unity:** 6000.2.10f1 (Unity 6)
- **Render Pipeline:** Universal Render Pipeline (URP)
- **AR Foundation:** Cross-platform AR abstraction
- **Niantic Lightship ARDK:** Advanced AR features
  - Object detection (80+ categories)
  - Semantic segmentation
  - Depth estimation
  - Persistent anchors

### Platform-Specific
- **iOS:** ARKit + Lightship ARKit Loader
- **Android:** ARCore + Lightship ARCore Loader
- **VR:** OpenXR Loader
- **Editor:** Lightship Simulation + XR Environment Simulation

### APIs & Services
- **Translation:** Offline TranslationDatabaseSO
- **TTS:** Native iOS/Android speech synthesis
- **Persistence:** JSON (JsonUtility) + PlayerPrefs

### Third-Party Packages
- TextMeshPro (UI text rendering)
- Newtonsoft.Json (JSON serialization)
- Input System (new input handling)

---

## 📦 Installation & Setup

### Prerequisites
- **Unity 6000.2.10f1** (exact version required)
- **Unity Hub** recommended
- **Platform modules:**
  - iOS Build Support (for iOS builds)
  - Android Build Support (for Android builds)
  - OpenXR Plugin (for VR)

### Installation Steps

1. **Clone Repository**
   ```bash
   git clone <repository-url>
   cd "Lingua AR"
   ```

2. **Open in Unity Hub**
   - Open Unity Hub
   - Click "Add" → Select project folder
   - Unity version: 6000.2.10f1
   - Click "Open"

3. **Package Resolution**
   - Packages auto-resolve from `Packages/manifest.json`
   - Wait for compilation to complete

4. **Niantic Lightship Setup**
   - Obtain API key from [Niantic Lightship](https://lightship.dev)
   - Open `Assets/Settings/LightshipSettings.asset`
   - Paste API key

5. **Test in Editor**
   - Open `Assets/Scenes/AR Scene.unity`
   - Enter Play mode
   - XR Simulation activates automatically

---

## 🎮 Usage Guide

### Testing in Unity Editor

1. **Load AR Scene**
   - Open `Assets/Scenes/AR Scene.unity`

2. **Select Simulation Environment**
   - Window → XR → AR Foundation → XR Environment
   - Choose: Kitchen, Living Room, Office, etc.

3. **Controls**
   - **Mouse:** Look around
   - **WASD:** Move camera
   - **Right-click + drag:** Rotate view

4. **Interact**
   - Detection rectangles appear on objects
   - Click rectangle to open action menu
   - Test pronunciation, examples, vocabulary save

### Building for Mobile

#### iOS Build
1. File → Build Settings → iOS
2. Switch Platform
3. Player Settings:
   - Bundle Identifier: com.yourcompany.linguaar
   - Target SDK: iOS 14.0+
4. Build → Open in Xcode
5. Configure signing & deploy

#### Android Build
1. File → Build Settings → Android
2. Switch Platform
3. Player Settings:
   - Package Name: com.yourcompany.linguaar
   - Minimum API Level: 24
4. Build APK → Install on device

---

## 📄 License

This project is licensed under the MIT License.

---

## 🙏 Acknowledgments

- **Niantic Lightship** for AR object detection
- **Unity Technologies** for Unity 6 and ARFoundation
- Code samples adapted from Niantic Lightship documentation

---

**Built with ❤️ for language learners worldwide**
