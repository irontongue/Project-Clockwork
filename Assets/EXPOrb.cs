using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPOrb : MonoBehaviour
{
    [SerializeField] float autoFloatDistance;
    [SerializeField] float floatSpeed;
    [SerializeField] float acceleration;

    [HideInInspector] public float expValue;

    float currentSpeed;

    private void Update()
    {
        if (GameState.GamePaused)
            return;

        if(currentSpeed > 0)
            transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, PlayerMovement.playerPosition) > autoFloatDistance && GameState.inCombat)
        {
            if (currentSpeed > 0)
                currentSpeed -= acceleration  * Time.deltaTime;
            return;
        }

        currentSpeed += acceleration * Time.deltaTime;

        if (currentSpeed > floatSpeed)
            currentSpeed = floatSpeed;

       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerLevelUpManager.instance.ReciveEXP(expValue);
            Destroy(gameObject);
        }
    }
}
