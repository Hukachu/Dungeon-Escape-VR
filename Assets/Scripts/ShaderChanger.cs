using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ControllerActivityMaterialSwap : MonoBehaviour
{
    [Header("Target")]
    public Renderer targetRenderer;        // auto-fills from this GameObject if left null
    [Min(0)] public int materialIndex = 0; // which sub-material to swap (0 = first)

    [Header("Materials")]
    [Tooltip("Material to use while the controllers are active.")]
    public Material activeMaterial;        // assign a separate Material asset in the Inspector

    [Header("Revert timing")]
    [Tooltip("Revert after this many seconds without any controller input/motion.")]
    public float inactivityTimeout = 1.5f;

    [Header("Which controllers to monitor")]
    public bool monitorLeft  = true;
    public bool monitorRight = true;

    [Header("Activity thresholds (inputs)")]
    public float axisThreshold  = 0.02f;   // trigger/grip
    public float stickThreshold = 0.01f;   // thumbstick magnitude

    [Header("Activity thresholds (motion)")]
    public float linearVelThreshold     = 0.05f; // m/s
    public float angularVelDegThreshold = 5f;    // deg/s
    public float positionDeltaThreshold = 0.005f; // m (fallback if no velocity)
    public float angleDeltaThreshold    = 2f;     // deg (fallback)

    [Header("Ignore hand-tracking")]
    [Tooltip("If true, only devices with the Controller characteristic are considered. Hand-tracking won’t trigger activity.")]
    public bool ignoreHandTracking = true;

    // --- internals ---
    InputDevice _left, _right;
    float _lastActiveTime = -999f;
    bool _activeApplied;

    Material _originalMat;

    // pose history (for fallback motion)
    Vector3 _leftLastPos,  _rightLastPos;
    Quaternion _leftLastRot = Quaternion.identity, _rightLastRot = Quaternion.identity;
    bool _haveLeftPose, _haveRightPose;

    void Reset() { targetRenderer = GetComponent<Renderer>(); }

    void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        if (!targetRenderer) { Debug.LogError($"{name}: No Renderer found."); enabled = false; return; }

        var mats = targetRenderer.materials; // instanced copy
        if (materialIndex < 0 || materialIndex >= mats.Length) {
            Debug.LogError($"{name}: materialIndex {materialIndex} out of range (has " + mats.Length + ").");
            enabled = false; return;
        }
        _originalMat = mats[materialIndex];

        ResolveDevices();
        InputDevices.deviceConnected    += OnDeviceChanged;
        InputDevices.deviceDisconnected += OnDeviceChanged;

        SeedPose(_left,  ref _leftLastPos,  ref _leftLastRot,  ref _haveLeftPose);
        SeedPose(_right, ref _rightLastPos, ref _rightLastRot, ref _haveRightPose);
    }

    void OnDestroy()
    {
        InputDevices.deviceConnected    -= OnDeviceChanged;
        InputDevices.deviceDisconnected -= OnDeviceChanged;
    }

    void OnDeviceChanged(InputDevice _) {
        ResolveDevices();
        SeedPose(_left,  ref _leftLastPos,  ref _leftLastRot,  ref _haveLeftPose);
        SeedPose(_right, ref _rightLastPos, ref _rightLastRot, ref _haveRightPose);
    }

    void ResolveDevices()
    {
        _left  = monitorLeft  ? GetControllerDevice(InputDeviceCharacteristics.Left)  : default;
        _right = monitorRight ? GetControllerDevice(InputDeviceCharacteristics.Right) : default;
    }

    InputDevice GetControllerDevice(InputDeviceCharacteristics handed)
    {
        var list = new List<InputDevice>();
        var mask = InputDeviceCharacteristics.Controller | handed;
        InputDevices.GetDevicesWithCharacteristics(mask, list);
        // pick the first valid controller (ignore pure hand-tracking devices)
        foreach (var d in list)
        {
            // extra guard: skip if it’s flagged as HandTracking
            if (ignoreHandTracking && (d.characteristics & InputDeviceCharacteristics.HandTracking) != 0)
                continue;
            return d;
        }
        return default; // none found
    }

    void Update()
    {
        bool active =
            (monitorLeft  && IsActive(_left,  ref _leftLastPos,  ref _leftLastRot,  ref _haveLeftPose)) ||
            (monitorRight && IsActive(_right, ref _rightLastPos, ref _rightLastRot, ref _haveRightPose));

        if (active)
        {
            _lastActiveTime = Time.time;
            if (!_activeApplied) ApplyActive();
        }
        else if (_activeApplied && Time.time - _lastActiveTime >= inactivityTimeout)
        {
            RevertOriginal();
        }
    }

    bool IsActive(InputDevice dev, ref Vector3 lastPos, ref Quaternion lastRot, ref bool havePose)
    {
        if (!dev.isValid) return false;

        // If somehow we got a non-controller device, ignore it.
        if ((dev.characteristics & InputDeviceCharacteristics.Controller) == 0)
            return false;
        if (ignoreHandTracking && (dev.characteristics & InputDeviceCharacteristics.HandTracking) != 0)
            return false;

        // --- inputs ---
        bool b;
        if (dev.TryGetFeatureValue(CommonUsages.primaryButton, out b) && b) return true;
        if (dev.TryGetFeatureValue(CommonUsages.secondaryButton, out b) && b) return true;
        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out b) && b) return true;
        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out b) && b) return true;
        if (dev.TryGetFeatureValue(CommonUsages.menuButton, out b) && b) return true;

        float f;
        if (dev.TryGetFeatureValue(CommonUsages.trigger, out f) && f > axisThreshold) return true;
        if (dev.TryGetFeatureValue(CommonUsages.grip,    out f) && f > axisThreshold) return true;

        Vector2 v2;
        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out v2) && v2.sqrMagnitude > stickThreshold * stickThreshold)
            return true;

        // --- motion (controllers) ---
        Vector3 v, w;
        if (dev.TryGetFeatureValue(CommonUsages.deviceVelocity, out v) && v.magnitude >= linearVelThreshold)
            return true;

        if (dev.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out w))
        {
            float angDegPerSec = w.magnitude * Mathf.Rad2Deg;
            if (angDegPerSec >= angularVelDegThreshold) return true;
        }

        // fallback: pos/rot deltas
        Vector3 pos; Quaternion rot;
        bool havePos = dev.TryGetFeatureValue(CommonUsages.devicePosition, out pos);
        bool haveRot = dev.TryGetFeatureValue(CommonUsages.deviceRotation, out rot);

        if (havePos && havePose && Vector3.Distance(pos, lastPos) >= positionDeltaThreshold)
            return true;

        if (haveRot && havePose && Quaternion.Angle(rot, lastRot) >= angleDeltaThreshold)
            return true;

        if (havePos) lastPos = pos;
        if (haveRot) lastRot = rot;
        if (havePos || haveRot) havePose = true;

        return false;
    }

    void SeedPose(InputDevice dev, ref Vector3 lastPos, ref Quaternion lastRot, ref bool havePose)
    {
        havePose = false;
        if (!dev.isValid) return;
        Vector3 pos; Quaternion rot; bool got = false;
        if (dev.TryGetFeatureValue(CommonUsages.devicePosition, out pos)) { lastPos = pos; got = true; }
        if (dev.TryGetFeatureValue(CommonUsages.deviceRotation, out rot)) { lastRot = rot; got = true; }
        havePose = got;
    }

    void ApplyActive()
    {
        if (!activeMaterial) return;
        var mats = targetRenderer.materials;
        mats[materialIndex] = activeMaterial;
        targetRenderer.materials = mats;
        _activeApplied = true;
    }

    void RevertOriginal()
    {
        var mats = targetRenderer.materials;
        mats[materialIndex] = _originalMat;
        targetRenderer.materials = mats;
        _activeApplied = false;
    }

    // optional
    public void ForceActive() => ApplyActive();
    public void ForceRevert() => RevertOriginal();
}
