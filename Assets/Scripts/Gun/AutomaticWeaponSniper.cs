
using Sirenix.OdinInspector;
using UnityEngine;



public enum SniperUpgrades { }
public class AutomaticWeaponSniper : AutomaticWeaponBase
{
    [TabGroup("SetUp"), SerializeField] ParticleSystem muzzleFlashPFX;
    [TabGroup("SetUp"), SerializeField] ParticleSystem bloodHitPFX;
    [TabGroup("SetUp"), SerializeField] ParticleSystem terrainFX;
    [TabGroup("SetUp"), SerializeField] LineRenderer lineRenderer;
    [TabGroup("Effects"),SerializeField] float vaporTrailDissolveSpeed = 1;
    Animator anim;
    GameObject enemyObj;
    float aimTimer = 0;
    bool lineActive = false;
    Material lineRendererMat;
    float lineRendererDissolveTimer = 1;
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
        if(lineRendererDissolveTimer != 1)
        {
            lineRendererMat.SetFloat("_Dissolve", Mathf.Clamp(lineRendererDissolveTimer += Time.deltaTime * vaporTrailDissolveSpeed,0,1));
            if(lineRendererDissolveTimer >= 1)
                lineRenderer.gameObject.SetActive(false);
        }
        if (firing) // Wait until animation has finished
            return;
        if (!UpdateCoolDown()) // wait until off cooldown
            return;

        if(!RayCastForwardGameObject(everythingEnemy))
        {
            anim.SetBool("Armed", true);
            return;
        }
        onCooldown = true;
        firing = true;
        anim.SetBool("Armed", false);
        anim.SetTrigger("Shoot");
        ShootAtTarget();
        //ShootAtTarget();
    }

    /// <summary>
    /// used to publically call ShootAtTarget
    /// iterator is used to call loop ShootAtTarget in case stats.Bullets > 1
    /// </summary>
    /// <param name="iterator"></param>
    public override void Shoot(int iterator = 0)
    {
        //ShootAtTarget(iterator == 0);
    }
    /// <summary>
    /// Handles damaging enemy and playing / Spawning PFX
    /// </summary>
    /// <param name="playFX"></param>
    public void ShootAtTarget(bool playFX = true)
    {
        if (playFX)
            muzzleFlashPFX.Play();
        if(audioSource)
            PlayRandomAudioClip(fireAudioClips);

        lineRenderer.SetPosition(0, playerTransform.position);//muzzleFlashPFX.transform.position);

        if(stats.numberToPierce > 0)
        {
            RaycastHit[] hits = RayCastForwardRayHitEverything(excludePlayerLayerMask);
            SortRaycastHits(ref hits);

            for(int i = 0; i < hits.Length; i++)
            {   
                if(stats.numberToPierce == i)
                {
                    break;
                }
                if(!HitCheck(hits[i]))
                {
                    break;
                }
            }
            
            if(hits.Length <= stats.numberToPierce)
                lineRenderer.SetPosition(1, hits[hits.Length -1].point);
            else
                lineRenderer.SetPosition(1,transform.position + transform.forward * 100);
            
        }
        else
        {
            RaycastHit hit = RayCastForwardRayHit(excludePlayerLayerMask);
            HitCheck(hit);
            
            if(stats.numberToPierce > 0)
                lineRenderer.SetPosition(1,transform.position + transform.forward * 100);
            else
                lineRenderer.SetPosition(1,hit.point);
        }

        lineRenderer.gameObject.SetActive(true);
        lineRendererMat.SetFloat("_Dissolve", 1);
        lineRendererDissolveTimer = 0;
    }
    /// <summary>
    /// Checks if a RayCastHit is an enemy or terrain and then damages them and spawns FX
    /// </summary>
    /// <param name="hit"></param>
    /// <returns> true if its an enemy</returns>
    bool HitCheck(RaycastHit hit)
    {
        if (hit.transform == null)
            return false;
        EnemyDamageHandler dH;
        if(dH = hit.transform.GetComponent<EnemyDamageHandler>())
        {
            dH.DealDamage(stats.damage);
            if (bloodHitPFX)
                Instantiate(bloodHitPFX, hit.transform.position, Quaternion.identity).transform.LookAt(playerTransform.position);
            return true;
        }
        else
        {
            if(terrainFX)
                Instantiate(terrainFX, hit.transform.position, Quaternion.identity).transform.LookAt(playerTransform.position);
            return false;
        }
    }
    public void FinishAnimation()
    {
        firing = false;
    }
}