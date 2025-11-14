using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    // Exit out of the application
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
