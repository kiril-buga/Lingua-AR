# TTS Pronunciation & Example Sentences - Setup Guide

## 🎉 What I Built

You now have two new powerful features:

### 1. **Platform Native TTS (Text-to-Speech)**
- Hear pronunciation of detected words
- Uses iOS AVSpeechSynthesizer and Android TextToSpeech
- Supports all 4 languages (English, French, German, Italian)
- High-quality native voices
- Works offline (voices are pre-installed on devices)

### 2. **Example Sentences System**
- Show real-world usage examples for vocabulary
- Display sentences in both English and target language
- Extensible database (easy to add more examples)
- Clean, scrollable UI panel

---

## 📦 Files Created

**TTS System:**
- `Assets/Scripts/Audio/TTSManager.cs` - Cross-platform manager
- `Assets/Scripts/Audio/IOSTTSProvider.cs` - iOS implementation
- `Assets/Scripts/Audio/AndroidTTSProvider.cs` - Android implementation
- `Assets/Plugins/iOS/TTSPlugin.mm` - iOS native plugin (Objective-C++)
- `Assets/Plugins/Android/TTSPlugin.java` - Android native plugin (Java)

**Example Sentences:**
- `Assets/Scripts/ObjectDetectionSOs/ExampleSentencesDatabaseSO.cs` - Database SO
- `Assets/Scripts/UI/ExampleSentencesPanel.cs` - UI panel for displaying sentences

**UI:**
- `Assets/Scripts/UI/ActionMenuPanel.cs` - Action menu with pronunciation + examples buttons

---

## 🚀 Setup Instructions (30 minutes)

### Part 1: TTS Setup (10 minutes)

#### Step 1: Create TTSManager GameObject

1. **Open AR Scene.unity**
2. **Create empty GameObject** in Hierarchy
3. **Name it:** `TTSManager`
4. **Add Component** → `TTSManager`
5. **In Inspector, configure:**
   - **Language Settings:** Drag your `LanguageSettings` asset
   - **Auto Play On Focus:** ✓ (check if you want auto-pronunciation on click)
   - **Speech Rate:** 0.5 (0.0 = slow, 1.0 = fast)

#### Step 2: Test TTS in Editor

1. **Press Play**
2. **Click on a detected object** (e.g., chair)
3. **Check Console** - you should see:
   ```
   [EditorTTS] 🔊 Speaking: 'Sedia' (Language: it-IT, Rate: 0.5)
   ```
4. **Note:** Editor only logs (no actual audio). Real audio works on iOS/Android.

---

### Part 2: Action Menu Setup (10 minutes)

The action menu appears when you focus on an object and provides buttons for pronunciation and examples.

#### Step 1: Create Action Menu UI

1. **In AR Scene**, find or create a **Canvas**
2. **Create UI → Panel** (Right-click Canvas → UI → Panel)
3. **Name it:** `ActionMenuPanel`
4. **Configure the Panel:**
   - Set **Anchor:** Center
   - Set **Position:** (0, 0, 0)
   - Set **Size:** Width: 300, Height: 200

#### Step 2: Add CanvasGroup

1. **Select ActionMenuPanel**
2. **Add Component** → `CanvasGroup`
3. **Set Alpha:** 0 (will fade in automatically)

#### Step 3: Add UI Elements

Inside the `ActionMenuPanel`, create:

**A. Title Text:**
- Right-click ActionMenuPanel → UI → Text - TextMeshPro
- Name: `TitleText`
- Text: "Word"
- Font Size: 24
- Alignment: Center

**B. Info Text:**
- Create another TMP Text
- Name: `InfoText`
- Text: "Confidence: 0.00"
- Font Size: 14

**C. Pronunciation Button:**
- Right-click ActionMenuPanel → UI → Button - TextMeshPro
- Name: `PronunciationButton`
- Button text: "🔊 Hear Pronunciation"

**D. Examples Button:**
- Create another Button
- Name: `ExamplesButton`
- Button text: "📝 Example Sentences"

**E. Close Button:**
- Create another Button
- Name: `CloseButton`
- Button text: "✕ Close"

#### Step 4: Add ActionMenuPanel Component

1. **Select ActionMenuPanel GameObject**
2. **Add Component** → `ActionMenuPanel`
3. **In Inspector, assign references:**
   - **Canvas Group:** Drag the CanvasGroup component
   - **Panel Transform:** Drag the ActionMenuPanel's RectTransform
   - **Title Text:** Drag TitleText
   - **Info Text:** Drag InfoText
   - **Pronunciation Button:** Drag PronunciationButton
   - **Examples Button:** Drag ExamplesButton
   - **Close Button:** Drag CloseButton
   - **Canvas:** Drag the main Canvas

---

### Part 3: Example Sentences Setup (10 minutes)

#### Step 1: Create Example Sentences Database

1. **Right-click in Project** → Create → LinguaAR → Example Sentences → Sentences Database
2. **Name it:** `ExampleSentencesDatabase`
3. **Save in:** `Assets/Settings/`

#### Step 2: Add Some Example Sentences

Let's add examples for "chair" to test:

1. **Select ExampleSentencesDatabase**
2. **In Inspector**, expand `Word Examples`
3. **Add element:**
   - **Object Class:** `chair`
   - **Examples → Add element:**
     - **English Sentence:** `Please sit on the chair.`
     - **Translated Sentence:** `Si prega di sedersi sulla sedia.`
     - **Source:** `Manual`
4. **Add another example:**
     - **English Sentence:** `The chair is comfortable.`
     - **Translated Sentence:** `La sedia è comoda.`
     - **Source:** `Manual`

#### Step 3: Create Example Sentences Panel UI

1. **In Canvas**, create another **UI → Panel**
2. **Name it:** `ExampleSentencesPanel`
3. **Configure:**
   - Anchor: Stretch (full screen)
   - Offsets: All 0
   - Add **CanvasGroup** component (Alpha: 0)

4. **Add children:**
   - **Title Text** (TMP): "Examples: Word"
   - **Close Button**: "✕ Close"
   - **Scroll View**: For sentences list
     - Inside Scroll View → Content: This is where sentences appear
   - **No Examples Text** (TMP): "No examples available"

#### Step 4: Create Sentence Item Prefab

1. **Create UI → Panel** (temporary, in Hierarchy)
2. **Name it:** `SentenceItem`
3. **Add two TMP Text children:**
   - `EnglishText`: Font size 16
   - `TranslatedText`: Font size 14, italic
4. **Drag to Project** to make it a prefab
5. **Delete from Hierarchy**

#### Step 5: Add ExampleSentencesPanel Component

1. **Select ExampleSentencesPanel GameObject**
2. **Add Component** → `ExampleSentencesPanel`
3. **Assign references:**
   - **Canvas Group:** Drag CanvasGroup
   - **Title Text:** Drag title text
   - **Sentences Container:** Drag Scroll View → Viewport → Content
   - **Sentence Item Prefab:** Drag the SentenceItem prefab
   - **Close Button:** Drag close button
   - **No Examples Text:** Drag "no examples" text
   - **Sentences Database:** Drag ExampleSentencesDatabase asset

---

## ✅ Testing

### Test in Editor

1. **Press Play**
2. **Click on "chair" detection rectangle**
3. **Action menu should appear** with 3 buttons
4. **Click "🔊 Hear Pronunciation":**
   - Console shows: `[EditorTTS] 🔊 Speaking: 'Sedia'`
5. **Click "📝 Example Sentences":**
   - Panel appears with 2 sentences
   - English and Italian versions shown
6. **Click Close** or ESC to dismiss

### Test on iOS Device

1. **Build to iOS** (File → Build Settings → iOS)
2. **Open in Xcode**
3. **Deploy to device**
4. **Focus on an object**
5. **Tap "Hear Pronunciation"**
6. **Device speaks:** "Sedia" in Italian voice! 🎉

### Test on Android Device

1. **Build to Android** (File → Build Settings → Android)
2. **Install APK**
3. **Same as iOS** - native Android TTS speaks the word

---

## 🎨 Customization

### Change Speech Rate

In `TTSManager` Inspector:
- **Speech Rate:** 0.3 (slower) or 0.8 (faster)

### Auto-Play Pronunciation

In `TTSManager` Inspector:
- **Auto Play On Focus:** ✓
- Now pronunciation plays automatically when you click an object

### Add More Example Sentences

1. Select `ExampleSentencesDatabase`
2. Add new `Word Examples` entries
3. Common words to add: table, lamp, bottle, phone, etc.

---

## 📊 Example Sentences - Bulk Data

You can manually add more sentences or I can provide a pre-populated database. Here's the format:

**Chair:**
- EN: "The chair is made of wood." → IT: "La sedia è fatta di legno."
- EN: "I bought a new chair." → IT: "Ho comprato una nuova sedia."

**Table:**
- EN: "The table is round." → IT: "Il tavolo è rotondo."
- EN: "Put the book on the table." → IT: "Metti il libro sul tavolo."

Want me to create a populator script similar to the translation one?

---

## 🐛 Troubleshooting

### TTS doesn't speak on device

**iOS:**
- Check that device has Italian voice installed (Settings → Accessibility → Spoken Content → Voices)
- Ensure app has audio permissions

**Android:**
- Go to Settings → Accessibility → Text-to-Speech
- Check that TTS engine is installed
- Download language data if needed

### Action menu doesn't appear

- Check that `ObjectSelectionManager` has `DrawRect` reference assigned
- Verify `ActionMenuPanel` is active in Hierarchy (it auto-hides, but must exist)
- Check Console for errors

### Example sentences don't show

- Verify `ExampleSentencesDatabase` has entries for the clicked word
- Check that `ExampleSentencesPanel` has `Sentences Database` assigned
- Ensure sentence item prefab is assigned

---

## 🚀 What's Next?

Now that you have TTS and examples, you could add:

1. **Vocabulary Saving** - Save words to persistent list
2. **Spaced Repetition** - Track learning progress
3. **Offline TTS Caching** - Pre-download audio files
4. **More Example Sentences** - Bulk populate from Tatoeba database
5. **Quiz Mode** - Test vocabulary knowledge

**Want to add any of these?** Let me know!
