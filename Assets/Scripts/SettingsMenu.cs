using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private bool enabled;
    [Header("Audio Manager & BGM")] 
    public AudioSource AudioManagerAudioSource;
    public AudioSource BGMAudioSource;
    [Header("Object Refs")]
    public GameObject settingsMenuCanvas;
    public Slider audioVolumeSlider;
    public Slider bgmVolumeSlider;
    
    
    private void Start()
    {
        //Look for the AudioSource component refs for the AudioManager & BGM GameObjects in the scene if they're not linked in the inspector
        if (AudioManagerAudioSource == null)
        {
            AudioManagerAudioSource = FindObjectOfType<AudioManager>().gameObject.GetComponent<AudioSource>();
        }
        if (BGMAudioSource == null)
        {
            BGMAudioSource = FindObjectOfType<BGMManager>().gameObject.GetComponent<AudioSource>();
        }

        //Default the settings menu canvas to inactive on scene start
        settingsMenuCanvas.SetActive(false);
        //Set the slider UI values to the current Audio Source values
        audioVolumeSlider.value = AudioManagerAudioSource.volume;
        bgmVolumeSlider.value = BGMAudioSource.volume;
    }

    public void SetAudioVolume()
    {
        AudioManagerAudioSource.volume = audioVolumeSlider.value;
    }

    public void SetBGMVolume()
    {
        BGMAudioSource.volume = bgmVolumeSlider.value;
    }

    public void ToggleMenu()
    {
        enabled = !enabled;
        settingsMenuCanvas.SetActive(enabled);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif 
    }

    void Update()
    {
        //Set the settings menu canvas to active if the escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }
}
