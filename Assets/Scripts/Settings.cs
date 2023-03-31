using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static float SFXVolume = 1.0f;
    public static float MusicVolume = 1.0f;
    public static float MouseSensitivity = 1.0f;

    private static (int, int)[] Resolutions = { (1920, 1080), (1280, 720), (800, 600) };

    public void OnSFXVolumeChanged(float volume) => SFXVolume = volume;
    public void OnMusicVolumeChanged(float volume) => MusicVolume = volume;
    public void OnMouseSensitivityChanged(float val) => MouseSensitivity = val * 2f;

    public void OnFullsecreen(bool enabled)
    {
        Screen.fullScreen = enabled;
    }

    public void OnResolutionChanged(int index)
    {
        (int width, int height) = Resolutions[index];
        Screen.SetResolution(width, height, Screen.fullScreen);
    }
}
