using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Tooltip("Name of the scene to load (must be in Build Settings).")]
    public string sceneName = "GameScene";

    // Hook this to your Button's OnClick()
    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoader] sceneName is empty.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    // (Optional) Async version if you prefer a non-blocking load
    public void LoadTargetSceneAsync()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoader] sceneName is empty.");
            return;
        }
        StartCoroutine(LoadAsync(sceneName));
    }

    private System.Collections.IEnumerator LoadAsync(string name)
    {
        var op = SceneManager.LoadSceneAsync(name);
        op.allowSceneActivation = true;   // set false if you want to wait for a fade, then true
        while (!op.isDone) yield return null;
    }

    // (Optional) Quit button
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
