using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    private GameObject player;

    private bool gameMenuEnable = true;

    public GameObject pauseMenuUI;
    [SerializeField] private GameObject helpButton;
    private bool helpButtonEnabled = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameMenuEnable)
        {
            if (GameIsPaused)
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
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        if (!player)
        {
            player = FindObjectOfType<PlayerMovement>().gameObject;
        }        
        player.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        if (!player)
        {
            player = FindObjectOfType<PlayerMovement>().gameObject;
        }
        player.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Quit()
    {
        Application.Quit();

    }

    public void Menu()
    {
        //Add after creating Menu scene

        //SceneManager.LoadScene("Menu");
    }

    public void EnableHelpButton()
    {
        helpButton.SetActive(true);
        helpButtonEnabled = true;
    }

    public void DisableHelpButton()
    {
        helpButton.SetActive(false);
        helpButtonEnabled = false;
    }

    public bool GetHelpButtonState()
    {
        return helpButtonEnabled;
    }

    public void EnableGameMenu(bool value)
    {
        if (value)
        {
            gameMenuEnable = true;
        }
        else
        {
            gameMenuEnable = false;
        }
    }
}
