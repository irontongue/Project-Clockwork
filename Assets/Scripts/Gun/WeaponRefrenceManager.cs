using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponRefrenceManager : MonoBehaviour
{
    public TextMeshProUGUI ammoUIText;
    public AudioSource audioSource;
    public UnityEngine.UI.Image ammoWheel;
    public Revolver revolver;



    public void RecieveUpgrade(WeaponType weaponType, WeaponStatType weaponStatType, float amountToAdd)
    {
       
    }

    void UpdateStats(BasePrimaryWeapon weapon, WeaponStatType weaponStatType, float amountToAdd)
    {
        switch(weaponStatType)
        {
            case WeaponStatType.Damage:
                weapon.damage += amountToAdd;
                break;
            case WeaponStatType.AttackSpeed:
                weapon.seccondsBetweenShots += amountToAdd;
                break;
            /*case WeaponStatType.reloadTime:
                weapon.reloadTime += amountToAdd;
                break;*/
        
        }
    }
}
