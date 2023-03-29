using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public Canvas PauseMenu;
    public PlayerInput CharacterInput;
    private bool _isPaused = false;
    private List<Tween> _pausedTweens;

    private void Start()
    {
        _isPaused = false;
        Cursor.visible = _isPaused;
        PauseMenu.gameObject.SetActive(_isPaused);
        CharacterInput.enabled = !_isPaused;
        Time.timeScale = 1.0f;
    }

    public void OnPause()
    {
        _isPaused = !_isPaused;
        Cursor.visible = _isPaused;
        PauseMenu.gameObject.SetActive(_isPaused);
        CharacterInput.enabled = !_isPaused;
        Time.timeScale = (_isPaused) ? 0 : 1;
    }
}
