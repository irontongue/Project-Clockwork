using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class AutomaticWeaponCooldownUI : MonoBehaviour
{
    public static AutomaticWeaponCooldownUI selfRef;
    [SerializeField]GameObject uiPrefab; 
    [SerializeField]Color backgroundColor = new Color(0.5f,0.5f,0.5f, 0.5f);
    int amountOfWeapons;
    [SerializeField] Vector2 initialOffset;
    [SerializeField] float yOffset = 105f;
    [SerializeField] float xOffeset = 105f;
    [SerializeField] int imagesPerRow = 3;


    void Awake()
    {
        selfRef = this;
    }

    [Button("AddWeapon")]
    public Image AddWeapon(Sprite cooldownImage = null)
    {
        Vector2 position = new Vector2(initialOffset.x + transform.position.x + xOffeset * (float)Mathf.Floor((float)amountOfWeapons / (float)imagesPerRow), initialOffset.y + transform.position.y + yOffset * (amountOfWeapons % imagesPerRow));
        Image image = Instantiate(uiPrefab, position, Quaternion.identity, transform).GetComponent<Image>();
        image.sprite = cooldownImage;
        image.color = backgroundColor;

        Image child = image.transform.GetChild(0).GetComponent<Image>();
        child.sprite = cooldownImage;
        child.type = Image.Type.Filled;
        child.fillMethod = Image.FillMethod.Vertical;
        amountOfWeapons++;
        return child;
    }
}
