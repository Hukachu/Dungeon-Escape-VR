using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PedestalSlotTags : MonoBehaviour
{
    [Header("Match by Gem Tag (e.g., \"Red\", \"Blue\", \"Green\")")]
    public string requiredGemTag = "Red";

    [Header("Debug (read-only)")]
    public bool isCorrect;   // true when the correct gem is inside

    // Expose for manager logs
    public Transform Occupant => _occupantRoot;
    public string OccupantTag => _occupantRoot ? _occupantRoot.tag : "(none)";

    // Track the current gem occupying this pedestal (root transform) and how many of its colliders are inside
    Transform _occupantRoot;
    int _occupantColliderCount;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; // pedestal must be a trigger volume
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(requiredGemTag)) return;

        var gemRoot = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform.root;

        if (_occupantRoot == null || gemRoot == _occupantRoot)
        {
            bool wasEmpty = (_occupantColliderCount == 0);
            _occupantRoot = gemRoot;
            _occupantColliderCount++;

            isCorrect = true;

            if (wasEmpty)
                Debug.Log($"[Pedestal] {name}: ACCEPTED gem '{_occupantRoot.name}' (tag {requiredGemTag}). isCorrect=TRUE");
            else
                Debug.Log($"[Pedestal] {name}: additional collider entered for '{_occupantRoot.name}'. count={_occupantColliderCount}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (_occupantRoot == null) return;

        var gemRoot = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform.root;
        if (gemRoot != _occupantRoot) return;

        _occupantColliderCount = Mathf.Max(0, _occupantColliderCount - 1);

        if (_occupantColliderCount == 0)
        {
            Debug.Log($"[Pedestal] {name}: REMOVED gem '{_occupantRoot.name}'. isCorrect=FALSE");
            _occupantRoot = null;
            isCorrect = false;
        }
        else
        {
            Debug.Log($"[Pedestal] {name}: collider exited for '{gemRoot.name}'. remaining count={_occupantColliderCount}");
        }
    }

    // If a gem starts already inside the trigger (scene load), this will catch it once
    void OnTriggerStay(Collider other)
    {
        if (_occupantRoot != null || !other.CompareTag(requiredGemTag)) return;

        var gemRoot = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform.root;
        _occupantRoot = gemRoot;
        _occupantColliderCount = 1;
        isCorrect = true;

        Debug.Log($"[Pedestal] {name}: INITIALIZED from Stay with gem '{_occupantRoot.name}' (tag {requiredGemTag}). isCorrect=TRUE");
    }
}
