# Clickable Object Selection - Setup Guide

## What I Built

✅ **Click-to-Select System:**
- Detected objects initially show **only category name** (e.g., "chair: 0.87")
- **Click/tap a rectangle** → shows translation and highlights it
- **Click again or press ESC** → unfocus and resume normal detection
- Focused object gets **cyan border + larger scale** for visibility

---

## Setup Instructions (5 minutes)

### Step 1: Verify EventSystem Exists

The AR Scene needs an **EventSystem** for click detection to work.

1. **Open AR Scene.unity**
2. **Check if EventSystem exists:**
   - Look in Hierarchy for "EventSystem" GameObject
   - If it exists → great, skip to Step 2
   - If it doesn't exist → Create one:

**To Create EventSystem:**
1. Right-click in Hierarchy → **UI → Event System**
2. This creates a GameObject called "EventSystem"
3. It should have two components:
   - EventSystem
   - Standalone Input Module (or Input System UI Input Module)
4. Done! Leave it as-is

---

### Step 2: Add ObjectSelectionManager to Scene

1. **In Hierarchy**, find or create an empty GameObject
   - Name it: `ObjectSelectionManager`
2. **Add Component** → `ObjectSelectionManager`
3. **In Inspector**, configure:
   - **Pause Detection When Focused:** ✓ (checked)
   - **Detection Controller:** Drag the GameObject that has `ObjectDetectionSample` component
   - **Draw Rect:** Drag the GameObject that has `DrawRect` component (usually same object as Detection Controller)

---

### Step 3: Verify Canvas Has GraphicRaycaster

Your UI rectangles need GraphicRaycaster to receive clicks.

1. **Find the Canvas** in Hierarchy (should be where your rectangles are drawn)
2. **Check components:**
   - Canvas ✓
   - CanvasScaler ✓
   - **GraphicRaycaster** ← Make sure this exists!

3. **If GraphicRaycaster is missing:**
   - Select the Canvas GameObject
   - Click **Add Component**
   - Search for "GraphicRaycaster"
   - Add it

---

### Step 4: Test It!

1. **Press Play** in AR Scene
2. **Select an XR Environment** (Kitchen, Living Room, etc.)
3. **Look for detectable objects** (chairs, tables, bottles)
4. **You should see:**
   - Rectangles showing: `"chair: 0.87"` (category only)
   - **NO translation visible yet** ← This is correct!

5. **Click on a rectangle:**
   - ✅ Rectangle turns **cyan blue** and grows slightly (1.15x scale)
   - ✅ Translation appears below: `"chair: 0.87\nSedia"` ← Italian translation shows!
   - ✅ **All other rectangles disappear** (only focused one visible)
   - ✅ Detection is paused (UI freezes)
   - ✅ Console logs: `[ObjectSelectionManager] Focused on: chair (Sedia)`

6. **Click again or press ESC:**
   - ✅ Rectangle returns to normal
   - ✅ Detection resumes
   - ✅ Console logs: `[ObjectSelectionManager] Unfocused object`

---

## How It Works

### Normal State (Before Click)
```
┌────────────────────┐
│  chair: 0.87       │ ← White/colored border
└────────────────────┘
```

### Focused State (After Click)
```
╔══════════════════════╗
║  chair: 0.87         ║ ← Cyan border (highlighted)
║  Sedia               ║ ← Translation appears!
╚══════════════════════╝
   (1.15x scale)
```

---

## Keyboard Controls

- **Left Click** on rectangle → Focus object
- **Left Click** again → Unfocus
- **ESC** → Unfocus current object
- **Right Click** → Unfocus current object

---

## Troubleshooting

### Problem: Clicking does nothing

**Solutions:**
1. **Check EventSystem exists** in Hierarchy
2. **Check Canvas has GraphicRaycaster** component
3. **Check Console** for `[UIRectObject] Clicked: ...` message
   - If you see this → click is working, focus logic might be broken
   - If you don't see this → click detection is broken

4. **Verify UIRectObject prefab** has Image component with:
   - `Raycast Target` ✓ (checked)

### Problem: Translation doesn't appear on click

**Solutions:**
1. **Check ObjectSelectionManager** is in scene and active
2. **Verify _detectionController** is assigned in Inspector
3. **Check Console** for errors

### Problem: Can't unfocus objects

**Solutions:**
1. Press **ESC** or **Right Click**
2. **Click the same rectangle again** (toggles focus)

### Problem: Other rectangles disappear when I click one

**This is correct behavior!** When you focus on one object:
- All OTHER rectangles are hidden
- Only the focused rectangle remains visible (cyan, larger, with translation)
- Detection pauses (so UI doesn't update)
- Click the focused rectangle again or press ESC to resume normal mode

---

## Customization

### Change Focused Color

Edit `UIRectObject.cs` line 38:
```csharp
private Color _focusedColor = new Color(0f, 0.8f, 1f, 0.9f); // Cyan
```

Try:
- Gold: `new Color(1f, 0.84f, 0f, 0.9f)`
- Green: `new Color(0.2f, 1f, 0.2f, 0.9f)`
- Pink: `new Color(1f, 0.4f, 0.8f, 0.9f)`

### Change Focused Scale

Edit `UIRectObject.cs` line 40:
```csharp
private Vector3 _focusedScale = Vector3.one * 1.15f; // 15% larger
```

Try: `* 1.25f` for 25% larger

### Disable Detection Pause

In ObjectSelectionManager Inspector:
- Uncheck **"Pause Detection When Focused"**
- Now detection continues even when focused (translation still shows)

---

## What's Next?

Now that you have clickable selection, you can add:

1. **Vocabulary Saving** - "Save Word" button when focused
2. **Pronunciation Audio** - Play TTS when clicking
3. **Example Sentences** - Show usage examples
4. **Action Menu** - Popup with multiple options

Let me know what you want to add next!
