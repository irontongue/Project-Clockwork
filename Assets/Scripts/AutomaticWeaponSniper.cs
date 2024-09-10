using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AutomaticWeaponSniper : AutomaticWeaponBase
{
    [SerializeField] Image donutImage;
    [SerializeField] ParticleSystem muzzleFlashPFX;
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
        if(donutImage == null)
        {
            Debug.LogError("On Sniper, DonutImage needs to be assigned");
        }
    }
    void Update()
    {
        if (firing)
            return;
        if (onCooldown)
        {
            if (Timer(ref coolDownTime, stats.coolDown))
                onCooldown = false;
            else
                return;
        }
        enemyObj = RayCastForwardGameObject(everythingEnemy);

        if (enemyObj != null)
        {
            aimTimer = Mathf.Clamp01(aimTimer += Time.deltaTime * stats.chargeUpTime);
            UpdateRing();
            if (anim.GetBool("Armed") == false)
            {
                anim.SetBool("Armed", true);
            }
        }
        else if (aimTimer > 0)
        {
            aimTimer = Mathf.Clamp01(aimTimer -= Time.deltaTime * stats.chargeUpTime);
            UpdateRing();
        }
        else if (anim.GetBool("Armed") == true)
        {
            anim.SetBool("Armed", false);
        }
        if (aimTimer != 1)
            return;
        aimTimer = 0;
        UpdateRing();
        onCooldown = true;
        firing = true;
        anim.SetBool("Armed", false);
        anim.SetTrigger("Shoot");
        //ShootAtTarget();
    }
    public override void Shoot(int iterator = 0)
    {
        ShootAtTarget(iterator == 0);
        base.Shoot(iterator);
    }
    public void ShootAtTarget(bool playFX = true)
    {
        if (playFX)
            muzzleFlashPFX.Play();
        enemyObj.GetComponent<EnemyInfo>().DealDamage(stats.damage);
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
