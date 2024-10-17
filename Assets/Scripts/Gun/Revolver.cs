using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : BasePrimaryWeapon
{
    
    protected override void Update()
    {
        base.Update();
      
        if (GameState.GamePaused)
            return;

        if ((Input.GetKeyDown(KeyCode.R) || currentAmmoInMag <= 0) && CanReloadWeapon() && !weaponReloading)
        {
            StartReload();
            return;
        }

        if (!CanFire())
            return;

        if (!Input.GetMouseButton(0))
            return;

        DamageFowardEnemy();
        PlayFireSound();
        PlayFireAnimation();
        RemoveAmmo(1);
        UpdateAmmoUI();
        ResetFireCooldown();




    }
}
