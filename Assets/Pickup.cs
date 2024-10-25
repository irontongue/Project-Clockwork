using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    enum PickUpTyp {HealthPotion}
    [SerializeField] PickUpTyp pickUpType;
    [SerializeField] float amount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Consumeable();
        }
    }

    void Consumeable()
    {
        switch(pickUpType)
        {
            case PickUpTyp.HealthPotion:
                GlobalStaticReference.playerDamageHandler.Heal(amount);
                break;
        }

        Destroy(gameObject);
    }

}
