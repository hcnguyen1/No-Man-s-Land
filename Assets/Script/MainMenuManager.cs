using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start the game by loading the main scene
    public void PlayGame()
    {
        Debug.Log("Starting");
        SceneManager.LoadScene("Scene1");
    }

    // Exit
    public void ExitGame()
    {
        Debug.Log("Exiting");
        Application.Quit();
    }
}
