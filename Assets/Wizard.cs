using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    public int minimum = 0;
    public int maximum = 10;

    public int guess = 0;

    public float time;
    
    bool delayTrigger = false;
    public float delayTime = 5;

    void Start()
    {
        Invoke("Guess", delayTime);
    }

    void Update()
    {
  

        
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {

        }
    }
    void Guess()
    {
        print(" is your number this?");
    }

    int Avg(int x, int y)
    {
        return (x + y) / 2;
    }
    void func()
    {
        return;
    }


}
