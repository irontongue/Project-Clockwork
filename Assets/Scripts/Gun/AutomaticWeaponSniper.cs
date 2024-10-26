
using UnityEngine;



public enum SniperUpgrades { }
public class AutomaticWeaponSniper : AutomaticWeaponBase
{
    [SerializeField] ParticleSystem muzzleFlashPFX;
    [SerializeField] ParticleSystem bloodHitPFX;
    Animator anim;
    GameObject enemyObj;
    float aimTimer = 0;
    bool lineActive = false;
    [SerializeField] LineRenderer lineRenderer;
    Material lineRendererMat;
    protected override void Start()
    {
        base.Start();
        lineRendererMat = lineRenderer.material;
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
    }
    void Update()
    {
        if(GameState.GamePaused)
            return;
        if(lineActive)
        {
            lineRendererMat.SetFloat("_Dissolve", 1f);
        }
        if (firing) // Wait until animation has finished
            return;
        if (!UpdateCoolDown()) // wait until off cooldown
            return;
        onCooldown = true;
        firing = true;
        //anim.SetBool("Armed", false);
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
    }
    /// <summary>
    /// Handles damaging enemy and playing / Spawning PFX
    /// </summary>
    /// <param name="playFX"></param>
    public void ShootAtTarget(bool playFX = true)
    {
        if (playFX)
            muzzleFlashPFX.Play();
        if(stats.numberToPierce > 0)
        {
            RaycastHit[] hits = RayCastForwardRayHitEverything(everythingEnemy);
            
            print(hits.Length);
            for(int i = 0; i < hits.Length; i++)
            {   
                print(hits[i].transform.name);
                // if(stats.numberToPierce == i)
                //    break;
                //HitCheck(hits[i]);
            }
        }
        else
        {
            RaycastHit hit = RayCastForwardRayHit(everythingEnemy);
            HitCheck(hit);
        }
    }
    void HitCheck(RaycastHit hit)
    {
        if (hit.transform != null)
        {
            print(hit.transform.name);
            hit.transform.GetComponent<EnemyDamageHandler>().DealDamage(stats.damage);
            if (bloodHitPFX)
                Instantiate(bloodHitPFX, hit.transform.position, Quaternion.identity).transform.LookAt(playerTransform.position);
        }
    }
    public void FinishAnimation()
    {
        firing = false;
    }
}