using UnityEngine;

public class GemPedestalPuzzle : MonoBehaviour
{
    public PedestalSlotTags[] pedestals; // assign your 3 pedestals
    public DoorMover doorMover;

    bool _opened;
    bool[] _lastCorrect;

    void Start()
    {
        if (pedestals == null || pedestals.Length == 0)
        {
            Debug.LogWarning("[Puzzle] No pedestals assigned.");
            return;
        }

        _lastCorrect = new bool[pedestals.Length];

        Debug.Log($"[Puzzle] Initialized with {pedestals.Length} pedestals:");
        for (int i = 0; i < pedestals.Length; i++)
        {
            var p = pedestals[i];
            if (!p)
            {
                Debug.LogWarning($"[Puzzle]  - Pedestal index {i}: MISSING reference.");
                continue;
            }
            Debug.Log($"[Puzzle]  - {p.name}: requires tag '{p.requiredGemTag}' (current occupant: {p.OccupantTag})");
            _lastCorrect[i] = p.isCorrect;
        }
    }

    void Update()
    {
        if (_opened || pedestals == null || pedestals.Length == 0) return;

        bool allCorrect = true;

        for (int i = 0; i < pedestals.Length; i++)
        {
            var p = pedestals[i];
            if (!p) { allCorrect = false; continue; }

            // Log state changes only
            if (p.isCorrect != _lastCorrect[i])
            {
                _lastCorrect[i] = p.isCorrect;
                if (p.isCorrect)
                    Debug.Log($"[Puzzle] Pedestal '{p.name}' CORRECT (occupant: {p.OccupantTag})");
                else
                    Debug.Log($"[Puzzle] Pedestal '{p.name}' INCORRECT / EMPTY (occupant: {p.OccupantTag})");
            }

            if (!p.isCorrect) allCorrect = false;
        }

        if (allCorrect)
        {
            _opened = true;
            Debug.Log("[Puzzle] All pedestals correct. Opening door!");
            doorMover?.Open();
        }
    }
}
