using System.Collections.Generic;
using Niantic.Lightship.AR.Semantics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem; // New Input System

// This script was originally provided by Niantic and modified by Alive Studios
// https://lightship.dev/docs/ardk/how-to/ar/query_semantics_real_objects/
public class SemanticQuery : MonoBehaviour
{
    [Header("AR")]
    public ARCameraManager _cameraMan;
    public ARSemanticSegmentationManager _semanticMan;
    [SerializeField] private ARRaycastManager _raycastMan; // <-- needed for world placement

    [Header("UI / Visuals")]
    public TMP_Text _text;
    public RawImage _image;
    public Material _material;

    [Header("Spawning")]
    [SerializeField] private Transform spawnObjectParent;
    public List<ChannelToObject> ChannelToObjects;

    [Header("Settings")]
    [Tooltip("How long the pointer must be held before we query semantics (seconds).")]
    public float holdToQuerySeconds = 0.05f;

    private string _channel = "ground";
    private float _holdTimer = 0f;
    private bool _wasPressedLastFrame = false;

    // Reusable list for raycasts
    private static readonly List<ARRaycastHit> _hits = new();

    private void OnEnable()
    {
        if (_image != null) _image.enabled = false;
        if (_cameraMan != null) _cameraMan.frameReceived += CameraManOnframeReceived;
    }

    private void OnDisable()
    {
        if (_cameraMan != null) _cameraMan.frameReceived -= CameraManOnframeReceived;
    }

    private void CameraManOnframeReceived(ARCameraFrameEventArgs args)
    {
        if (_semanticMan == null || _material == null)
            return;

        // Semantics generally only run on device; in Editor they may be unavailable
        if (!_semanticMan.subsystem.running)
            return;

        // Request the current channel texture
        Matrix4x4 semanticMat = Matrix4x4.identity;
        var texture = _semanticMan.GetSemanticChannelTexture(_channel, out semanticMat);

        // Guard against null/invalid textures (prevents D3D12 errors in Editor)
        if (texture == null || texture.width <= 0 || texture.height <= 0)
        {
            if (_image != null) _image.enabled = false;
            return;
        }

        if (_image != null)
        {
            _image.enabled = true;
            _image.material = _material;
            _image.material.SetTexture("_SemanticTex", texture);
            _image.material.SetMatrix("_SemanticMat", semanticMat);
        }
    }

    private void Update()
    {
        if (_semanticMan == null || !_semanticMan.subsystem.running)
            return;

        // --- New Input System: get primary pointer pressed + position ---
        bool isPressed;
        Vector2 screenPos;
        GetPrimaryPointer(out isPressed, out screenPos);

        if (isPressed)
        {
            _holdTimer += Time.deltaTime;

            if (_holdTimer >= holdToQuerySeconds)
            {
                // Query channel names under the pressed screen position
                var list = _semanticMan.GetChannelNamesAt((int)screenPos.x, (int)screenPos.y);

                if (list != null && list.Count > 0)
                {
                    _channel = list[0];
                    if (_text != null) _text.text = _channel;

                    // Attempt to raycast that screen point onto a plane to place content in world space
                    if (_raycastMan != null &&
                        _raycastMan.Raycast(screenPos, _hits, TrackableType.PlaneWithinPolygon))
                    {
                        var pose = _hits[0].pose;

                        foreach (var channelToObject in ChannelToObjects)
                        {
                            if (channelToObject.channel == _channel && channelToObject.GameObject != null)
                            {
                                Debug.Log($"The channel '{_channel}' has been detected and will spawn an object!");
                                var go = Instantiate(channelToObject.GameObject, pose.position, pose.rotation, spawnObjectParent);
                                Destroy(go, 3f);
                            }
                        }
                    }
                }
                else
                {
                    if (_text != null) _text.text = "?";
                }

                // reset to require a new hold before the next query
                _holdTimer = 0f;
            }
        }
        else
        {
            _holdTimer = 0f;
        }

        _wasPressedLastFrame = isPressed;
    }

    /// <summary>
    /// Reads pointer state from the new Input System.
    /// Prefers touch on mobile; falls back to mouse in Editor/desktop.
    /// </summary>
    private void GetPrimaryPointer(out bool isPressed, out Vector2 pos)
    {
        // Default outputs
        isPressed = false;
        pos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        // Touch first (mobile)
        if (Touchscreen.current != null)
        {
            // Use the first active touch that is not ended/canceled
            var touches = Touchscreen.current.touches;
            for (int i = 0; i < touches.Count; i++)
            {
                var t = touches[i];
                var phase = t.phase.ReadValue();
                if (phase == UnityEngine.InputSystem.TouchPhase.Began ||
                    phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    isPressed = true;
                    pos = t.position.ReadValue();
                    return;
                }
            }
        }

        // Mouse fallback (Editor/desktop)
        if (Mouse.current != null)
        {
            isPressed = Mouse.current.leftButton.isPressed;
            pos = Mouse.current.position.ReadValue();
        }
    }
}

[System.Serializable]
public struct ChannelToObject
{
    public string channel;
    public GameObject GameObject;
}
