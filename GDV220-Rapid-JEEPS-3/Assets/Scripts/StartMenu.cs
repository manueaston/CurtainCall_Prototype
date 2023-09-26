using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public LevelTransition transition;

    public void StartGame()
    {
        transition.StartFade();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
