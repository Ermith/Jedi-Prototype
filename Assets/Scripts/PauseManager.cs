using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public Canvas PauseScreen;
    public GameObject Menu;
    public GameObject OptionsMenu;
    public GameObject Credits;
    public PlayerInput CharacterInput;
    private bool _isPaused = false;
    private List<Tween> _pausedTweens;

    private void Start()
    {
        _isPaused = false;
        Cursor.visible = _isPaused;
        PauseScreen.gameObject.SetActive(_isPaused);
        CharacterInput.enabled = !_isPaused;
        Time.timeScale = 1.0f;
    }

    public void OnPause()
    {
        _isPaused = !_isPaused;
        Cursor.visible = _isPaused;
        PauseScreen.gameObject.SetActive(_isPaused);
        CharacterInput.enabled = !_isPaused;
        Time.timeScale = (_isPaused) ? 0 : 1;

        if (!_isPaused)
        {
            Credits.SetActive(false);
            OptionsMenu.SetActive(false);
            Menu.SetActive(true);
        }
    }
}
