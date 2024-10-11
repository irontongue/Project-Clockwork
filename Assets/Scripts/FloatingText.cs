using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class FloatingText : MonoBehaviour
{
    public float timer = 1;
    public float speed = 1;
    public TextMeshProUGUI textMeshPro;
    void Start()
    {
    }
    private void Awake()
    {
        if(textMeshPro == null)
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
       if(timer > 0)
        {
            timer -= Time.deltaTime;
            transform.position = new Vector3(0,speed * Time.deltaTime, 0)  + transform.position;
            transform.LookAt(FloatingTextManager.playerTransform.position);        
        }
       else
        {
            gameObject.SetActive(false);
        }
    }
    public void ResetText(string damageValue)
    {
        timer = 1;
            textMeshPro.text = damageValue;
    }
}
