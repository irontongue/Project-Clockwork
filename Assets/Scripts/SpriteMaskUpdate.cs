using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This Script is required to Update the Shader "SpriteMask" Material.
/// </summary>
public class SpriteMaskUpdate : MonoBehaviour
{

    /// <summary>
    /// 1 = none 0.0001 = all. never set to 0 as it breaks it
    /// </summary>
    [SerializeField]float maskPercentage = 1;
    SpriteRenderer spriteRenderer;
    Material material;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material; 
    }
    public void SetMaskPercentage(float amount)
    {
        maskPercentage = Mathf.Clamp(amount, 0.01f, 1);
    }
    private void Update()
    {
        material.SetFloat("_MaskPercentage", maskPercentage);
        
    }
}
