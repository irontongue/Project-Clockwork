using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

public class SettingsSetter : MonoBehaviour
{

    [SerializeField] Slider audioSlider,musicVolumeSlider, mouseSlider;
    [SerializeField] AudioSource music;

    [SerializeField] float musicVolumeMultiplyer = 0.25f;
    private void Start()
    {
        audioSlider.value = GlobalSettings.audioVolume;
        mouseSlider.value = GlobalSettings.mouseSensitivty;
        musicVolumeSlider.value = GlobalSettings.musicVolume ;
        music.volume = musicVolumeSlider.value * musicVolumeMultiplyer;
        Application.targetFrameRate = 9999;
        QualitySettings.maxQueuedFrames = 2;

       // GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
    }

    public void AudioSlider()
    {
        GlobalSettings.audioVolume = audioSlider.value;
        
    }
    public void MouseSlider()
    {
        GlobalSettings.mouseSensitivty = mouseSlider.value;
    }
    public void MusicSlider()
    {
        GlobalSettings.musicVolume = musicVolumeSlider.value;
        music.volume = musicVolumeSlider.value * musicVolumeMultiplyer;
    }

}
