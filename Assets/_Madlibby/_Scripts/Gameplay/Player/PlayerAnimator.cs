using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerMovement mov;
    private Animator anim;
    private SpriteRenderer spriteRend;
    //this is primarily for the color of the particle FX
    public Color CurrentForegroundColor;

    
    [Header("Movement Tilt")]
    [SerializeField] private float maxTilt;
    [SerializeField] [Range(0, 1)] private float tiltSpeed;

    [Header("Particle FX")]
    //These are some fun effects that seemed pretty basic to implement, so I figured I'd try it!
    [SerializeField] private GameObject jumpFX;
    [SerializeField] private GameObject landFX;
    private ParticleSystem _jumpParticle;
    private ParticleSystem _landParticle;

//These three trigger specific animations once variables have changed from PlayerMovement
    public bool startedJumping { private get; set; }
    public bool justLanded { private get; set; }
    public bool startedGrabbing { private get; set; }
    public bool startedThrowing { private get; set; }
     
    public float currentVelY;

     private void Start()
     {
        mov = GetComponent<PlayerMovement>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        anim = spriteRend.GetComponent<Animator>();

        _jumpParticle = jumpFX.GetComponent<ParticleSystem>();
        _landParticle = landFX.GetComponent<ParticleSystem>();
     }

     private void LateUpdate()
     {
        //This section PROBABLY wont be useful after our character's not a square anymore, but who knows!
        #region Tilt
        float tiltProgress;

        int mult = -1;

        tiltProgress = Mathf.InverseLerp(-mov.Data.runMaxSpeed, mov.Data.runMaxSpeed, mov.Rb.linearVelocity.x);
        mult = (mov.IsFacingRight) ? 1 : -1;

        float newRot = ((tiltProgress * maxTilt * 2) - maxTilt);
        float rot = Mathf.LerpAngle(spriteRend.transform.localRotation.eulerAngles.z * mult, newRot, tiltSpeed);
        spriteRend.transform.localRotation = Quaternion.Euler(0,0, rot * mult);
        #endregion

        CheckAnimationState();

        ParticleSystem.MainModule jumpPSettings = _jumpParticle.main;
        jumpPSettings.startColor = new ParticleSystem.MinMaxGradient(CurrentForegroundColor);
        ParticleSystem.MainModule landPSettings = _landParticle.main; 
        landPSettings.startColor = new ParticleSystem.MinMaxGradient(CurrentForegroundColor);
     }

     private void CheckAnimationState()
     {
        if (startedJumping)
        {
            anim.SetTrigger("Jump");
            GameObject obj  = Instantiate(jumpFX, transform.position - (Vector3.up * transform.localScale.y / 2), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            startedJumping = false;
            return;
        }

        if (justLanded)
        {
            anim.SetTrigger("Land");
            GameObject obj = Instantiate(landFX, transform.position - (Vector3.up * transform.localScale.y / 1.5f), Quaternion.Euler(-90, 0, 0));
            Destroy (obj, 1);
            justLanded = false;
            return;
        }

        if (startedGrabbing)
        {
         anim.SetTrigger("Grab");
         //Should there be a separate particle effect for when Libby grabs something? Unclear, leaving a comment for now
         startedGrabbing = false;
         return;
        }

        if (startedThrowing)
        {
         anim.SetTrigger("Throw");
         startedThrowing = false;
         return;
        }

        anim.SetFloat("Vel Y", mov.Rb.linearVelocity.y);
     }
}