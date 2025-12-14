using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start the game by loading the main scene
    public void PlayGame()
    {
        SceneManager.LoadScene("Lobby");
    }

    // Exit
    public void ExitGame()
    {
        Application.Quit();
    }
}
