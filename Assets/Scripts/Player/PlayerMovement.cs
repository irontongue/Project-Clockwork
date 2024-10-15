using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{



    // Internal Variables;

    Vector3 currentVelocity;
    Vector3 dashVelocity;
    Vector3 lastPlayerVelocity;
    Vector3 maxPlayerWalkSpeed;
    Vector3 lastPlayerSafePos;

    bool isGrounded;
    bool isSliding;
    static public GameObject playerRef;

    public static Vector3 playerPosition;
    public static quaternion playerRotation;
    public static Transform playerTransform;


    
    void Awake()
    {
        playerTransform = this.transform;
        playerRef = this.gameObject;
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        remainingJumps = extraJumps;
        maxPlayerWalkSpeed = (Camera.main.transform.right * 1 + Camera.main.transform.forward * 1) * baseSpeed;
        playerRef = gameObject;

        //slide inits
        cam = Camera.main;
        initialCamY = cam.transform.localPosition.y;
    }


    void Update()
    {
        playerPosition = transform.position;
        playerRotation = transform.rotation;
        
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
        DevText.DisplayInfo("isaccGrounded", "Actually Grounded : " + isActuallyGrounded, "Movement");
        DevText.DisplayInfo("onlycontrollergrounded", "Only Controller Grounded : " + controllerColidingButNotGrounded, "Movement");
        DevText.DisplayInfo("cyote", "Cyote Time:" + currentCyoteTime, "Movement");
        DevText.DisplayInfo("isActuallyGrounded", "isActuallyGrounded " + isActuallyGrounded, "Movement");
        DevText.DisplayInfo("lastPlayerSafePos", "lastPlayerSafePos " + lastPlayerSafePos, "Movement");
        DevText.DisplayInfo("maxPlayerWalk", "MaxPlayerWalkSpeed" + maxPlayerWalkSpeed, "Movement");
        DevText.DisplayInfo("sprintTimer", "TimeToNextDash: " + dashCooldownTimer, "Movement");
        if (isActuallyGrounded)
        {
            lastPlayerSafePos = transform.position;
        }
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
    [SerializeField] float slideFriction;
    float currentCyoteTime;
    bool isActuallyGrounded;



    void GroundCheck()
    {
        RaycastHit hit;
        isActuallyGrounded = Physics.SphereCast(transform.position, 0.25f, Vector3.down, out hit, (controller.height / 1.8f) + groundedThreshold, groundMask);

        currentCyoteTime -= Time.deltaTime;

        if (isActuallyGrounded)
        {

            isGrounded = true;
            currentCyoteTime = coyoteTime;

            remainingJumps = extraJumps;
        }
        else if(currentCyoteTime <= 0)
                isGrounded = false;

        //credit to https://discussions.unity.com/t/character-controller-slide-down-slope/188130/2
        if (controllerColidingButNotGrounded)
        {
            currentVelocity.x += (1f - controllerHitNormal.y) * controllerHitNormal.x * (1f - slideFriction);
            currentVelocity.z += (1f - controllerHitNormal.y) * controllerHitNormal.z * (1f - slideFriction);

          
        }
    }

    bool controllerColidingButNotGrounded;
    Vector3 controllerHitNormal;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        controllerColidingButNotGrounded = Vector3.Angle(Vector3.up, hit.normal) <= 55;

        controllerHitNormal = hit.normal;
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
            dashCooldownTimer = dashCooldown;
            readyToDash = false;
        }

        if (currentlyDashing)
        {
            dashTimer -= Time.deltaTime;

            currentVelocity.x = dashVelocity.x;
            currentVelocity.z = dashVelocity.z;

            if (dashTimer <= 0)
                CancelDash();

            return;
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
    [Header("Slide Settings")]
    [SerializeField] float extraVelRequiredToSlide;
    [SerializeField] float slideFallOff;
    [SerializeField] float uphillExtraSlideFallOff;
    [SerializeField] float minMagBeforeSlideCancelMultiplyer;
    [SerializeField] float minAngleForSlide;
    [SerializeField] float downSlopeAccelMultiplyer;
    [Header("CameraSettings")]
    [SerializeField] float camYDrop;
    [SerializeField] float camDropSpeed;
    Camera cam;
    float initialCamY;

    bool onSlope;
    bool slidingTooSlow;
    Vector3 newCamPos;
    bool chainSlideCheck; // since im checking to slide on getkey, if you just keep holding controll, you stop sliding and instantly start again, so this will stop that from happening

    Vector3 VelWithoutGravity;
    void Slide()
    {
        DevText.DisplayInfo("onSlope", "OnSlope: " + onSlope, "Sliding");
        DevText.DisplayInfo("slidingTooSlow", "SlidingTooSlow: " + slidingTooSlow, "Sliding");
        DevText.DisplayInfo("sliding", "Sliding: " + isSliding, "Sliding");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSliding = false;
            return;
        }


        onSlope = GroundAngle() > minAngleForSlide;

        VelWithoutGravity = currentVelocity; // dont want gravity effecting the magnitude, since we only care about the plannar movements
        slidingTooSlow = VelWithoutGravity.magnitude < maxPlayerWalkSpeed.magnitude * minMagBeforeSlideCancelMultiplyer && !onSlope;



        if (isSliding)
        {
            if (cam.transform.localPosition.y > initialCamY - camYDrop)
            {
                newCamPos = cam.transform.localPosition;
                newCamPos.y -= camDropSpeed * Time.deltaTime;
                if (newCamPos.y < initialCamY - camYDrop)
                    newCamPos.y = initialCamY - camYDrop;

                cam.transform.localPosition = newCamPos;
                
            }
                
            if(FacingUp())
                currentVelocity -= VelWithoutGravity.normalized * (slideFallOff + uphillExtraSlideFallOff) * Time.deltaTime;
            else if (!onSlope)
                currentVelocity -= VelWithoutGravity.normalized * slideFallOff * Time.deltaTime;
            else
                currentVelocity += VelWithoutGravity.normalized * downSlopeAccelMultiplyer * Time.deltaTime;

            if (slidingTooSlow)
            {
                currentVelocity = Vector3.zero;
                isSliding = false;
              
            }

            return;
        }
        else if (cam.transform.localPosition.y < initialCamY )
        {
            newCamPos = cam.transform.localPosition;
            newCamPos.y += camDropSpeed * Time.deltaTime;
            if (newCamPos.y > initialCamY)
                newCamPos.y = initialCamY;

            cam.transform.localPosition = newCamPos;

        }
 
        Physics.Raycast(transform.position + (transform.forward * 0.5f), Vector3.down, out RaycastHit hit, controller.height + 1);

        if (hit.point.y > transform.position.y - 0.5)
            return;

        if (FacingUp())
            return;

        if (slidingTooSlow)
            return;

        if (Input.GetKey(KeyCode.LeftControl) && !chainSlideCheck)
        {
            chainSlideCheck = true;
            isSliding = true;
            CancelDash();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (chainSlideCheck)
            {
                chainSlideCheck = false;
                return; // dont want to instantly slide again
            }
            
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
    public void ResetPlayerToSafePos()
    {
        controller.enabled = false;
        transform.position = lastPlayerSafePos;
        controller.enabled = true;
        currentVelocity = Vector3.zero;
        lastPlayerVelocity = Vector3.zero;
    }
   

    [Header("Dependancies")]
    CharacterController controller;
}
