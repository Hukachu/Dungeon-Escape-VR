using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(BoxCollider))] // trigger for hand overlap
public class XRUIBraileSequence_BlockHands : MonoBehaviour
{
    [Header("Sequence")]
    public string buttonIdOverride = "";

    [Header("Hand blocking")]
    public string[] blockTags = { "Finger", "Palm" };
    public string[] blockNameContains = {
        "L_IndexTip","R_IndexTip","L_MiddleTip","R_MiddleTip",
        "L_RingTip","R_RingTip","L_PinkyTip","R_PinkyTip",
        "L_ThumbTip","R_ThumbTip","L_Palm","R_Palm"
    };

    [Header("Move this mesh on press (no extra script needed)")]
    public Transform moveTarget;          // ← assign your VisualMesh here
    public float pressDistance = 0.01f;   // meters down (local Y)
    public float moveSpeed = 12f;

    [Header("Auto-size collider to UI rect")]
    public bool autoSizeColliderFromRect = true;
    public float colliderDepth = 0.02f;

    Button _btn;
    BoxCollider _box;
    Vector3 _restPos, _pressedPos;
    int _blockerCount;

    void Reset()
    {
        var bc = GetComponent<BoxCollider>();
        bc.isTrigger = true;
    }

    void Awake()
    {
        _btn = GetComponent<Button>();
        _box = GetComponent<BoxCollider>();
        _box.isTrigger = true;
    }

    void Start()
    {
        if (!moveTarget) moveTarget = transform; // fallback, but you should assign VisualMesh
        _restPos    = moveTarget.localPosition;
        _pressedPos = _restPos + Vector3.down * pressDistance;

        SetInteractable(true);
    }

    void LateUpdate()
    {
        if (autoSizeColliderFromRect)
        {
            var rt = transform as RectTransform;
            if (rt)
            {
                _box.size   = new Vector3(rt.rect.width, rt.rect.height, colliderDepth);
                _box.center = Vector3.zero;
            }
        }
    }

    // Hook this in the Button's OnClick()
    public void CallSequence()
    {
        if (_blockerCount > 0 || (_btn && !_btn.interactable)) return;

        string id = string.IsNullOrEmpty(buttonIdOverride) ? gameObject.name : buttonIdOverride;
        SequenceBraileUnlocker.Instance?.RegisterBrailePress(id);

        StopAllCoroutines();
        StartCoroutine(CoPressVisual());
    }

    IEnumerator CoPressVisual()
    {
        // down
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            moveTarget.localPosition = Vector3.Lerp(_restPos, _pressedPos, t);
            yield return null;
        }
        // up
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            moveTarget.localPosition = Vector3.Lerp(_pressedPos, _restPos, t);
            yield return null;
        }
        moveTarget.localPosition = _restPos;
    }

    // --- hand overlap blocking ---
    void OnTriggerEnter(Collider other)
    {
        if (IsBlockingCollider(other))
        {
            _blockerCount++;
            if (_blockerCount == 1) SetInteractable(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsBlockingCollider(other))
        {
            _blockerCount = Mathf.Max(0, _blockerCount - 1);
            if (_blockerCount == 0) SetInteractable(true);
        }
    }

    bool IsBlockingCollider(Collider col)
    {
        bool tagOk = false;
        foreach (var tag in blockTags)
            if (!string.IsNullOrEmpty(tag) && col.CompareTag(tag)) { tagOk = true; break; }
        if (!tagOk) return false;

        if (blockNameContains != null && blockNameContains.Length > 0)
        {
            string n = col.name;
            foreach (var sub in blockNameContains)
                if (!string.IsNullOrEmpty(sub) &&
                    n.IndexOf(sub, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }
        return true;
    }

    void SetInteractable(bool on)
    {
        if (_btn) _btn.interactable = on;
    }
}
