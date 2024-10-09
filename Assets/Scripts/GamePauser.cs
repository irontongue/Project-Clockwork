using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauser : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public static GamePauser instance;

    private void Awake()
    {
        instance = this;
    }
 
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(PauseGame(!GameState.GamePaused, true, gameObject))
                 OpenPauseMenu(GameState.GamePaused);
        }
    }
    bool systemPaused;
    public bool PauseGame(bool pauseGame,bool playerControlledPause, GameObject callback) // i am forcing the callback, just in case a script pauses the game weirdly without us knowing
    {
        if (!playerControlledPause)
            systemPaused = true;

        if (playerControlledPause && systemPaused)
            return false;



        if (Debugger.debugMode)
            print(callback.name + " " + (pauseGame == true ? "Paused" : "Unpaused") + " The Game");
        GameState.GamePaused = pauseGame;
        if(!pauseGame)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            systemPaused = false;
        }
            
        else
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }

        return true;
            
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
