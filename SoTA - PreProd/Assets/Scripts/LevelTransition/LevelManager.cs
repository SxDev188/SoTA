using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private Scene activeScene;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        // Singleton moment

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        activeScene = SceneManager.GetActiveScene();
    }
    public void LoadNextSceen()
    {
        int nextBuildIndex = activeScene.buildIndex + 1;
        if (SceneManager.sceneCount > nextBuildIndex)
        SceneManager.LoadScene(nextBuildIndex);     
        else
        {
            Application.Quit();
            EditorApplication.isPlaying = false;
        }
    }
}
