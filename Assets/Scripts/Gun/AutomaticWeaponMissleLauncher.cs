using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AutomaticWeaponMissleLauncher : AutomaticWeaponBase
{
    [SerializeField] GameObject misslePrefab;
    [SerializeField] Transform missleLaunchPoint;
    GameObject missle;
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

    }
}
