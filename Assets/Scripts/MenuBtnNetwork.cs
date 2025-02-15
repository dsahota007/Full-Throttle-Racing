using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBtnNetwork : MonoBehaviour
{
    private static string lastScene = ""; // To store the name of the last scene.

    private void Start()
    {
        // Save the current scene as the last scene before switching.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Clean up the event listener to avoid memory leaks.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu") // Optionally skip tracking MainMenu.
        {
            lastScene = scene.name;
        }
    }

    public void MainMenuBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTrack1()
    {
        SceneManager.LoadScene("Track1");
    }

    public void LoadTrack2()
    {
        SceneManager.LoadScene("Track2");
    }

    public void LoadTrack3()
    {
        SceneManager.LoadScene("Track3");
    }

    public void ReloadLastScene()
    {
        if (!string.IsNullOrEmpty(lastScene))
        {
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.LogWarning("No previous scene to reload!");
        }
    }
}
