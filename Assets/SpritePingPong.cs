using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePingPong : MonoBehaviour
{
    [SerializeField] float jumpSpeed, yJumpMax;
    float initYPos;
    private void Start()
    {
        initYPos = transform.position.y;
    }
    float newYPos;
    void Update()
    {
        newYPos = initYPos + Mathf.PingPong(Time.time * jumpSpeed, yJumpMax);
        transform.position = new(transform.position.x, newYPos,transform.position.z);
    }
}
