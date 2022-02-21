using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{

    public AudioMixer audioMixer; // creating public audio mixer 

    // list of our resolutions split into widths and heights
    List<int> widths = new List<int>() { 640, 800, 1024, 1280, 1360, 1600, 1680, 1920 };
    List<int> heights = new List<int>() { 480, 800, 768, 720, 768, 900, 1050, 1080 };

    public void SetScreenSize(int index)
    {
        bool fullscreen = Screen.fullScreen;
        int width = widths[index];
        int height = heights[index];
        Screen.SetResolution(width, height, fullscreen); // sets resolution to height and width selected
    }

    public void SetVolume(float volume)
    {
        // changes the value of the mixer - uses exposed parameter
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
