using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEventNewWeapon : WaveEvent
{
    public override void WaveEnd()
    {
        FindAnyObjectByType<UpgradeManager>().StartWeaponUnlock();
    }
}
