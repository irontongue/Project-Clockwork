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
    public static WeaponRefrenceManager instance;

    private void Awake()
    {
        instance = this;
    }


    public void RecieveUpgrade(WeaponType weaponType, WeaponStatType weaponStatType, float amountToAdd)
    {
        UpdateStats(revolver, weaponStatType, amountToAdd);// temp for now, will be switch for each primary weapon
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
            case WeaponStatType.ReloadSpeed:
                weapon.reloadTime += amountToAdd;
                break;
            case WeaponStatType.MagCapacity:
                weapon.magCapacity += (int)amountToAdd;
                break;
        
        }
    }
}
