using UnityEngine;

public class QuitGame : MonoBehaviour
{
    [Tooltip("Optional delay (seconds) before quitting.")]
    public float delay = 0f;

    // Hook this to your Button's OnClick()
    public void Exit()
    {
        if (delay <= 0f)
        {
            DoQuit();
        }
        else
        {
            Invoke(nameof(DoQuit), delay);
        }
    }

    private void DoQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        // Makes the Quit button work while testing in the Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
