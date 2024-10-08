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
            PauseGame(!GameState.GamePaused, gameObject);
            OpenPauseMenu(GameState.GamePaused);
        }
    }

    public void PauseGame(bool pauseGame, GameObject callback) // i am forcing the callback, just in case a script pauses the game weirdly without us knowing
    {
        if (Debugger.debugMode)
            print(callback.name + " " + (pauseGame == true ? "Paused" : "Unpaused") + " THE GAME!");
        GameState.GamePaused = pauseGame;
        if(!pauseGame)
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
