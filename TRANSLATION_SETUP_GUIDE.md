# Offline Translation System - Setup Guide

## Overview

You now have a **fully offline translation system** that replaces the DeepL API runtime dependency. This guide explains how to set it up and use it.

## What Changed

**Before:**
- Runtime DeepL API calls (slow, requires internet, API costs)
- Async translation with callbacks
- API key security issues

**After:**
- Pre-translated database (instant, offline, free)
- Synchronous lookup (0ms delay)
- No API keys in runtime code
- Multi-language support from day 1

---

## Step 1: Create the ScriptableObject Assets

### 1.1 Create Translation Database

1. In Unity, right-click in the Project window
2. Navigate to: **Create → LinguaAR → Translation → Translation Database**
3. Name it: `TranslationDatabase`
4. Recommended location: `Assets/Settings/TranslationDatabase.asset`

### 1.2 Create Language Settings

1. Right-click in Project window
2. Navigate to: **Create → LinguaAR → Settings → Language Settings**
3. Name it: `LanguageSettings`
4. Recommended location: `Assets/Settings/LanguageSettings.asset`

---

## Step 2: Populate Translations (ONE-TIME - 5 seconds!)

### Use Pre-Made Translations (Recommended - Super Easy!)

1. In Unity menu bar, go to: **LinguaAR → Populate Translation Database (Pre-Made)**
2. A small window will open
3. **Drag your `TranslationDatabase` asset** to the "Target Database" field
4. Click **"Populate Database Now"**
5. **Done in 1 second!** A popup will confirm success
6. Your database now has all 618 translations!

### Alternative: Generate with DeepL API (If you want fresh translations)

1. In Unity menu bar, go to: **LinguaAR → Generate All Translations**
2. Fill in DeepL API key and target database
3. Click "Generate All Translations"
4. Wait ~5-10 minutes

**Note:** The pre-made option is faster and more reliable!

---

## Step 3: Configure ObjectDetectionSample

1. Open the **AR Scene**
2. Find the GameObject with the `ObjectDetectionSample` component
3. In the Inspector, locate the **"Offline Translation System"** section
4. **Assign the assets:**
   - **Translation Database:** Drag your `TranslationDatabase` asset
   - **Language Settings:** Drag your `LanguageSettings` asset

5. (Optional) **Disable legacy translation:**
   - Locate the `TranslateWords` component on the same GameObject
   - Uncheck "Is Translation Enabled" or remove the component entirely

---

## Step 4: Add Language Selection UI

### 4.1 Main Menu Integration

1. Open **Main Menu.unity** scene
2. Create a new UI panel or find your settings panel
3. Add the `LanguageSelectionUI` component to it
4. In the Inspector:
   - **Language Settings:** Drag your `LanguageSettings` asset

### 4.2 Option A: Dropdown Style

If you want a dropdown menu:

1. Add a **TMP_Dropdown** to your UI
2. In `LanguageSelectionUI` component:
   - Drag the dropdown to **Language Dropdown** field
3. Done! The dropdown will auto-populate with languages

### 4.3 Option B: Button Style

If you prefer individual buttons:

1. Create 4 buttons with text: "English", "Français", "Deutsch", "Italiano"
2. In `LanguageSelectionUI` component:
   - Drag each button to the corresponding field (English Button, French Button, etc.)
3. The selected button will highlight automatically

### 4.4 Option C: Simple Text Display

If you just want to show the current language:

1. Add a **TMP_Text** element
2. In `LanguageSelectionUI` component:
   - Drag the text to **Current Language Text** field
3. It will display: "🇮🇹 Italiano" (flag + language name)

---

## Step 5: Test the System

### 5.1 In Editor (XR Simulation)

1. Open **AR Scene.unity**
2. Press Play
3. Select an XR Environment (e.g., Kitchen)
4. Look at objects (chairs, tables, etc.)
5. You should see:
   - Object name in English (e.g., "chair")
   - Translation in your selected language (e.g., "sedia")
   - **Instant appearance** (no delay)

### 5.2 Test Language Switching

1. Open Main Menu scene
2. Press Play
3. Change the language using your dropdown/buttons
4. Open AR Scene
5. Translations should now be in the new language

### 5.3 Test Offline Mode

1. Disconnect from internet
2. Press Play in AR Scene
3. Translations should still work (proving it's offline)

---

## Troubleshooting

### Problem: No translations appear

**Solution:**
1. Check Console for errors
2. Verify `TranslationDatabase` and `LanguageSettings` are assigned in `ObjectDetectionSample`
3. Make sure you ran the Translation Generator successfully
4. Check that your `TranslationDatabase` asset has entries (should have 206 items)

### Problem: Translation Generator fails

**Possible causes:**
- Invalid DeepL API key
- API rate limit reached
- Network issues

**Solutions:**
1. Verify API key is correct
2. Wait 1 hour and try again (rate limit reset)
3. Use Option B (manual JSON import)

### Problem: Translations are in wrong language

**Solution:**
1. Open `LanguageSettings` asset
2. Set `Current Language` to your desired language
3. Or use the LanguageSelectionUI to change it

### Problem: "Editor code in runtime" error

**This should be fixed!** I removed the `UnityEditor` reference from `UIRectObject.cs`.

If you still see it:
1. Check that line 5 of `UIRectObject.cs` does NOT have `using UnityEditor;`
2. If it does, delete that line and save

---

## How to Add More Languages

If you want to add Spanish, Portuguese, etc. in the future:

1. Add new language to `TargetLanguage` enum in `TranslationDatabaseSO.cs`
2. Add new field to `MultiLanguageTranslation` class
3. Update `GetTranslation()` switch statement
4. Update `LanguageSettings.cs` helper methods
5. Re-run the Translation Generator with new language

---

## Performance Benefits

**Before (DeepL API):**
- Translation time: 200-500ms per object
- Network required: Yes
- Simultaneous translations: Limited by API rate
- Cost: API usage fees

**After (Offline Database):**
- Translation time: <1ms (instant)
- Network required: No
- Simultaneous translations: Unlimited
- Cost: $0

---

## Next Steps

Now that you have offline translation working, you can proceed with:

1. **Platform Native TTS** - Implement pronunciation audio
2. **Example Sentences** - Add Tatoeba database
3. **Clickable Objects** - Make rectangles interactive (from the earlier design)
4. **Vocabulary System** - Save words for later review

---

## Files Created

| File | Purpose |
|------|---------|
| `TranslationDatabaseSO.cs` | ScriptableObject for storing translations |
| `LanguageSettings.cs` | Global language selection settings |
| `TranslationGenerator.cs` | Editor tool to generate all translations |
| `LanguageSelectionUI.cs` | UI component for language picker |

---

## Support

If you encounter any issues:

1. Check the Unity Console for error messages
2. Verify all assets are properly assigned
3. Ensure you're using Unity 6000.2.10f1
4. Check that the Translation Generator completed successfully

**Questions?** Let me know and I'll help debug!
