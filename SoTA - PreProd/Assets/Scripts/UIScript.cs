using UnityEngine;
using UnityEngine.InputSystem;

public class UIScript : MonoBehaviour
{
    // TWEAKABLE VARIABLES
    [SerializeField] GameObject pauseMenuObject;
    [SerializeField] GameObject pauseMenuStartObject;
    [SerializeField] GameObject menuList;

    // STORING/VALUE VARIABLES
    private bool isPaused;
    private GameObject playerObject;

    // ENGINE METHODS ====================================== // 
    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnPauseGame(InputValue value) // Add checks for main menu & end screen! Maybe scene name or id?
    {
        if (isPaused) UnPauseGame();
        else PauseGame();
    }

    // METHODS ====================================== //
    public void QuitGame() // Check if we already have a function for this...
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void UnPauseGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        playerObject.GetComponent<PlayerInput>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuObject.SetActive(false);
        ResetPauseUI();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        playerObject.GetComponent<PlayerInput>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuObject.SetActive(true);
    }

    private void ResetPauseUI()
    {
        for (int i = 0; i < menuList.transform.childCount; i++)
        {
            menuList.transform.GetChild(i).gameObject.SetActive(false);
        }
        pauseMenuStartObject.SetActive(true);
    }

}
