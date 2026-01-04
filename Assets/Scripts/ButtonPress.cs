using UnityEngine;
using Bhaptics.SDK2;

public class PressableButton : MonoBehaviour
{
    [Header("Button Movement")]
    public float pressDistance = 0.01f;
    public float returnSpeed = 10f;

    [Header("Press Detection")]
    public string[] validTags = { "Finger", "Palm" };

    [Header("Cooldown")]
    [Tooltip("Minimum time between accepted presses (seconds).")]
    public float pressCooldown = 0.5f;

    [Header("Haptics (bHaptics Designer event keys)")]
    public string leftPressEvent  = "button_press_left";   // Device = GloveL
    public string rightPressEvent = "button_press_right";  // Device = GloveR
    [Range(0f,1f)] public float hapticIntensity = 1.0f;
    public float hapticDuration = 0.12f;

    private Vector3 initialPosition;
    private Vector3 pressedPosition;

    private int touchCount = 0;           // how many valid colliders inside
    private bool hasRegisteredPress = false;

    // cooldown timer
    private float nextAllowedPressTime = 0f;

    private void Start()
    {
        initialPosition = transform.localPosition;
        pressedPosition = initialPosition + Vector3.down * pressDistance;
    }

    private void Update()
    {
        // move button toward target pos
        Vector3 targetPosition = touchCount > 0 ? pressedPosition : initialPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * returnSpeed);

        // when fully released, allow next press (subject to cooldown)
        if (touchCount == 0 && hasRegisteredPress)
            hasRegisteredPress = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidPressSource(other)) return;

        touchCount++;

        // only register once per touch, and only if cooldown elapsed
        if (!hasRegisteredPress && touchCount == 1)
        {
            if (Time.time >= nextAllowedPressTime)
            {
                hasRegisteredPress = true;

                // 1) sequence logic you already had
                SequenceButtonUnlocker.Instance?.RegisterPress(gameObject.name);

                // 2) fire haptic on the hand that touched
                PlayPressHaptic(other);

                // arm cooldown
                nextAllowedPressTime = Time.time + pressCooldown;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsValidPressSource(other)) return;
        touchCount = Mathf.Max(0, touchCount - 1);
    }

    private bool IsValidPressSource(Collider other)
    {
        foreach (string tag in validTags)
            if (other.CompareTag(tag)) return true;
        return false;
    }

    // -------- Haptics --------

    enum HandSide { Left, Right, Unknown }

    void PlayPressHaptic(Collider source)
    {
        var side = DetectHandSide(source);
        switch (side)
        {
            case HandSide.Left:
                BhapticsLibrary.PlayParam(leftPressEvent,  hapticIntensity, hapticDuration);
                break;
            case HandSide.Right:
                BhapticsLibrary.PlayParam(rightPressEvent, hapticIntensity, hapticDuration);
                break;
            default:
                // fallback if we can't tell: try the right event
                BhapticsLibrary.PlayParam(rightPressEvent, hapticIntensity, hapticDuration);
                break;
        }
    }

    HandSide DetectHandSide(Collider other)
    {
        // 1) prefer explicit tags on any parent (add them if you want)
        Transform t = other.transform;
        for (int i = 0; i < 6 && t != null; i++, t = t.parent)
        {
            string n = t.name.ToLowerInvariant();
            if (n.StartsWith("l_") || n.Contains("left"))  return HandSide.Left;
            if (n.StartsWith("r_") || n.Contains("right")) return HandSide.Right;
        }

        return HandSide.Unknown;
    }
}
