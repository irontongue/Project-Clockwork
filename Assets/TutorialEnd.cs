using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnd : MonoBehaviour
{
    [SerializeField] GameObject tutorialEndPopup;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            GamePauser.instance.PauseGame(true,false, gameObject);
            tutorialEndPopup.SetActive(true);
        }    
    }
}
