using UnityEngine;
using Bhaptics.SDK2;

public class BHapticsBootstrap : MonoBehaviour
{
    // Drag your project JSON (TextAsset) in the Inspector
    public TextAsset tactProjectJson;

    // Pick something unique for your app; non-empty apiKey is required by this overload
    const string APP_ID  = "682db5d73bea372354b0e52e";
    const string API_KEY = "rlAwIIe12AH7I2rugal8"; // any non-empty string works on-device

    void Awake()
    {
        if (tactProjectJson == null)
        {
            Debug.LogError("[bHaptics] No project JSON assigned.");
            return;
        }

        // Initialize(appId, apiKey, projectJsonText, isEditor)
        BhapticsLibrary.Initialize(APP_ID, API_KEY, tactProjectJson.text, false);
        Debug.Log("[bHaptics] Initialized with project JSON (4-arg overload).");
    }
}
