using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AutomaticWeaponMissleLauncher : AutomaticWeaponBase
{
    [SerializeField] GameObject misslePrefab;
    [SerializeField] Transform missleLaunchPoint;
    GameObject missle;
    public static Vector3 lockOnPoint;
    protected  override void Start()
    {
        base.Start();
        ObjectPooler.InitilizeObjectPool("AWML_Missle", misslePrefab, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameState.GamePaused)
            return;
        SetLockOnPoint();
        if(!UpdateCoolDown())
            return;
        if(firing)
        {
            missle = ObjectPooler.RetreiveObject("AWML_Missle");
            missle.transform.position = cam.transform.position;
            missle.transform.forward = cam.transform.forward;
            missle.SetActive(true);
            firing = false;
            onCooldown = true;
        }
        else
        {   
            if(audioSource) 
                PlayRandomAudioClip(fireAudioClips);
            firing = true;
        }
        void SetLockOnPoint()
        {
            RaycastHit hit;
            if(Physics.Raycast(playerTransform.position, cam.transform.forward, out hit, Mathf.Infinity,excludePlayerLayerMask))
            {
                lockOnPoint = hit.point;
            }
            else
                lockOnPoint = playerTransform.position + playerTransform.forward * 100f;
        }

    }
}
