using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
//This scriptable object is holding all of the movement parameters for the player in a format that's adjustable
//We can fine tune the specific numbers for polish once we're happy with the basic functionality
public PlayerData Data;

#region COMPONENTS

public Rigidbody2D Rb {get; private set;}

//This playeranimator script (also referenced in that script) handles player animations!
//This can absolutely be reworked/overhauled if there's a better method, animations are not my specialty
public PlayerAnimator Animator {get; private set; }
//This is largely for use later with grabbing parts of movement
private GameObject grabbedObject;
#endregion

#region STATE CHECKS
//Publicly readable variables for other scripts that control what the player can do at a given time.
//These are privately written to so they can't be messed with otherwise.

public bool IsFacingRight {get; private set;}
public bool IsJumping {get; private set;}
public bool IsGrabbing {get; private set;}

//This timer serves the same purpose but can also return a bool
public float LastOnGroundTime {get; private set;}

//Checks that tweak falling speed based on player input
private bool _isJumpCut;

private bool _isJumpFalling;

#endregion

#region INPUT PARAMETERS

private Vector2 _moveInput;
public float LastPressedJumpTime {get; private set;}

public float LastPressedGrabTime {get; private set;}

#endregion

#region CHECK PARAMETERS
//These are all of the flags for confirming if Libby's on the ground, over an object/enemy, holding something
[Header("Checks")]
[SerializeField] private Transform _groundCheckPoint;
//groundCheck here will largely depend on the final character size
[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
[Space (5)]
[SerializeField] private Transform _objectCheckPoint;

[SerializeField] private Vector2 _objectCheckSize = new Vector2 (0.49f, 0.03f);

//The point where word blocks or enemies will be held above Libby's head
[SerializeField] private Transform _holdSpotPoint;
//This last variable may end up being unneccesary but I'm setting it up just in case
[SerializeField] private Vector2 _holdSpotSize = new Vector2(2.77f, 0.77f);

#endregion

#region LAYERS & TAGS
[Header ("Layers & Tags")]
[SerializeField] private LayerMask _groundLayer;
//Assuming we want to grab blocks and enemies, this layer will work for both
[SerializeField] private LayerMask _objectLayer;
//One more separate layer tracking anything currently being held
[SerializeField] private LayerMask _pickUpLayer;

#endregion 

private void Awake()
{
    Rb = GetComponent<Rigidbody2D>();
    Animator = GetComponent<PlayerAnimator>();
}

private void Start()
{
    SetGravityScale(Data.gravityScale);
}

private void Update()
{
    #region TIMERS
    LastOnGroundTime -= Time.deltaTime;
    LastPressedJumpTime -= Time.deltaTime;
    LastPressedGrabTime -= Time.deltaTime;
    #endregion

    #region INPUT HANDLER
    _moveInput.x = Input.GetAxisRaw("Horizontal");
    _moveInput.y = Input.GetAxisRaw ("Vertical");

    if (_moveInput.x > 0)
        CheckDirectionToFace(_moveInput.x > 0);

    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C))
    {
        OnJumpInput();
    }

    if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C))
    {
        OnJumpUpInput();
    }
    
    if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.LeftShift))
    {
        OnGrabInput();
    }
    
    #endregion
    
    #region COLLISION CHECKS
    if (!IsJumping)
    {
        //checking if the player is colliding with the ground? Or on top of an object/enemy?
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) || Physics2D.OverlapBox(_objectCheckPoint.position, _objectCheckSize, 0, _objectLayer))
        {
            if(LastOnGroundTime < -0.1f)
            {
                Animator.justLanded = true;
            }
            //if not, give the player however much coyote time we've set
            LastOnGroundTime = Data.coyoteTime;
        }
        //will need a second collision check for hurt box, should it go in this script? unsure!
    }
    #endregion

    #region JUMP CHECKS
    if (IsJumping && Rb.linearVelocity.y < 0)
    {
        IsJumping = false;

        _isJumpFalling = true;
    }

    if (LastOnGroundTime > 0 && !IsJumping)
    {
        _isJumpCut = false;

        _isJumpFalling = false;
    }

     //Single Jump function is called here, to not able able to spam the button
    if (CanJump() && LastPressedJumpTime > 0)
    {
         IsJumping = true;
        _isJumpCut = false;
        _isJumpFalling = false;
        Jump();
        //if the player is actually grabbing something,
        //we can just apply extra force in the Jump method
        Animator.startedJumping = true;
    }
        
    
    #endregion
    
    #region GRAB CHECKS
    //This will need to change if Libby is also able to grab enemies/objects from the sides
    //But wanted to make sure this worked for now
    if (CanGrab() && !IsGrabbing && LastPressedGrabTime > 0 && Physics2D.OverlapBox(_objectCheckPoint.position, _objectCheckSize, 0, _objectLayer))
    {
        Collider2D pickUp = Physics2D.OverlapBox(_objectCheckPoint.position, _objectCheckSize, 0, _objectLayer);
        if (pickUp)
        {
            //This might not work as an effect but could be nice for some extra emphasis?
            Sleep(Data.grabSleepTime);
            Animator.startedGrabbing = true;
            grabbedObject = pickUp.gameObject;

            //Might need a conditional here for movement effects while grabbing
            IsGrabbing = true;
            IsJumping = false;
            _isJumpCut = false;
            _isJumpFalling = true;

            //This also may make more sense as its own class!
            Grab(grabbedObject);
        }
    }
    /*will return to this later for finalizing throw details
    else if (IsGrabbing && LastPressedGrabTime > 0 && Physics2D.OverlapCircle(_holdSpotPoint.position, _holdSpotSize, .4f, _pickUpLayer))
    {
        
        IsGrabbing = false;
        Animator.startedThrowing = true;
        StartCoroutine(nameof(Throw), grabbedObject);
    }
    */
    #endregion
    

    #region GRAVITY
    if (Rb.linearVelocity.y < 0 && _moveInput.y < 0)
    {
        //Setting a faster fall by holding down
        SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
        //But not too fast if there's a big drop, setting a cap
        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, Mathf.Max(Rb.linearVelocity.y, -Data.maxFastFallSpeed));
    }
    else if (_isJumpCut)
    {
        //If jump is released early, then Libby will fall faster
        SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, Mathf.Max(Rb.linearVelocity.y, -Data.maxFallSpeed));
    }
    else if((IsJumping || _isJumpFalling) && Mathf.Abs(Rb.linearVelocity.y) < Data.jumpHangTimeThreshold)
    {
        SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
    }
    else if (Rb.linearVelocity.y < 0)
    {
        //Libby will also fall faster when falling
        SetGravityScale(Data.gravityScale * Data.fallGravityMult);
        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, Mathf.Max(Rb.linearVelocity.y, -Data.maxFallSpeed));
    }
    else
    {
        //Just standing on regular ground or moving on it will set gravity to normal
        SetGravityScale(Data.gravityScale);
    }
    #endregion
}

private void FixedUpdate()
{
    Run(1);

    //may need a variation of this for grabbing
}

#region INPUT CALLBACKS
//These methods are run when input is detected in Update()
public void OnJumpInput()
{
    LastPressedJumpTime = Data.jumpInputBufferTime;
}

public void OnJumpUpInput()
{
    if (CanJumpCut())
        _isJumpCut = true;  
}

public void OnGrabInput()
{
    LastPressedGrabTime = Data.grabInputBufferTime;
}
#endregion

//These methods get called around a few times and are general use
#region GENERAL METHODS
public void SetGravityScale(float scale)
{
    Rb.gravityScale = scale;
}

private void Sleep(float duration)
{
    //Rather than using StartCoroutine everytime for pauses, Sleep will make this easier
    //Also will make troubleshooting easier, if it comes up
    StartCoroutine(nameof(PerformSleep), duration);
}

private IEnumerator PerformSleep(float duration)
{
    Time.timeScale = 0;
    //checking for real seconds since the game world is stopped now
    yield return new WaitForSecondsRealtime(duration); 
    Time.timeScale = 1;
}
#endregion

//These methods handle the finessing of platformer movement
#region RUN METHODS
private void Run(float lerpAmount)
{
    //Desired velocity and direction we're moving in
    float targetSpeed = _moveInput.x * Data.runMaxSpeed;
    //Using Lerp to smooth changes out over time
    targetSpeed = Mathf.Lerp(Rb.linearVelocity.x, targetSpeed, lerpAmount);

    #region Acceleration
    float accelRate;

    //For acclerating, turning, decelerating, and a multiplier while in the air
    if (LastOnGroundTime > 0)
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
    else
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
    #endregion

    #region Jump Apex Acceleration
    //This increases acceleration and maxSpeed when at the apex of a jump
    if ((IsJumping || _isJumpFalling) && Mathf.Abs(Rb.linearVelocity.y) < Data.jumpHangTimeThreshold)
    {
        accelRate *= Data.jumpHangAccelerationMult;
        targetSpeed *= Data.jumpHangMaxSpeedMult;
    }
    #endregion

    #region Conserve Momentum
    //This makes sure that players don't get slowed down if they move faster than their max speed in a direction
    if(Data.doConserveMomentum && Mathf.Abs(Rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(Rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        accelRate = 0;
    #endregion

    //The difference between current and desired velocity needs to be calculated
    float speedDif = targetSpeed - Rb.linearVelocity.x;

    float movement = speedDif * accelRate;

    //This can then be applied to the rigidbody for smooth movement!
    Rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

    //Calling this method via FixedUpdate should have it run roughly 50 times a second
}

private void Turn()
{
    //Keeps momentum and scale when turning, just flipping it
    Vector3 scale = transform.localScale;
    scale.x *= -1;
    transform.localScale = scale;

    IsFacingRight = !IsFacingRight;
}
#endregion

#region JUMP METHOD
private void Jump()
{
    //Like mentioned before, this makes it so we can't jump multiple times from one press
    LastPressedJumpTime = 0;
    LastOnGroundTime = 0;

    
    //Increasing the force while falling to make sure there's as much jump height overall
    float force = Data.jumpForce;
    if (Rb.linearVelocity.y < 0)
        force -= Rb.linearVelocity.y;
    
    Rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    
}
#endregion

#region GRAB METHODS

private void Grab(GameObject grabbed)
{
    //moving the grabbed item to go over Libby's head
    grabbed.transform.position = _holdSpotPoint.position;
    grabbed.transform.parent = transform;
    grabbed.layer = 8;
    //Making sure the item moves with the player and doesn't have collision/physics while doing so
    if (grabbed.GetComponent<Rigidbody2D>())
        grabbed.GetComponent<Rigidbody2D>().simulated = false;
}

/* 
private void Throw(GameObject thrown)
{

}
*/
#endregion

#region CHECK METHODS

public void CheckDirectionToFace(bool isMovingRight)
{
    if (isMovingRight != IsFacingRight)
        Turn();
}

private bool CanJump()
{
    return LastOnGroundTime > 0 && !IsJumping;
}

private bool CanJumpCut()
{
    return IsJumping && Rb.linearVelocity.y > 0;
}

private bool CanGrab()
{
    return LastOnGroundTime > 0 && !IsGrabbing;
}
#endregion

}