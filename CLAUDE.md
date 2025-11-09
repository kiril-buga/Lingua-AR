# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Lingua AR** is an AR/VR language learning application built with Unity 6000.2.10f1. The app uses real-time object detection and semantic segmentation to help users learn vocabulary by detecting real-world objects and displaying translations.

**Core Technologies:**
- Niantic Lightship ARDK (advanced AR features)
- Unity ARFoundation (cross-platform AR)
- Universal Render Pipeline (URP)
- DeepL API (translation)

**Platforms:**
- iOS (ARKit + Lightship)
- Android (ARCore + Lightship)
- VR (OpenXR)
- Editor (XR Simulation)

## Development Workflow

### Opening the Project
- Requires Unity 6000.2.10f1
- Open via Unity Hub or `Lingua AR.sln`
- Packages auto-resolve from `Packages/manifest.json`

### Testing in Editor
1. Open `Assets/Scenes/AR Scene.unity`
2. XR Simulation activates automatically in Play mode
3. Select simulation environment in Window → XR → AR Foundation → XR Environment
4. Available environments: Backyard, Bedroom, Kitchen, LivingRoom, Office, etc. (12+ rooms in `Assets/UnityXRContent/ARFoundation/SimulationEnvironments/`)
5. Use mouse/keyboard to navigate simulated AR camera

### Building for iOS
1. File → Build Settings → iOS
2. Switch Platform (requires iOS module installed)
3. Click Build (outputs to `Builds/IOS/`)
4. Open generated `Unity-iPhone.xcodeproj` in Xcode
5. Configure signing and provisioning profile
6. Build and deploy to physical device (ARKit requires real hardware)

### Building for Android
1. File → Build Settings → Android
2. Switch Platform (requires Android module)
3. Build APK or Android App Bundle
4. Install on ARCore-compatible device

**Note:** No automated build scripts exist. All builds are manual through Unity Editor.

## Architecture Overview

### Object Detection Pipeline

The core AR feature follows this flow:

```
ARObjectDetectionManager (Niantic Lightship)
        ↓
ObjectDetectionSample.cs (Main Controller)
├── Filters detections by confidence (0.6 threshold)
├── Processes detection metadata
└── Emits events to downstream systems
        ↓
    ┌───────┴────────┬──────────────────┬─────────────────┐
    ↓                ↓                  ↓                 ↓
DrawRect.cs    TranslateWords.cs  Event Emission   SpawnObjectsAroundObjectDetected.cs
(UI Overlay)   (DeepL API)        (subscribers)    (3D object placement)
```

**Key Components:**

- **ObjectDetectionSample.cs** - Central orchestrator that manages ARObjectDetectionManager, filters by confidence and valid channels, coordinates UI/translation/spawning
- **DrawRect.cs** - UI visualization using object pooling pattern for detection rectangles
- **TranslateWords.cs** - DeepL API integration for real-time translation (toggle-able)
- **SpawnObjectsAroundObjectDetected.cs** - Singleton that spawns 3D objects at detected positions, rate-limited to 1 object/second
- **Depth_ScreenToWorldPosition.cs** - Converts 2D screen coordinates to 3D world positions using Lightship depth data

### Event-Driven Architecture

Systems communicate via events to maintain decoupling:

```csharp
// Central event in ObjectDetectionSample.cs
public static event Action<(string category, Vector2 rectPosition)> OnFoundItemAtPosition;
```

Multiple systems subscribe to detection events independently. Always unsubscribe in `OnDestroy()` to prevent memory leaks.

### ScriptableObject Configuration System

Object spawning is data-driven via ScriptableObjects:

```
ListSpawnObjectToObjectClassSO (container)
└── List<SpawnObjectToObjectClassSO>
    ├── detectionClass (e.g., "chair", "lamp")
    ├── objectToSpawn (GameObject prefab)
    ├── maxSpawn (spawn limit)
    └── objectName (display name)
```

Configure mappings in Unity Inspector without code changes. Located in `Assets/Scripts/SkriptableObjectsObjectDetected/`.

### XR Multi-Platform System

Different XR loaders for each platform (configured in `Assets/XR/Settings/`):

- **LightshipSimulationLoader** - Editor testing with full Lightship features
- **LightshipARKitLoader** - iOS with Lightship capabilities
- **LightshipARCoreLoader** - Android with Lightship capabilities
- **OpenXRLoader** - VR headset support

Lightship provides: depth estimation, meshing, semantic segmentation, object detection, persistent anchors.

## Critical Code Patterns

### Platform-Specific Depth Handling

iOS and Android require different matrix transformations:

```csharp
// In Depth_ScreenToWorldPosition.cs
#if UNITY_IOS
    _displayMatrix = args.displayMatrix.Value.transpose;
#else
    _displayMatrix = args.displayMatrix.Value;
#endif
```

Always test depth/world positioning on both platforms when modifying spatial code.

### Manager Lifecycle Pattern

AR managers must be properly initialized and cleaned up:

```csharp
void Start() {
    _manager.enabled = true;
    _manager.MetadataInitialized += OnMetadataInitialized;
}

void OnDestroy() {
    _manager.MetadataInitialized -= OnMetadataInitialized;
}
```

### Object Pooling

`DrawRect.cs` uses pooling to reduce GC pressure:
- Preallocates UI rectangles
- Reuses GameObjects via index tracking
- Avoids runtime instantiation during AR sessions

Apply this pattern when creating frequently spawned/destroyed objects.

### Singleton Pattern

`SpawnObjectsAroundObjectDetected` uses singleton for global access:

```csharp
public static SpawnObjectsAroundObjectDetected Instance;

void Awake() {
    Instance = this;
}
```

Access via `SpawnObjectsAroundObjectDetected.Instance` from any script.

## External API Integrations

### DeepL Translation API

- **Endpoint:** `https://api-free.deepl.com/v2/translate`
- **Location:** `Assets/Scripts/ObjectDetection/TranslateWords.cs`
- **Current Implementation:** API key is hardcoded (security risk)
- **Target Language:** Default is Italian ("IT"), configurable per instance
- **Usage:** Translates detected object names on-demand

**Security Note:** API key should be moved to environment variables or secure configuration system, not committed to version control.

### Niantic Lightship API

- **Configuration:** `Assets/Settings/LightshipSettings.asset`
- **API Key:** Stored in asset file
- **Features Used:** Object detection model, depth estimation, semantic segmentation
- **Documentation:** Code samples adapted from Niantic docs (see attribution comments in `SemanticQuery.cs`)

## Project Structure

```
Assets/
├── Scenes/
│   ├── Main Menu.unity       - Entry point with scene selection
│   ├── AR Scene.unity        - Main AR experience with detection
│   └── VR Scene.unity        - VR learning mode
├── Scripts/
│   ├── ObjectDetection/      - Core AR detection logic
│   ├── SkriptableObjectsObjectDetected/ - Spawn configuration SOs
│   └── MainMenu/             - Menu navigation
├── Prefabs/                  - Reusable objects (UI, spawnable items)
├── UnityXRContent/ARFoundation/SimulationEnvironments/ - Test environments
├── XR/Settings/              - XR loader configurations per platform
└── Settings/                 - URP and rendering settings
```

## Known Issues & Constraints

### Code Quality Concerns

1. **No Namespaces:** All scripts in global namespace (potential naming conflicts)
2. **No Assembly Definitions:** Monolithic compilation (slower build times)
3. **Editor Code in Runtime:** `UIRectObject.cs` uses `UnityEditor` namespace (breaks builds)
4. **Naming Typo:** `SkriptableObjectsObjectDetected` folder misspelled

### Security Issues

- DeepL API key hardcoded in `TranslateWords.cs:17`
- Lightship API key in version-controlled asset file
- No environment variable system or secure config management

### Missing Infrastructure

- No unit tests or test assembly
- No CI/CD configuration
- No automated build scripts
- No custom editor tools for workflow automation

## Important File Locations

- Object detection controller: `Assets/Scripts/ObjectDetection/ObjectDetectionSample.cs`
- Depth/world positioning: `Assets/Scripts/ObjectDetection/Depth_ScreenToWorldPosition.cs`
- Translation system: `Assets/Scripts/ObjectDetection/TranslateWords.cs`
- UI visualization: `Assets/Scripts/ObjectDetection/DrawRect.cs`
- 3D spawning: `Assets/Scripts/ObjectDetection/SpawnObjectsAroundObjectDetected.cs`
- Semantic segmentation: `Assets/Scripts/ObjectDetection/SemanticQuery.cs`
- Spawn configuration: `Assets/Scripts/SkriptableObjectsObjectDetected/`
- Custom shader: `Assets/Shaders/SemanticShader.shader`
- Input actions: `Assets/InputSystem_Actions.inputactions`

## Attribution

Code samples adapted from Niantic Lightship documentation:
- `SemanticQuery.cs` - Modified from Niantic sample
- `SemanticShader.shader` - Based on Niantic shader example

Original sources cited in file comments.
