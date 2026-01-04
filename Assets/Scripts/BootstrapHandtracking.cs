using UnityEngine;

public class QuestHandTrackingPermission : MonoBehaviour
{
    // Meta/Quest hand-tracking permission (added to the manifest by your XR/Meta plugin)
    const string OCULUS_HAND_TRACKING = "com.oculus.permission.HAND_TRACKING";

    // Some Unity XR Hands runtimes also expose a generic name; asking for both is harmless.
    const string ANDROID_HAND_TRACKING = "android.permission.HAND_TRACKING";

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        RequestIfNeeded(OCULUS_HAND_TRACKING);
        RequestIfNeeded(ANDROID_HAND_TRACKING);
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    static void RequestIfNeeded(string perm)
    {
        if (!string.IsNullOrEmpty(perm) &&
            !UnityEngine.Android.Permission.HasUserAuthorizedPermission(perm))
        {
            UnityEngine.Android.Permission.RequestUserPermission(perm);
        }
    }
#endif
}
