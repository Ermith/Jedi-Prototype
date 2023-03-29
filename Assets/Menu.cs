using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private PauseManager _pauseManager;

    private void Start()
    {
        _pauseManager = FindObjectOfType<PauseManager>();
        
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync("TutorialLevel");
        DOTween.Clear();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Options()
    {

    }

    public void Resume()
    {
        _pauseManager?.OnPause();
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        DOTween.KillAll();
        DOTween.Clear();
    }


}
