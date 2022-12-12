using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class SceneController : MonoBehaviour
{
    private UIActionAsset uiActionAsset;
    [SerializeField] GameObject pauseMenu;

    public TextMeshProUGUI itemsText;

    private void Awake()
    {
        uiActionAsset = new UIActionAsset();
        itemsText.text = "ITEMS: 0 / 5";
    }

    private void OnEnable()
    {
        uiActionAsset.UI.Pause.started += Pause;
        uiActionAsset.UI.Enable();
    }

    private void OnDisable()
    {
        uiActionAsset.UI.Pause.started -= Pause;
        uiActionAsset.UI.Disable();
    }

    public void Pause(InputAction.CallbackContext obj)
    {
        Cursor.lockState = CursorLockMode.Confined;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        
    }
    public void StartNew()
    {
        GameManager.Instance.NextLevel();   
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
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateItemsText(int itemCount)
    {
        itemsText.text = "ITEMS: " + itemCount + " / 5";
    }

}
