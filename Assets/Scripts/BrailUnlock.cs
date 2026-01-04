using System.Collections.Generic;
using UnityEngine;

public class SequenceBraileUnlocker : MonoBehaviour
{
    public static SequenceBraileUnlocker Instance;

    [Header("Button Sequence Settings")]
    public List<string> correctSequence = new List<string> { "W", "O", "R", "D" };

    [Header("Gem Settings")]
    public Rigidbody gemRigidbody;

    private readonly List<string> currentSequence = new List<string>();
    private bool sequenceCompleted = false;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegisterBrailePress(string buttonName)
    {
        if (sequenceCompleted) return;

        string key = buttonName;

        int nextIndex = currentSequence.Count;

        // CASE 1: correct next input
        if (key == correctSequence[nextIndex])
        {
            currentSequence.Add(key);
            Debug.Log($"[Sequence] + {key}  -> progress {currentSequence.Count}/{correctSequence.Count}");

            if (currentSequence.Count == correctSequence.Count)
            {
                sequenceCompleted = true;
                UnlockGem();
            }
            return;
        }


        ResetSequenceManual();
    }

    private void UnlockGem()
    {
        Debug.Log("[Sequence] Correct sequence entered! Unlocking gem...");
        if (!gemRigidbody) return;

        // Unfreeze all constraints (or just what you need)
        gemRigidbody.constraints = RigidbodyConstraints.None;
    }

    // Optional: manual reset you can call from elsewhere
    public void ResetSequenceManual()
    {
        sequenceCompleted = false;
        currentSequence.Clear();
        Debug.Log("[Sequence] Manually reset.");
    }
}
