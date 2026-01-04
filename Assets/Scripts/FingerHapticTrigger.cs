using UnityEngine;
using Bhaptics.SDK2;

public class FingerHapticTrigger : MonoBehaviour
{
    public enum HandSide { Left, Right }
    [Header("Which hand is this object?")]
    public HandSide side = HandSide.Right;

    [Header("Haptic Events (from Designer)")]
    public string leftEventName  = "drag_block_left";   // Device = GloveL
    public string rightEventName = "drag_block_right";  // Device = GloveR
    [Range(0f,1f)] public float intensity = 1f;
    public float singlePulseDuration = 0.2f;

    [Header("Triggering")]
    public string targetTag = "MovableBlock";
    public float interval = 0.1f;

    bool touching; float t;

    void Update()
    {
        if (!touching) return;
        t += Time.deltaTime;
        if (t >= interval)
        {
            string key = (side == HandSide.Left) ? leftEventName : rightEventName;
            BhapticsLibrary.PlayParam(key, intensity, singlePulseDuration);
            t = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag)) { touching = true; t = interval; }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag)) touching = false;
    }
}