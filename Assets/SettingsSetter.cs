using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

public class SettingsSetter : MonoBehaviour
{

    [SerializeField] Slider audioSlider, mouseSlider;
    [SerializeField] AudioSource music;
    

    private void Start()
    {
        audioSlider.value = GlobalSettings.audioVolume;
        mouseSlider.value = GlobalSettings.mouseSensitivty;
        music.volume = audioSlider.value * 0.05f;
        Application.targetFrameRate = 9999;
        QualitySettings.maxQueuedFrames = 2;

        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
    }

    public void AudioSlider()
    {
        GlobalSettings.audioVolume = audioSlider.value;
        music.volume = audioSlider.value * 0.05f;
    }
    public void MouseSlider()
    {
        GlobalSettings.mouseSensitivty = mouseSlider.value;
    }

}
