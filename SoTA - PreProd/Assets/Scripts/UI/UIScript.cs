using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    // TWEAKABLE VARIABLES

    [Header("Start Menu Items")]
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject mainMenuStartObject;
    [SerializeField] private GameObject mainMenuList;

    [Header("End Menu Items")]
    [SerializeField] private GameObject endMenuObject;
    [SerializeField] private GameObject endMenuStartObject;
    [SerializeField] private GameObject endMenuList;
  
    [Header("Pause Menu Items")]
    [SerializeField] private GameObject pauseMenuObject;
    [SerializeField] private GameObject pauseMenuStartObject;
    [SerializeField] private GameObject menuList;

    [Header("Other Menus Items")]
    [SerializeField] private GameObject HUD;

    // STORING/VALUE VARIABLES
    private bool isPaused;
    private GameObject playerObject;

    private bool inStartScene = false;
    private bool inEndScene = false;

    // ENGINE METHODS ====================================== // 
    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() // Inefficient but works
    {
        if(playerObject == null) // Try and find the player object again
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }

        if(SceneManager.GetActiveScene().name == "StartScene")
        {
            inStartScene = true;
            inEndScene = false;

            mainMenuObject.SetActive(true);

            HUD.SetActive(false);
            endMenuObject.SetActive(false);
            pauseMenuObject.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "EndScene")
        {
            inStartScene = false;
            inEndScene = true;

            endMenuObject.SetActive(true);

            HUD.SetActive(false);
            mainMenuObject.SetActive(false);
            pauseMenuObject.SetActive(false);
        }
        else
        {
            inStartScene = false;
            inEndScene = false;

            HUD.SetActive(true);

            mainMenuObject.SetActive(false);
            endMenuObject.SetActive(false);
        }
    }


    private void OnPauseGame(InputValue value) 
    {
        if((!inStartScene && !inEndScene))
        {
            if (isPaused) UnPauseGame();
            else PauseGame();
        }
    }

    // METHODS ====================================== //
    public void QuitGame() // TODO: Check if we already have a function for this...
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void UnPauseGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        playerObject.GetComponent<PlayerInput>().enabled = true;
        pauseMenuObject.SetActive(false);
        ResetPauseUI();
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        playerObject.GetComponent<PlayerInput>().enabled = false;
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
