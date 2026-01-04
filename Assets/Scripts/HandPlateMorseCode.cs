using UnityEngine;
using Bhaptics.SDK2;

public class HapticTriggerOnTouch : MonoBehaviour
{
    [Header("Haptic Event")]
    public string hapticEventName = "morse_code"; // must match .tact event name
    public float intensity = 1.0f;
    public float duration = 1.0f;
    public float angleX = 0.0f;
    public float offsetY = 0.5f;

    private int requestId = -1;
    private bool isPlaying = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MorseCode") && !isPlaying)
        {
            Debug.Log($"[bHaptics] Playing haptic: {hapticEventName}");

            requestId = BhapticsLibrary.PlayParam(
                hapticEventName,
                intensity,
                duration,
                angleX,
                offsetY
            );

            isPlaying = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MorseCode") && isPlaying)
        {
            Debug.Log($"[bHaptics] Stopping haptic ID: {requestId}");
            BhapticsLibrary.StopInt(requestId);
            isPlaying = false;
            requestId = -1;
        }
    }
}
