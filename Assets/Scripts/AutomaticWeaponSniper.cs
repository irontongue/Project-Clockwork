using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum SniperUpgrades { }
public class AutomaticWeaponSniper : AutomaticWeaponBase
{
    [SerializeField] Image donutImage;
    [SerializeField] ParticleSystem muzzleFlashPFX;
    [SerializeField] ParticleSystem bloodHitPFX;
    Animator anim;
    GameObject enemyObj;
    float aimTimer = 0;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        if(muzzleFlashPFX == null)
        {
            Debug.LogWarning("On Sniper, Assign MuzzleFlashPFX, trying to assign Now");
            muzzleFlashPFX = GetComponentInChildren<ParticleSystem>();
            if (muzzleFlashPFX == null) { Debug.LogError("On Sniper, There is no MuzzleFlashPFX to assign"); }
            
        }
        if(bloodHitPFX == null) 
        {
            Debug.LogWarning("On Sniper, Hit PFX needs to be assigned");
        }
        if(donutImage == null)
        {
            Debug.LogError("On Sniper, DonutImage needs to be assigned");
        }
    }
    protected override void Update()
    {
        base.Update();
        if (firing) // Wait until animation has finished
            return;
        if (!UpdateCoolDown()) // wait until off cooldown
            return;

        enemyObj = RayCastForwardGameObject(everythingEnemy); // Check if were looking at an enemy and store it as enemy Info

        if (enemyObj != null)
        {
            aimTimer = Mathf.Clamp01(aimTimer += Time.deltaTime * stats.chargeUpTime); // ++ time
            UpdateRing();//Updates the ring UI to match the aimTimer
            if (anim.GetBool("Armed") == false) //Trigger the animation ONCE
            {
                anim.SetBool("Armed", true);
            }
        }
        else if (aimTimer > 0) // If we are not looking at an enemy -- time
        {
            aimTimer = Mathf.Clamp01(aimTimer -= Time.deltaTime * stats.chargeUpTime);
            UpdateRing();
        }
        else if (anim.GetBool("Armed") == true) // If time == 0, Trigger the holster animation ONCE
        {
            anim.SetBool("Armed", false);
        }
        if (aimTimer != 1) // 1 means we are ready to shoot
            return;
        aimTimer = 0; // 0 to reset the ring and get ready for next shot
        UpdateRing(); // setting the ring to 0
        onCooldown = true;
        firing = true;
        anim.SetBool("Armed", false);
        anim.SetTrigger("Shoot");
        //ShootAtTarget();
    }

    /// <summary>
    /// used to publically call ShootAtTarget
    /// iterator is used to call loop ShootAtTarget in case stats.Bullets > 1
    /// </summary>
    /// <param name="iterator"></param>
    public override void Shoot(int iterator = 0)
    {
        ShootAtTarget(iterator == 0);
        base.Shoot(iterator);
    }
    /// <summary>
    /// Handles damaging enemy and playing / Spawning PFX
    /// </summary>
    /// <param name="playFX"></param>
    public void ShootAtTarget(bool playFX = true)
    {
        print("shootAtTarget");
        if (playFX)
            muzzleFlashPFX.Play();
        RaycastHit hit = RayCastForwardRayHit(everythingEnemy);
        if(hit.transform != null)
        {
            hit.transform.GetComponent<EnemyDamageHandler>().DealDamage(stats.damage);
            if(bloodHitPFX)
                Instantiate(bloodHitPFX, hit.transform.position, Quaternion.identity).transform.LookAt(playerTransform.position);
        }
    }
    public void FinishAnimation()
    {
        firing = false;
    }
    void UpdateRing()
    {
        donutImage.fillAmount = aimTimer;
    }
}
