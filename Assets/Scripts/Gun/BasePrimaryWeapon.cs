using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BasePrimaryWeapon : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float seccondsBetweenShots;
    [SerializeField] protected float reloadTime;// i may switch this out to anim controlled in the future.

    [SerializeField] protected AudioClip[] fireSounds;
    [SerializeField] protected LayerMask enemyMask;
    

    [SerializeField] protected int magCapacity;
    [SerializeField] protected int maxAmmoReserve;

    
    int currentAmmoInMag;
    int currentReserveAmmo;
    AudioSource source;
    Animator animator;

    float timeSinceLastShot = 0;

    TextMeshProUGUI ammoUIText;

    Camera cam;

    bool weaponReloading;
    float weaponReloadTimer;

    // youll be happy james, im finally getting any external refrences at runtime.
    protected virtual void Start()
    {
        cam = Camera.main;
        WeaponRefrenceManager wrm = GetComponentInParent<WeaponRefrenceManager>();
        ammoUIText = wrm.ammoUIText;
        source = wrm.audioSource;

        currentAmmoInMag = magCapacity;
        currentReserveAmmo = maxAmmoReserve;

        UpdateAmmoUI();
        
    }

    //constantly tick down the shot timer.
    protected virtual void Update()
    {
        timeSinceLastShot -= Time.deltaTime;

        if(weaponReloading)
        {
            weaponReloadTimer -= Time.deltaTime;
             
            if (weaponReloadTimer <= 0)
                ReloadWeapon();
        }
        print("can Fire");
    }
    // try to get any enemy infrom of the player
    protected EnemyInfo RayCastForward()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, enemyMask))
        {
            return hit.transform.GetComponent<EnemyInfo>();
        }
        return null;
    }
    //damage the given enemy, has room for a modifyer if any abilitys should want to add to it in the future
    protected void DamageEnemy(EnemyInfo enemy, float modifyer = 1)
    {
        enemy.damageHandler.DealDamage(damage * modifyer);
    }    
    //returns false if there is no ammo, returns true if reload sucseeded 
    protected bool CanReloadWeapon()
    {
        if (currentReserveAmmo <= 0 || currentAmmoInMag >= magCapacity)
            return false;
        return true;
    }
    protected void StartReload()
    {
        weaponReloading = true;
        weaponReloadTimer = reloadTime;
        PlayReloadAnimation();
        ammoUIText.text = "Reloading";
    }
    protected void ReloadWeapon()
    {
        weaponReloading = false;
        currentReserveAmmo += currentAmmoInMag;
        currentAmmoInMag = 0;

        if(currentReserveAmmo <= magCapacity)
        {
            currentAmmoInMag = currentReserveAmmo;
            currentReserveAmmo = 0;
        }    
        else
        {
            currentAmmoInMag = magCapacity;
            currentReserveAmmo -= magCapacity;
        }

        UpdateAmmoUI();
    }

    protected bool CanFire()
    {
        if (currentReserveAmmo > 0 && timeSinceLastShot <= 0 && !weaponReloading)
            return true;
        return false;
    }

    protected void DamageFowardEnemy()
    {
        EnemyInfo enemy = RayCastForward();

        if (enemy == null)
            return;

        DamageEnemy(enemy);
    }

    protected void ResetFireCooldown()
    {
        timeSinceLastShot = seccondsBetweenShots;
    }

    // the reasoin i have this being a function is because some weapons might constume more ammo depending on abilities, and i dont really want to be modifying top level variables in any children
    protected void RemoveAmmo(int count)
    {
        currentAmmoInMag -= count;
    }

    protected void UpdateAmmoUI()
    {
        if (ammoUIText == null)
        {
            Debug.LogWarning("ammoUIText not set in WRM");
            return;
        }
        ammoUIText.text = currentAmmoInMag + " / " + currentReserveAmmo;
    }
    protected void PlayFireSound()
    {
        if(source == null)
        {
            Debug.LogWarning("AudioSource not set in WRM");
            return;
        }
        source.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length - 1)]);
    }
    protected void PlayFireAnimation()
    {
        if (animator == null)
            return;
        animator.SetTrigger("fire");
    }
    protected void PlayReloadAnimation()
    {
        if (animator == null)
            return;
        animator.SetTrigger("reload");
    }

}
