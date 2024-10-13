using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

public class FPSRecorder : MonoBehaviour
{
    [SerializeField] float measureTime = 10;
    [SerializeField] TextMeshProUGUI text;
    float frames;
    bool fin;

    string fileName = "Benchmark.txt";
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        frames++;

        if(Time.timeSinceLevelLoad >= measureTime && !fin)
        {
            print(frames / measureTime);
            text.text = (frames / measureTime).ToString();
            Invoke("ReLoad", 2);
            fin = true;
            StreamWriter writer;
            if (!File.Exists(fileName))
                writer = File.CreateText(fileName);
            else
                writer = File.AppendText(fileName);

          writer.WriteLine((frames / measureTime).ToString());
          writer.Close();

        }
    }

    void ReLoad()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
}
