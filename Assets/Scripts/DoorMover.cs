using UnityEngine;

public class DoorMover : MonoBehaviour
{
    public Transform door;          // assign your door root/mesh
    public float openDistance = 2f; // moves DOWN by this many meters
    public float openSpeed = 1.5f;  // m/s

    Vector3 _closed;
    Vector3 _open;
    bool _opening;
    bool _loggedOpened;

    void Awake()
    {
        if (!door) door = transform;
        _closed = door.localPosition;
        _open   = _closed + Vector3.down * openDistance;

        Debug.Log($"[Door] {name}: Setup complete. closed={_closed}, openTarget={_open}, speed={openSpeed}");
    }

    void Update()
    {
        if (_opening)
        {
            var before = door.localPosition;
            door.localPosition = Vector3.MoveTowards(before, _open, openSpeed * Time.deltaTime);

            if (!_loggedOpened && Vector3.SqrMagnitude(door.localPosition - _open) < 1e-6f)
            {
                _loggedOpened = true;
                Debug.Log($"[Door] {name}: Fully OPEN at {door.localPosition}");
            }
        }
    }

    public void Open()
    {
        if (_opening) return;
        _opening = true;
        Debug.Log($"[Door] {name}: Opening… from {door.localPosition} to {_open}");
    }
}
