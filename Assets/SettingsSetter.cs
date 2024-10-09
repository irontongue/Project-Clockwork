using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSetter : MonoBehaviour
{

    [SerializeField] Slider audioSlider, mouseSlider;
    [SerializeField] AudioSource music;
    

    private void Start()
    {
        audioSlider.value = GlobalSettings.audioVolume;
        mouseSlider.value = GlobalSettings.mouseSensitivty;
        music.volume = audioSlider.value;
    }

    public void AudioSlider()
    {
        GlobalSettings.audioVolume = audioSlider.value;
        music.volume = audioSlider.value;
    }
    public void MouseSlider()
    {
        GlobalSettings.mouseSensitivty = mouseSlider.value;
    }

}
