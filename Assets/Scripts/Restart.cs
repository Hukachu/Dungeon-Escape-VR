using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnGemTouch : MonoBehaviour
{
    [Header("Tags that count as 'gem'")]
    public string[] gemTags = { "Pink", "Red", "Blue" };

    [Tooltip("If true, this collider should be a Trigger. If false, use OnCollisionEnter instead.")]
    public bool useTrigger = true;

    private HashSet<string> _tagSet;
    private bool _reloading = false;

    void Awake()
    {
        _tagSet = new HashSet<string>(gemTags);
        if (useTrigger && TryGetComponent<Collider>(out var c) && !c.isTrigger)
            Debug.LogWarning("[RestartOnGemTouch] Collider is not set as Trigger but useTrigger=true.");
        if (!useTrigger && TryGetComponent<Collider>(out var c2) && c2.isTrigger)
            Debug.LogWarning("[RestartOnGemTouch] Collider is Trigger but useTrigger=false (collision will not fire).");
    }

    // Trigger path
    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        TryRestartFor(other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform.root);
    }

    // Collision path (if you prefer non-trigger collider)
    void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return;
        var t = collision.rigidbody ? collision.rigidbody.transform : collision.transform.root;
        TryRestartFor(t);
    }

    void TryRestartFor(Transform root)
    {
        if (_reloading || root == null) return;
        // Compare the ROOT's tag (child colliders may be untagged)
        if (_tagSet.Contains(root.tag))
        {
            _reloading = true;
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
