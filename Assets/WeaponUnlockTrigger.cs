using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUnlockTrigger : MonoBehaviour
{
    [SerializeField] GameObject newWeaponTutorial;

    bool tutorialStarted;
    private void OnTriggerEnter(Collider other)
    {
        if (tutorialStarted)
            return;
        if (other.transform.tag == "Player")
        {
            tutorialStarted = true;
            FindAnyObjectByType<UpgradeManager>().StartWeaponUnlock();
            newWeaponTutorial.SetActive(true);
           
        }

    }

    private void Update()
    {
        if (!tutorialStarted)
            return;

        if (GameState.GamePaused)
            return;

        Destroy(newWeaponTutorial);
    }
}
