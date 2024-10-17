using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class BasePrimaryWeapon : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] public float damage;
    [SerializeField] public float range;
    [SerializeField] public float seccondsBetweenShots;
    [SerializeField] public float reloadTime;// i may switch this out to anim controlled in the future.

    [SerializeField] protected AudioClip[] fireSounds;
    [SerializeField] protected LayerMask enemyMask;
    

    [SerializeField] public int magCapacity;
    [SerializeField] protected int maxAmmoReserve;

    [SerializeField] protected ParticleSystem muzzleFlashPFX;
    [SerializeField] protected ParticleSystem terrainHitPFX;
    [SerializeField] protected ParticleSystem bloodHitPFX;

    [SerializeField] float ammoWheelMaxFill;




    protected int currentAmmoInMag;
    int currentReserveAmmo;
    AudioSource source;
    Animator animator;

    float timeSinceLastShot = 0;

    TextMeshProUGUI ammoUIText;
    

    Camera cam;
    Image ammoWheel;

    protected bool weaponReloading;
    float weaponReloadTimer;

    // you'll be happy james, im finally getting any external refrences at runtime.
    // Finally - James
    protected virtual void Start()
    {
        cam = Camera.main;
        WeaponRefrenceManager wrm = GetComponentInParent<WeaponRefrenceManager>();
        ammoUIText = wrm.ammoUIText;
        source = wrm.audioSource;
        ammoWheel = wrm.ammoWheel;  
        currentAmmoInMag = magCapacity;
        currentReserveAmmo = maxAmmoReserve;
        animator = GetComponent<Animator>();

        UpdateAmmoUI();
        
    }

    //constantly tick down the shot timer.
    protected virtual void Update()
    {
        if (GameState.GamePaused)
            return;
        timeSinceLastShot -= Time.deltaTime;

        if(weaponReloading)
        {
            weaponReloadTimer -= Time.deltaTime;
            ammoWheel.fillAmount = (weaponReloadTimer / reloadTime);
            if (weaponReloadTimer <= 0)
                ReloadWeapon();
        }
      
    }
    // try to get any enemy infrom of the player
    protected EnemyDamageHandler RayCastForward()
    {
        muzzleFlashPFX.Play();
        RaycastHit hit;
        
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {

            if (hit.transform.GetComponent<EnemyDamageHandler>()) 
            {
        
                Instantiate(bloodHitPFX, hit.point, Quaternion.identity).gameObject.transform.LookAt(cam.transform.position);
                return hit.transform.GetComponent<EnemyDamageHandler>();
            }
            Instantiate(terrainHitPFX, hit.point, Quaternion.identity).gameObject.transform.LookAt(cam.transform.position);
        
        }


        return null;
    }
    //damage the given enemy, has room for a modifier if any abilitys should want to add to it in the future
    protected void DamageEnemy(EnemyDamageHandler enemy, float modifier = 1)
    {
        enemy.DealDamage(damage * modifier);
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
      //  currentReserveAmmo += currentAmmoInMag;
        currentAmmoInMag = 0;

        currentAmmoInMag = magCapacity;
    //    currentReserveAmmo -= magCapacity;
        UpdateAmmoUI();
    }

    protected bool CanFire()
    {
        if (currentReserveAmmo > 0 && timeSinceLastShot <= 0 && !weaponReloading && currentAmmoInMag > 0)
            return true;
        return false;
    }

    protected void DamageFowardEnemy()
    {
        EnemyDamageHandler enemy = RayCastForward();
    
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
        if (ammoWheel == null)
        {
            Debug.LogWarning("ammoUIText not set in WRM");
            return;
        }
        //  ammoUIText.text = currentAmmoInMag + " / " + magCapacity;

        float ammoRemaing = ((float)currentAmmoInMag / (float)magCapacity) ; 
        ammoWheel.fillAmount = ammoRemaing * ammoWheelMaxFill;
  
    }
    protected void PlayFireSound()
    {
        if(source == null)
        {
            Debug.LogWarning("AudioSource not set in WRM");
            return;
        }
        source.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length - 1)], GlobalSettings.audioVolume);
    }
    protected void PlayFireAnimation()
    {
        if (animator == null)
            return;
        animator.SetTrigger("Fire");
    }
    protected void PlayReloadAnimation()
    {
        if (animator == null)
            return;
        animator.SetTrigger("Reload");
    }

}
