using UnityEngine;
using Bhaptics.SDK2;

public class BrailFingerHaptic : MonoBehaviour
{
    [Tooltip("List of valid finger names matching pattern keys in your .tact file (e.g., L_IndexTip, R_ThumbTip)")]
    public string[] validFingerNames = { "L_IndexTip", "R_IndexTip"};

    private void OnTriggerEnter(Collider other)
    {
        string name = other.gameObject.name;

        // Check if the name matches any known finger
        foreach (var finger in validFingerNames)
        {
            if (name == finger)
            {
                BhapticsLibrary.Play(finger.ToLower());  // The key inside your .tact file
                Debug.Log($"[bHaptics] Played haptic for: {finger}");
                break;
            }
        }
    }
}
