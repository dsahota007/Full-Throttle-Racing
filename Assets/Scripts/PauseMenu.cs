using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject postProcessingVolume;

    private bool isPaused = false;


    void Start()
    {
        pauseMenuUI.SetActive(false);
        postProcessingVolume.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Freeze the game
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
        AudioListener.pause = true;
        postProcessingVolume.SetActive(true);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f; 
        isPaused = false;
        //Cursor.lockState = CursorLockMode.Locked; // Lock the cursor back
        //Cursor.visible = false; // Hide the cursor
        AudioListener.pause = false;
        postProcessingVolume.SetActive(false);
    }

    public void MenuBtn()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f; 
        isPaused = false;
        AudioListener.pause = false;
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

}
