using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauser : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public static GamePauser instance;

    private void Start()
    {
        instance = this;
    }
 
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!GameState.GamePaused);
            OpenPauseMenu(GameState.GamePaused);
        }
    }

    public void PauseGame(bool open)
    {
        GameState.GamePaused = open;
        if(!open)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
            
        else
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
            
    }
    void OpenPauseMenu(bool open)
    {
        if(open)
        {
            try
            {
                pauseMenu.SetActive(true);
            }
            catch { }
        }
        else
        {
            try
            {
                pauseMenu.SetActive(false);
            }
            catch { }
        }
    }
}
