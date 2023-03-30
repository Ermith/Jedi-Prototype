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
        DOTween.Clear();
        SceneManager.LoadScene("TutorialLevel");
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
        DOTween.Clear();
        SceneManager.LoadScene("MainMenu");
        //DOTween.KillAll();
    }


}
