using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
    public void BenchMark()
    {
        SceneManager.LoadScene(2);
    }
    public void LoadTutorial()
    {
        SceneManager.LoadScene(2);
    }
}
