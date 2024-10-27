using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class AutomaticWeaponBase : MonoBehaviour
{
    [TabGroup("SetUp"), SerializeField] protected WeaponType weaponType;
    [TabGroup("SetUp"),SerializeField] protected DamageType damageType;
    [TabGroup("SetUp"),SerializeField] protected Sprite uiCDSprite;
    Image uiCDImage;
    [TabGroup("LayerMasks"), SerializeField] protected LayerMask enemyLayerMask = 8;
    [TabGroup("LayerMasks"), SerializeField] protected LayerMask excludePlayerLayerMask = ~(1<<6);
    [TabGroup("LayerMasks"), SerializeField] protected LayerMask everythingEnemy = 1 << 3 | 1 << 7;

    [TabGroup("Audio"), SerializeField] protected AudioClip[] fireAudioClips;
    //**REFERENCES**\\
    AllWeaponStats allStats;
    protected WeaponStats stats;
    protected Camera cam;
    protected Transform playerTransform;
    protected AudioSource audioSource;

    //**LOGIC**\\
    protected bool onCooldown = false;
    protected float coolDownTime = 0;
    protected bool firing = false;
    protected float uiCDTimeOffset = 0f;

    virtual protected void Start()
    {
        try{ audioSource = GetComponentInParent<AudioSource>(); }
        catch{ Debug.LogWarning(weaponType + " Does not have an audioSource");}
        uiCDImage = AutomaticWeaponCooldownUI.selfRef.AddWeapon(uiCDSprite);
        playerTransform = transform.root;
        cam = Camera.main;
        allStats = FindAnyObjectByType<AllWeaponStats>();
        stats = allStats.GetWeaponStat(weaponType);
        try
        {
            allStats.weaponReferences.Add(weaponType, this);
        }
        catch 
        {
            Debug.LogWarning("Tried to add multiple of the same key to dictionary");
        }


    }

    /// <summary>
    /// returns the first GameObject in front of the crosshair
    /// returns null if no objects present
    /// takes distance in units otherwise infinite distance
    /// </summary>
    /// <param name="layerMask"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    protected GameObject RayCastForwardGameObject(LayerMask layerMask,float distance = float.MaxValue)
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance,layerMask))
        {
            return hit.transform.gameObject;
        }
        return null;
    }
    /// <summary>
    /// Returns the RayCastHit of the first collider infront of the crosshair
    /// </summary>
    /// <param name="layerMask"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    protected RaycastHit RayCastForwardRayHit(LayerMask layerMask, float distance = float.MaxValue)
    {
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance, layerMask);
        return hit;
    }
    protected RaycastHit[] RayCastForwardRayHitEverything(LayerMask layerMask, float distance = float.MaxValue)
    {
        RaycastHit[] hit = Physics.RaycastAll(cam.transform.position, cam.transform.forward,distance, layerMask);
        return hit;
    }
    /// <summary>
    /// Returns all enemies  within a radius
    /// Useful for explosions
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="position"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    protected EnemyInfo[] GetAllEnemiesInSphere(float radius,Vector3 position, LayerMask layerMask)
    {
        Collider[]hits = Physics.OverlapSphere(position, radius, layerMask);
        EnemyInfo[] info = new EnemyInfo[hits.Length];
        if(hits.Length == 0)
            return null;
        for (int i = 0; i < hits.Length; i++)
        {
            info[i] = hits[i].GetComponent<EnemyInfo>(); 
        }
        return info;
    }
    /// <summary>
    /// Returns a single enemy within a radius
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="position"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    protected EnemyInfo GetEnemyInSphere(float radius, Vector3 position, LayerMask layerMask, List<Transform> enemyTransforms = null)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, layerMask);
        if (hits.Length == 0)
            return null;
        if(enemyTransforms != null)
        {
            foreach(Collider c in hits)
            {
                if(!enemyTransforms.Contains(c.transform))
                    return c.GetComponent<EnemyInfo>();
            }
            return null;
        }
        else
        {
            print("o no");
            return hits[0].GetComponent<EnemyInfo>();
        }

    }
    /// <summary>
    /// Returns GetFirstEnemyInDirection in the forward vector
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    protected EnemyInfo GetFirstEnemyInfrontOfPlayer(float distance, float size, LayerMask layerMask)
    {
        return GetFirstEnemyInDirection(1, distance, size, layerMask);
    }
    /// <summary>
    /// Returns GetFirstEnemyInDirection in the backward vector
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    protected EnemyInfo GetFirstEnemyBehindPlayer(float distance, float size, LayerMask layerMask)
    {
        return GetFirstEnemyInDirection(-1, distance, size, layerMask);
    }
    /// <summary>
    /// Gets the first enemy in the forward or backward vector (direction = 1 for forward) (direction = -1 for backward)
    /// uses sphereCast
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="length"></param>
    /// <param name="width"></param>
    /// <returns>First enemy in the forward or backward vector</returns>
    protected EnemyInfo GetFirstEnemyInDirection(int direction, float length, float width, LayerMask layerMask)
    {
        if (direction != 1 && direction != -1)
        {
            Debug.LogWarning($"GetFirstEnemyInDirection: direction = {direction}, setting to forward (1)");
            direction = 1;
        }
        width = width * 0.5f;
        length = length * 0.5f;
        Collider[] hit = Physics.OverlapBox(playerTransform.position + (direction * length * 0.5f * cam.transform.forward), new Vector3(width, width, length), Quaternion.identity, layerMask);
        
        if (hit.Length == 0)
        {
            return null;
        }
        
        RaycastHit rayHit = new RaycastHit();
        foreach(Collider col in hit)
        {
            if (Physics.Raycast(playerTransform.position, (col.transform.position - playerTransform.position).normalized, out rayHit, float.MaxValue, excludePlayerLayerMask))
            { break; }
        }
        if (!rayHit.transform)
            return null;
        if (1 << rayHit.transform.gameObject.layer == (int)enemyLayerMask)
        {
            return rayHit.transform.GetComponent<EnemyInfo>();
        }
        else
            return null;
    }
    /// <summary>
    /// Deal Damage to a single Enemy
    /// </summary>
    /// <param name="enemyInfo"></param>
    /// <param name="amount"></param>
    protected void DamageEnemy(EnemyInfo enemyInfo, float amount)
    {
        enemyInfo.DealDamage(amount, damageType);
    }
    /// <summary>
    /// Deal damage to multple enemies
    /// loops through a given array
    /// </summary>
    /// <param name="enemyInfos"></param>
    /// <param name="amount"></param>
    protected void DamageMultipleEnemies(EnemyInfo[] enemyInfos, float amount)
    {
        foreach(EnemyInfo info in enemyInfos)
        {
            info.DealDamage(amount, damageType);
        }
    }
    /// <summary>
    /// base to fire any weapon and should be overwritten.
    /// call base.Shoot() at the end of the overided statement to loop any weapon to shoot multiple bullets.
    /// </summary>
    /// <param name="iterator"></param>
    public virtual void Shoot(int iterator = 1)
    {
        if(iterator == stats.repeateShot)
        {
            return;
        }

        iterator++;
        Shoot(iterator);
    }
    /// <summary>
    /// simple Cooldown function to be put at the start of the weapon Update Loop
    /// set onCooldown to true after firing the weapon
    /// e.g.
    /// if(UpdateCoolDown())
    ///     return;
    /// </summary>
    /// <returns>true when onCooldown == false</returns>
    protected bool UpdateCoolDown()
    {
        if (onCooldown)
        {
            UpdateCoolDownUI();
            if (Timer(ref coolDownTime, stats.coolDown))
                onCooldown = false;
        }
        return !onCooldown;
    }
    void UpdateCoolDownUI()
    {
        //Timer(ref totalCooldownTimer, )
        uiCDImage.fillAmount = coolDownTime / 1 + uiCDTimeOffset;
    }
    /// <summary>
    /// Helper function
    /// ticks _time up by deltaTime and speed.
    /// </summary>
    /// <param name="_time"> Reference to own timer float </param>
    /// <param name="speed">1 = 1 second, 2 = 0.5f second</param>
    /// <returns>true if time >= 1 </returns>
    protected bool Timer(ref float _time, float speed)
    {
        _time += Time.deltaTime * speed;
        if(_time >= 1f)
        {
            _time = 0;
            return true;
        }
        return false;
    }
    protected void PlayRandomAudioClip(AudioClip[] audioClips)
    {
        int index = UnityEngine.Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[index], GlobalSettings.audioVolume);

    }

    public virtual void Upgrade<T>(T upgradeEnum)where T: Enum{ }

}
