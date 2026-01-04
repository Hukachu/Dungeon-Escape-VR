using UnityEngine;

public class QuestBluetoothPermission : MonoBehaviour
{
    const string ANDROID_BLUETOOTH_CONNECT = "android.permission.BLUETOOTH_CONNECT";
    const string ANDROID_BLUETOOTH_SCAN    = "android.permission.BLUETOOTH_SCAN";
    const string ANDROID_ACCESS_FINE_LOC   = "android.permission.ACCESS_FINE_LOCATION";

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        int sdk = GetSdkInt();
        if (sdk >= 31)
        {
            RequestIfNeeded(ANDROID_BLUETOOTH_CONNECT);
            RequestIfNeeded(ANDROID_BLUETOOTH_SCAN);
        }
        else
        {
            // On API <= 30 BLE scans require location permission
            RequestIfNeeded(ANDROID_ACCESS_FINE_LOC);
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    static int GetSdkInt()
    {
        using var version = new AndroidJavaClass("android.os.Build$VERSION");
        return version.GetStatic<int>("SDK_INT");
    }

    static void RequestIfNeeded(string perm)
    {
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(perm))
            UnityEngine.Android.Permission.RequestUserPermission(perm);
    }
#endif
}
