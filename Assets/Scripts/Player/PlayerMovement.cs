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

    public static AudioSource playerAudioSource;

    [Header("Dependancies")]
    CharacterController controller;

    void Awake()
    {
        playerTransform = this.transform;
        playerRef = this.gameObject;
        playerAudioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        remainingJumps = extraJumps;
        maxPlayerWalkSpeed = (Camera.main.transform.right * 0.71f + Camera.main.transform.forward * 0.71f) * baseSpeed;
        playerRef = gameObject;

        //slide inits
        cam = Camera.main;
        initialCamY = cam.transform.localPosition.y;
        initialCamFov += cam.fieldOfView;
        initialColliderSize = controller.height;

    }


    void Update()
    {
        playerPosition = transform.position;
        playerRotation = transform.rotation;

        if (GameState.GamePaused)
            return;

        Gravity();

        
        CaculateMoveVelocity();
        Jump();
        Dash();
        Slide();

        GroundCheck();
        FinalMoveCaculation();
        Footstep();
        CameraFov();


        DevText.DisplayInfo("pMovement", "PlayerVelocity: " + currentVelocity, "Movement");
        DevText.DisplayInfo("dMovement", "DashVel: " + dashVelocity, "Movement");
        DevText.DisplayInfo("grounded", "Grounded : " + isGrounded, "Movement");
        DevText.DisplayInfo("isaccGrounded", "Actually Grounded : " + isActuallyGrounded, "Movement");
        DevText.DisplayInfo("jumo", "currently Jumping : " + currentlyJumping, "Movement");
        //  DevText.DisplayInfo("onlycontrollergrounded", "Only Controller Grounded : " + controllerColidingButNotGrounded, "Movement");
        DevText.DisplayInfo("cyote", "Cyote Time:" + currentCyoteTime, "Movement");
        DevText.DisplayInfo("isActuallyGrounded", "isActuallyGrounded " + isActuallyGrounded, "Movement");
        DevText.DisplayInfo("lastPlayerSafePos", "lastPlayerSafePos " + lastPlayerSafePos, "Movement");
        DevText.DisplayInfo("maxPlayerWalk", "MaxPlayerWalkSpeed" + maxPlayerWalkSpeed, "Movement");
        DevText.DisplayInfo("sprintTimer", "TimeToNextDash: " + dashCooldownTimer, "Movement");

        DevText.DisplayInfo("neutralJump", "NeutralJump:" + hadNoInitialVelocity, "Movement");
        DevText.DisplayInfo("currentSeccondsSinceMove:", "LastMove: " + currentSecondsSinceMove, "Movement");
        DevText.DisplayInfo("useReducedjump:", "RedusedJump: " + useReducedJump, "Movement");
        DevText.DisplayInfo("magwihy", "MagWithoutY: " + currentVelocityWithoutY.magnitude, "Movement");
        DevText.DisplayInfo("magwihya", "MaxWalkWithoutY: " + maxPlayerWalkSpeed.magnitude, "Movement");
        DevText.DisplayInfo("Heigt,", "Height: " + controller.height, "Movement");
        if (isActuallyGrounded)
        {
            lastPlayerSafePos = transform.position;
        }


    }

    #region CaculateMoveVelocity

    [Header("Basic Movement")]
    [SerializeField] float baseSpeed;
    [SerializeField] float airMoveSpeed;

    [Header("Initial Air Velocity")]
    [SerializeField] float seccondsSinceMovingBeforeMaxJump;
    [SerializeField] float neutralJumpAirMoveSpeed;
    float currentSecondsSinceMove;
    bool hadNoInitialVelocity;

    Vector3 inputVector; // the raw input vector
    Vector3 walkVector; // the players move vector, unaffected by anything else

    float airJumpMultiplyer;
    bool useReducedJump; // dont know what to name this, this bool stores if we got rid of velocty from a jump if the player jumped before seccondSinceMovingBefor... was surpaced.
    Vector3 currentVelocityWithoutY;
    void CaculateMoveVelocity()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.z = Input.GetAxisRaw("Vertical");

        currentVelocityWithoutY.x = currentVelocity.x;
        currentVelocityWithoutY.z = currentVelocity.z;

        inputVector.Normalize(); // normalize the vector, so you dont move faster when moving left and foward.
        //check if was standing still
        if (inputVector == Vector3.zero && currentVelocityWithoutY.magnitude < 0.1)//0.1 since weird floating point precision errors.
        {
            hadNoInitialVelocity = true;
            currentSecondsSinceMove = 0;
        }
        else
        {
            currentSecondsSinceMove += Time.deltaTime;
            hadNoInitialVelocity = false;
        }
        

        float preservedGravity = currentVelocity.y;

        if (isSliding)
            return;

        if (!isActuallyGrounded || justJumpedGracePeriod)
        {
            if (hadNoInitialVelocity) // add a huge boost to air move if player was standing still- a  neutral jump
                airJumpMultiplyer = neutralJumpAirMoveSpeed;

            if (currentSecondsSinceMove < seccondsSinceMovingBeforeMaxJump && !useReducedJump)// if just started moving, add a tiny space for accleration for short jumps.
            {
                currentVelocity.x *= currentSecondsSinceMove / seccondsSinceMovingBeforeMaxJump;
                currentVelocity.z *= currentSecondsSinceMove / seccondsSinceMovingBeforeMaxJump;
                useReducedJump = true;
            }
            else// keep preserving the last velocity.
            {
                currentVelocity.x = lastPlayerVelocity.x;
                currentVelocity.z = lastPlayerVelocity.z;
            }

           
            currentVelocity += ((Camera.main.transform.right * inputVector.x + Camera.main.transform.forward * inputVector.z) * airMoveSpeed) * airJumpMultiplyer * Time.deltaTime;
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
    bool justJumpedGracePeriod;
    void Gravity()
    {
        currentVelocity.y -= gravity * Time.deltaTime;

        currentJumpTime -= Time.deltaTime;

        justJumpedGracePeriod = currentJumpTime > 0;
        if (isActuallyGrounded && currentJumpTime <= 0 && !isSliding)
        {
            if (currentlyJumping)
                currentVelocity += gravity * Time.deltaTime * -groundNormal;
            else
                currentVelocity.y = -gravity ;
        }
        else
        {
            currentVelocity.y += -gravity * Time.deltaTime;
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


    Vector3 groundNormal;
    void GroundCheck()
    {

        isActuallyGrounded = Physics.SphereCast(transform.position, 0.25f, Vector3.down, out RaycastHit hit, (controller.height / 1.8f) + groundedThreshold, groundMask);

        currentCyoteTime -= Time.deltaTime;

        if (isActuallyGrounded)
        {
            groundNormal = hit.normal;
            isGrounded = true;
            currentCyoteTime = coyoteTime;
            useReducedJump = false;

            remainingJumps = extraJumps;
        }
        else if (currentCyoteTime <= 0)
            isGrounded = false;

        if (currentJumpTime < 0)
            currentlyJumping = false;
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isSliding && Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit && !currentlyJumping)
        {
            currentVelocity = Vector3.ProjectOnPlane(new Vector3(0, -gravity, 0), hit.normal);
        }
    }

    #endregion
    #region Jumping

    [Header("Jumping")]
    [SerializeField] float initialJumpVel;
    [SerializeField] float extraJumps;


    float remainingJumps;
    readonly float jumpGracePeriod = 0.45f;
    float currentJumpTime;

    bool currentlyJumping;

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

        currentVelocity.y = 0;
        
        currentVelocity = currentVelocity.magnitude * cam.transform.forward;
      
        currentVelocity.y = initialJumpVel;
        isGrounded = false;
        currentlyJumping = true;
        
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
            dashCooldownTimer = dashCooldown;
            readyToDash = false;

            if (isSliding)
            {
                isSliding = false;
                slidelock = false;
            }
        }

        if (currentlyDashing)
        {
            dashTimer -= Time.deltaTime;

            currentVelocity.x = dashVelocity.x;
            currentVelocity.z = dashVelocity.z;

            slideGracePeriodActive = true;

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
    [SerializeField] float slideGracePeriodSinceLoosingVelocity;
    [SerializeField] float magnitudeToStopSlide;
    [SerializeField] float timeSinceSlideBeforeAutoCancel = 0.2f;
    [SerializeField] float groundCheckThreshold = -0.2f;
    [Header("Collision")]
    [SerializeField] Vector3 colliderOffset = new(0,0.5f,0);
    [SerializeField] float colliderRadius = 0.5f;
    [SerializeField] LayerMask enviromentLayermask;
    [Header("CameraSettings")]
    [SerializeField] float camYDrop;
    [SerializeField] float camDropSpeed;
    [Header("ColliderSettings")]
    [SerializeField] float minColliderSize;
    [SerializeField] float colliderDropSpeed;
    
    Vector3 velocityBeforeLoosingIt;
    Camera cam;
    float initialCamY;
    float initialColliderSize;

    bool onSlope; // are we currently on a slope
    bool slidingTooSlow; 
    Vector3 newCamPos; // used to control the height of the camerae
    bool chainSlideCheck; // since im checking to slide on getkey, if you just keep holding controll, you stop sliding and instantly start again, so this will stop that from happening
    float timeSinceMinVelocity; // how long has it been since the player has been moving with not enough velocity
    Vector3 VelWithoutGravity; // the players current velocity, but without y
    bool slideGracePeriodActive;// if the player can slide, regardless of if they have the required velocity
    bool slidelock; // since sliding works on input, not imput down, this locks the slide from happening untill the button is released
    Vector3 inputVectorAtStartOfSlide; // what intput was being held at the start of the slide
    float timeSinceSlideStarted = 0; // its in the name
    Vector3 slideDirection; // what direction the slide was started in 
    readonly bool slideDebug = false; // used to print helpfull stuff.
    void Slide()
    {
        SlideDebug();
        UpdateSlideState();

        if (Input.GetKeyUp(KeyCode.LeftControl))
            slidelock = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSliding = false;
            slidelock = false;
            if (slideDebug)
                print("Slide canceled because jumped");
            return;
        }
        if (CheckForCollision())
            isSliding = false;

        if (isSliding)
        {
            SlideMovement();
            return;
        }

        RaisePlayerHeight();

        Physics.Raycast(transform.position + (transform.forward * 0.5f), Vector3.down, out RaycastHit hit, controller.height + 1);


        if (hit.point.y > transform.position.y - 0.5)
        {
            return;
        }
 

        if (FacingUp()) // if the player is moving toward a upwoard slope
            return;

        if (inputVector == Vector3.zero) // if no input is being held, return
            return;

        if (!onSlope)
        {
            if (slidingTooSlow)
            {
                timeSinceMinVelocity -= Time.deltaTime;
                if (timeSinceMinVelocity <= 0)
                {
                    slideGracePeriodActive = false;
                    return;
                }

                slideGracePeriodActive = true;
            }
            else
            {
                timeSinceMinVelocity = slideGracePeriodSinceLoosingVelocity;
                velocityBeforeLoosingIt = currentVelocity;

            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !slidelock)
        {
            StartSlide();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            slidelock = false;
           
        }
    }

  

    // all of the variables used to determine if you can slide, should stop sliding
    void UpdateSlideState()
    {
        onSlope = GroundAngle() > minAngleForSlide;

        VelWithoutGravity = currentVelocity + dashVelocity; // dont want gravity effecting the magnitude, since we only care about the plannar movements
        VelWithoutGravity.y = 0;
        if (!currentlyDashing && !onSlope && !FacingUp())
            slidingTooSlow = VelWithoutGravity.magnitude < maxPlayerWalkSpeed.magnitude + extraVelRequiredToSlide;
        else
            slidingTooSlow = false;
    }

    //this is the code that runs when sliding
    void SlideMovement()
    {

        timeSinceSlideStarted += Time.deltaTime;
        // this is the bit that lets you controll your slide.
        if (inputVectorAtStartOfSlide == Vector3.forward)
        {
            float tempY = currentVelocity.y;
            currentVelocity.y = 0;
            currentVelocity = currentVelocity.magnitude * cam.transform.forward;
            currentVelocity.y = tempY;
        }
        else
        {
            float tempY = currentVelocity.y;
            currentVelocity.y = 0;
            currentVelocity = currentVelocity.magnitude * slideDirection;
            currentVelocity.y = tempY;
        }
        //this make the camera go down when sliding
        if (cam.transform.localPosition.y > initialCamY - camYDrop)
        {
            newCamPos = cam.transform.localPosition;
            newCamPos.y -= camDropSpeed * Time.deltaTime;
            if (newCamPos.y < initialCamY - camYDrop)
                newCamPos.y = initialCamY - camYDrop;

            cam.transform.localPosition = newCamPos;

        }
        // the same, but for the collider
        if (controller.height > minColliderSize)
        {
            controller.height -= Time.deltaTime * colliderDropSpeed;
            if (controller.height < minColliderSize)
                controller.height = minColliderSize;
        }

        if (FacingUp()) // if going up a steep slope
            currentVelocity -= VelWithoutGravity.normalized * (slideFallOff + uphillExtraSlideFallOff) * Time.deltaTime;
        else if(onSlope)
            currentVelocity += VelWithoutGravity.normalized * downSlopeAccelMultiplyer * Time.deltaTime; // if going down slope
        else if (isActuallyGrounded)
            currentVelocity -= VelWithoutGravity.normalized * slideFallOff * Time.deltaTime; // if just on flat ground
       

        if (magnitudeToStopSlide > currentVelocityWithoutY.magnitude && timeSinceSlideStarted > timeSinceSlideBeforeAutoCancel)
        {
            currentVelocity = Vector3.zero;
            isSliding = false;
            if (slideDebug)
                print("side canceled because going to slow");

        }

        return;
    }

    // moves the camera and collider up after a slide ends
    void RaisePlayerHeight()
    {
        if (cam.transform.localPosition.y < initialCamY)
        {
            newCamPos = cam.transform.localPosition;
            newCamPos.y += camDropSpeed * Time.deltaTime;
            if (newCamPos.y > initialCamY)
                newCamPos.y = initialCamY;

            cam.transform.localPosition = newCamPos;
            timeSinceSlideStarted = 0;
        }
        if (controller.height < initialColliderSize)
        {
            controller.height += Time.deltaTime * colliderDropSpeed;

            if (controller.height > initialColliderSize)
                controller.height = initialColliderSize;
        }
    }

    void StartSlide()
    {
        slideDirection = (Camera.main.transform.right * inputVector.x + Camera.main.transform.forward * inputVector.z).normalized;

        chainSlideCheck = true;
        isSliding = true;
        slidelock = true;
        if (slideGracePeriodActive)
            currentVelocity = velocityBeforeLoosingIt;

        inputVectorAtStartOfSlide = inputVector;

        CancelDash();
    }

    // this gets the angle of the stood on ground in Degrees 
    float GroundAngle()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, controller.height))
            return 0;

        return Vector3.Angle(hit.normal, transform.up);
    }
    // This returns true if the player is walking towards an upward slope;
    bool FacingUp()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, controller.height))
            return false;
        if(slideDebug)
             print((currentVelocityWithoutY != Vector3.zero ? "Using Player Vel" : "Using Player Foward"));
        return Vector3.Dot((currentVelocityWithoutY != Vector3.zero ? currentVelocityWithoutY.normalized : transform.forward), hit.normal) < groundCheckThreshold; 
    }

    Collider[] hitColliders;
    bool CheckForCollision()
    {
        hitColliders = Physics.OverlapSphere(transform.position + colliderOffset, colliderRadius, enviromentLayermask);
        if(slideDebug)
        foreach(Collider col in hitColliders)
        {
            print(col.transform.name);
        }
        if (hitColliders.Length != 0)
        {
            print("collided with enviroment!");
            return true;
        }
        
        return false;
    }
    void SlideDebug()
    {
        DevText.DisplayInfo("onSlope", "OnSlope: " + onSlope, "Sliding");
        DevText.DisplayInfo("slidingTooSlow", "SlidingTooSlow: " + slidingTooSlow, "Sliding");
        DevText.DisplayInfo("sliding", "Sliding: " + isSliding, "Sliding");
        DevText.DisplayInfo("slideGracePeriodActive", "slideGracePeriodActive: " + slideGracePeriodActive, "Sliding");
        DevText.DisplayInfo("fasingup", "facingUpSlope: " + FacingUp(), "Sliding");
    }

    #endregion
    #region FinalMoveCaculation

    void FinalMoveCaculation()
    {
        controller.Move((currentVelocity + dashVelocity) * Time.deltaTime);
        lastPlayerVelocity = currentVelocity;
    }

    #endregion
    #region Camera

    [SerializeField] float magnitueRequiredMaxFov;
    [SerializeField] float minExtraMagnitureForFovIncrease;
    [SerializeField] float maxSprintFov;
    [SerializeField] float fovIncreaseSpeed;
    [SerializeField] float fovReturnSpeed;
    float initialCamFov;
    float newFov;

    bool increaseFov;
    void CameraFov()
    {
        increaseFov = currentVelocityWithoutY.magnitude > maxPlayerWalkSpeed.magnitude + minExtraMagnitureForFovIncrease;
 
        newFov = Mathf.Lerp(initialCamFov, maxSprintFov, currentVelocityWithoutY.magnitude / magnitueRequiredMaxFov);
        
        if (increaseFov)
        {
            if (cam.fieldOfView >= newFov)
            {
                cam.fieldOfView = newFov;
                return;
            }

            cam.fieldOfView +=  fovIncreaseSpeed * Time.deltaTime;
        }
        else
        {
            if (cam.fieldOfView <= initialCamFov)
            {
                cam.fieldOfView = initialCamFov;
                return;
            }
         
            cam.fieldOfView -= fovReturnSpeed * Time.deltaTime;
        }
    }

    #endregion

    #region Footsteps
    [Header("Footsteps")]
    [SerializeField] float distanceBetweenSteps;
    [SerializeField] AudioClip[] footsteps;

    float currentDistance;
    void Footstep()
    {
        if (!isActuallyGrounded || isSliding || currentlyDashing)
            return;

        if (inputVector == Vector3.zero)
            currentDistance -= Time.deltaTime; // this prevents hearing footsteps when doing repeated small movements

        currentDistance += Vector3.Distance(transform.position, lastPlayerSafePos);

        if (currentDistance < distanceBetweenSteps)
            return;

        currentDistance = 0;

        playerAudioSource.PlayOneShot(footsteps[UnityEngine.Random.Range(0, footsteps.Length)], GlobalSettings.audioVolume) ;
    }

    #endregion
    #region Misc
    Vector3 respawnPos;
    [Header("Respawn")]
    [SerializeField] UnityEngine.UI.Image fadeOutImage;
    [SerializeField] float fadeTimer;
    public void ResetPlayerToSafePos(Vector3 pos)
    {
        StartCoroutine(Respawn());
        if (pos == Vector3.zero)
        {
            respawnPos = lastPlayerSafePos;
            print("No player respawn point found");
        }
           
        else
            respawnPos = pos; 
    }

    IEnumerator Respawn()
    {
        controller.enabled = false;
        float timer = 0;
        Color color = fadeOutImage.color;
        currentVelocity = Vector3.zero;
        lastPlayerVelocity = Vector3.zero;
        while (timer < fadeTimer)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeTimer);
            fadeOutImage.color = color;
            yield return new WaitForEndOfFrame();
        }
        transform.position = respawnPos;
   
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeTimer);
        
            fadeOutImage.color = color;
            yield return new WaitForEndOfFrame();
        }
        controller.enabled = true;
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.75f);
        Gizmos.DrawSphere(transform.position + colliderOffset, colliderRadius);
    }

    #endregion
}
