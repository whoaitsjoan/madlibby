using UnityEngine;

//enables us to make a new PlayerData object in the project menu 
//Right click in the Project Menu -> Create -> Player -> PlayerData
//This can then be dragged onto the Player object
[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Gravity")]
    //Ideal Jump height and jumpTimeToApex will need to factor in downward force here
    [HideInInspector] public float gravityStrength;
    //This is the gravity strength set up in project settings, and what the player's gravityScale is set to
    [HideInInspector] public float gravityScale;    
    [Space(5)]
    //Adds to the player's gravity scale while falling
    public float fallGravityMult; 
    public float maxFallSpeed;
    [Space(5)]
    //Even faster falling speed if the player holds down
    public float fastFallGravityMult; 
    //Maximum fall speed while holding down for a fast fall
    public float maxFastFallSpeed; 
     
     [Space(20)]

     [Header("Run")]
     //Target max speed
     public float runMaxSpeed; 
     //Speed that player accelerates to max speed
     //Set this to runMaxSpeed for instant speed
     public float runAcceleration; 
     [HideInInspector] public float runAccelAmount; //this gets multiplied by speedDiff for the actual force applied to the player
     //Slowdown rate! Like acceleration, set to runMaxSpeed for stopping on a dime.
     public float runDecceleration; 
     //Multiplied for speedDiff for actual slowdown force when stopping
     [HideInInspector] public float runDeccelAmount; 
     [Space(5)]
     //These multipliers get added to acceleration while airborne
     [Range(0f,1)] public float accelInAir;
     [Range(0f,1)] public float deccelInAir;
     [Space(5)]
     public bool doConserveMomentum = true;

     [Space(20)]

     [Header("Jump")]
     //Full height of the player's jump
     public float jumpHeight; 
     //Time to reach max jump height. Player's gravity and jump force also controlled by this!
     public float jumpTimeToApex;
     //The actual upward force applied when jumping
     [HideInInspector] public float jumpForce;  

    //Increases gravity if the player lets go of the jump button early
     public float jumpCutGravityMult; 
     //Reduces gravity leading up to the apex jump height
    [Range(0f,1)] public float jumpHangGravityMult;
    //Extra jump hang around this speed, usually around 0 and at the jump's apex
    public float jumpHangTimeThreshold;
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Space(20)]

    [Header("Assists")]
    //The time window of still being able to jump after falling off
    [Range(0.01f, 0.5f)] public float coyoteTime;
    //A jump is automatically performed after this grace period assuming the player is able to
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;
    [Range(0.01f, 0.5f)] public float grabInputBufferTime;

    
    [Header("Grab & Throw")]
    //How long the game stops for briefly before carrying out the grab animation and other actions
    public float grabSleepTime;
    // Will come back to this after calculating throw variables

    //When the inspector updates, this is how we get our final return values
    private void OnValidate()
    {
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //This sets the rigidbody's gravityscale relative to the project
        gravityScale = gravityStrength / Physics2D.gravity.y;

        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region Variable Ranges
        //This makes sure that the numbers being calculated for starting or stopping don't take as much time to deal with
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion 
    }
}