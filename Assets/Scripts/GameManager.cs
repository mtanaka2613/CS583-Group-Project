using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void StartNew()
    {
        // create a random int to select the first level 
        int randomLevel = Random.Range(2, 7);
        SceneManager.LoadScene(randomLevel);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void ControlsMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
