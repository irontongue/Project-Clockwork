using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

   

    // Internal Variables;

    Vector3 currentVelocity;
    Vector3 dashVelocity;
    Vector3 lastPlayerVelocity;

    bool isGrounded;
    bool jumping;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        CaculateMoveVelocity();
        Gravity();
        GroundCheck();
        Jump();
        Dash();
        FinalMoveCaculation();

        DevText.DisplayInfo("pMovement", "PlayerVelocity: " + currentVelocity, "Movement");
        DevText.DisplayInfo("dMovement", "DashVel: " + dashVelocity, "Movement");
        DevText.DisplayInfo("grounded", "Grounded : " + isGrounded, "Movement");
        DevText.DisplayInfo("cyote", "Cyote Time:" + currentCyoteTime, "Movement");
        DevText.DisplayInfo("isActuallyGrounded", "isActuallyGrounded " + isActuallyGrounded, "Movement");

    }

    #region CaculateMoveVelocity

    [Header("Basic Movement")]
    [SerializeField] float baseSpeed;
    [SerializeField] float airMoveSpeed;
    Vector3 inputVector;
    void CaculateMoveVelocity()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.z = Input.GetAxisRaw("Vertical");
       
        inputVector.Normalize(); // normalize the vector, so you dont move faster when moving left and foward.
        float preservedGravity = currentVelocity.y;

        if (!isGrounded)
        {
            currentVelocity.x = lastPlayerVelocity.x;
            currentVelocity.z = lastPlayerVelocity.z;

            currentVelocity += ((Camera.main.transform.right * inputVector.x + Camera.main.transform.forward * inputVector.z) * airMoveSpeed) * Time.deltaTime;
            currentVelocity.y = preservedGravity;
            return;
        }

       // i do not want to move the y velocity depending on what way the camera is facing, so i temporarly cache the velocity to restore later
        currentVelocity = (Camera.main.transform.right * inputVector.x + Camera.main.transform.forward * inputVector.z) * baseSpeed ;
        currentVelocity.y = preservedGravity;
    }

    #endregion

    #region Gravity

    [Header("Gravity")]
    [SerializeField] float gravity;

    void Gravity()
    {
        currentVelocity.y -= gravity * Time.deltaTime;

        currentJumpTime -= Time.deltaTime;

        if (isActuallyGrounded && currentJumpTime <= 0)
        {
            currentVelocity.y = -1;
        }

    }

    #endregion

    #region GroundCheck

    [Header("Ground Check")]
    [SerializeField] float groundedThreshold;
    [SerializeField] LayerMask groundMask;

    [SerializeField] float coyoteTime;
    float currentCyoteTime;
    bool isActuallyGrounded;

   
  
    void GroundCheck()
    {
        RaycastHit hit;
        isActuallyGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, (controller.height / 2) + groundedThreshold, groundMask);

        currentCyoteTime -= Time.deltaTime;
        

        if (isActuallyGrounded)
        {
            
            isGrounded = true;
            currentCyoteTime = coyoteTime;
            jumping = false;
        }
        else
        {
            if (currentCyoteTime <= 0)
            {
                isGrounded = false;
            }
        }
    }



    #endregion

    #region Jumping

    [Header("Jumping")]
    [SerializeField] float initialJumpVel;

    float jumpGracePeriod = 0.3f;
    float currentJumpTime;

    void Jump()
    {
        if (!isGrounded || jumping)
            return;

        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        currentVelocity.y = initialJumpVel;
        isGrounded = false;
        jumping = true;
        currentJumpTime = jumpGracePeriod;


    }

    #endregion

    #region Dash

    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashLenght;

    [SerializeField] float dashCooldown;
    float dashCooldownTimer;

    bool readyToDash = true;
    bool currentlyDashing;
    float dashTimer;
    void Dash()
    {
        if (readyToDash && Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentlyDashing = true;
            dashTimer = dashLenght;
            Vector3 dashDirection = (Camera.main.transform.right * Input.GetAxisRaw("Horizontal") + Camera.main.transform.forward * Input.GetAxisRaw("Vertical")).normalized;
            dashVelocity = dashDirection * dashSpeed;
        }

        if (currentlyDashing)
        {
            dashTimer -= Time.deltaTime;

            currentVelocity.x = dashVelocity.x;
            currentVelocity.z = dashVelocity.z;

            if (dashTimer <= 0)
            {
                currentlyDashing = false;
                dashCooldownTimer = dashCooldown;
                dashVelocity = Vector3.zero;
            }
        }

        if (!readyToDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
                readyToDash = true;
        }
    }

    #endregion

    #region FinalMoveCaculation

    void FinalMoveCaculation()
    {
        controller.Move((currentVelocity + dashVelocity) * Time.deltaTime);
        lastPlayerVelocity = currentVelocity;
    }

    #endregion

    [Header("Dependancies")]
    CharacterController controller;
}
