using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{



    // Internal Variables;

    Vector3 currentVelocity;
    Vector3 dashVelocity;
    Vector3 lastPlayerVelocity;
    Vector3 maxPlayerWalkSpeed;

    bool isGrounded;
    bool isSliding;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        remainingJumps = extraJumps;
        maxPlayerWalkSpeed = (Camera.main.transform.right * 1 + Camera.main.transform.forward * 1) * baseSpeed;

    }


    void Update()
    {
        if (GameState.GamePaused)
            return;
        Gravity();
        GroundCheck();
        Jump();
        CaculateMoveVelocity();
        Dash();
        Slide();
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
    Vector3 inputVector; // the raw input vector
    Vector3 walkVector; // the players move vector, unaffected by anything else
    void CaculateMoveVelocity()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.z = Input.GetAxisRaw("Vertical");

        inputVector.Normalize(); // normalize the vector, so you dont move faster when moving left and foward.

        float preservedGravity = currentVelocity.y;
        if (isSliding)
            return;

        if (!isActuallyGrounded || jumping)
        {
            currentVelocity.x = lastPlayerVelocity.x;
            currentVelocity.z = lastPlayerVelocity.z;

            currentVelocity += ((Camera.main.transform.right * inputVector.x + Camera.main.transform.forward * inputVector.z) * airMoveSpeed) * Time.deltaTime;
            currentVelocity.y = preservedGravity;
            return;
        }

        // i do not want to move the y velocity depending on what way the camera is facing, so i temporarly cache the velocity to restore later
        walkVector = (Camera.main.transform.right * inputVector.x + Camera.main.transform.forward * inputVector.z) * baseSpeed;


        currentVelocity = walkVector;
        currentVelocity.y = preservedGravity;
    }

    #endregion
    #region Gravity

    [Header("Gravity")]
    [SerializeField] float gravity;
    bool jumping;
    void Gravity()
    {
        currentVelocity.y -= gravity * Time.deltaTime;

        currentJumpTime -= Time.deltaTime;

        jumping = currentJumpTime > 0;

        if (isActuallyGrounded && currentJumpTime <= 0)
        {
            currentVelocity.y = -gravity * 0.5f;
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
        isActuallyGrounded = Physics.SphereCast(transform.position, 0.25f, Vector3.down, out hit, (controller.height / 2) + groundedThreshold, groundMask);

        currentCyoteTime -= Time.deltaTime;

        if (isActuallyGrounded)
        {

            isGrounded = true;
            currentCyoteTime = coyoteTime;

            remainingJumps = extraJumps;
        }
        else if(currentCyoteTime <= 0)
                isGrounded = false;
    }

    #endregion

    #region Jumping

    [Header("Jumping")]
    [SerializeField] float initialJumpVel;
    [SerializeField] float extraJumps;

    float remainingJumps;

    float jumpGracePeriod = 0.3f;
    float currentJumpTime;

    void Jump()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (!isGrounded)
        {
            if (remainingJumps > 0)
                remainingJumps--;
            else
                return;
        }

        currentVelocity.y = initialJumpVel;
        isGrounded = false;

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
        if (isSliding)
            return;
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
                CancelDash();
            
        }
        if (!readyToDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
                readyToDash = true;
        }
    }

    void CancelDash()
    {
        currentlyDashing = false;
        dashCooldownTimer = dashCooldown;
        dashVelocity = Vector3.zero;
    }

    #endregion

    #region Sliding

    [SerializeField] float extraVelRequiredToSlide;
    [SerializeField] float slideFallOff;
    [SerializeField] float uphillExtraSlideFallOff;
    [SerializeField] float minMagBeforeSlideCancel;
    [SerializeField] float minAngleForSlide;

    bool onSlope;
    bool slidePossible;

    Vector3 VelWithoutGravity;
    void Slide()
    {
        slidePossible = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSliding = false;
            return;
        }


        onSlope = GroundAngle() > minAngleForSlide;

        VelWithoutGravity = currentVelocity; // dont want gravity effecting the magnitude, since we only care about the plannar movements
        VelWithoutGravity.y = 0;

        if (isSliding)
        {
            if(FacingUp())
                currentVelocity -= VelWithoutGravity.normalized * (slideFallOff + uphillExtraSlideFallOff) * Time.deltaTime;
            else if (!onSlope)
                currentVelocity -= VelWithoutGravity.normalized * slideFallOff * Time.deltaTime;
            else
                currentVelocity += VelWithoutGravity.normalized * gravity * Time.deltaTime;

            if (VelWithoutGravity.magnitude < minMagBeforeSlideCancel)
            {
                currentVelocity = Vector3.zero;
                isSliding = false;
              
            }

            return;
        }
       
        Physics.Raycast(transform.position + (transform.forward * 0.5f), Vector3.down, out RaycastHit hit, controller.height + 1);

        if (hit.point.y > transform.position.y - 0.5)
            return;

        if (FacingUp())
            return;

        if (!slidePossible)
            return;
      
        
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isSliding = true;
            CancelDash();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isSliding = !isSliding;
            if (!isSliding)
                return;

            CancelDash();
        }
    }

    RaycastHit hit;

    float GroundAngle()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out hit, controller.height))
            return 0;

        return Vector3.Angle(hit.normal, transform.up);

    }

    bool FacingUp()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out hit, controller.height))
            return false;
        
        return Vector3.Dot(transform.forward, hit.normal) < -0.2f; // mystery number is because on a flat ground, the dot retruns verrrrry slightly below zero. -0.2 seems to be a good number
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
