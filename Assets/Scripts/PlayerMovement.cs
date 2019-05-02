using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    public float jumpAccel;
    public float moveAccel;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private AudioSource[] audioComponents;
    public float raySize;
    public float speedMargin;
    [HideInInspector] public bool isGrounded;
    public AudioClip attackSound1;
    public AudioClip attackSound2;
    public AudioClip jumpSound;
    private Timer jumpTimer;
    private bool readyingJump;
    public float waitPercentToJump;
    public float landingRaySize;
    [HideInInspector] public bool canControlPlayer;
    [HideInInspector] public Timer autoMoveTimer;
    [HideInInspector] public Vector2 autoMoveDirection;
    public float autoMoveTime;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        audioComponents =  GetComponents<AudioSource>();
        jumpTimer = new Timer(1.0f);
        jumpTimer.turnOff();
        autoMoveTimer = new Timer(autoMoveTime);
        canControlPlayer = true;
    }

    public void flipSpriteXToNormal() {
        spriteRenderer.flipX = false;
        
    }

    public void flipSprite() {
        if (rigidBody.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        if (rigidBody.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
    
    public void turnOffJumpTimer() {
        jumpTimer.turnOff();
    }

    void Update() {
        bool isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("player_idle");
        
        ////Update the animation variable
        /// 
        /// 
        /// 
        if(!isIdle) {
            if (rigidBody.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }

            if (rigidBody.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
        animator.SetFloat("run_speed", rigidBody.velocity.x);
        
        bool running = animator.GetCurrentAnimatorStateInfo(0).IsName("player_run");
        
        // animator.speed = 1.0f;
        // if (running)
        // {
        //     float maxVal = 18.0f;
        //     float absVel = Mathf.Min(Mathf.Abs(rigidBody.velocity.x), maxVal);
            
            
        //     animator.speed = Mathf.Max((1.0f - (absVel / maxVal)) * 1.0f, 0.9f);
        //     // animator.speed = 0.6f;
            
        // } else if(isIdle) {
        //     animator.speed = 0.0f;
            
        // } 
        if (Mathf.Abs(rigidBody.velocity.x) > 0.1f && !audioComponents[1].isPlaying)
        {
          audioComponents[1].Play();
        } 
        
        if((Input.GetButtonDown("Jump") && isGrounded && canControlPlayer)) {
            if (!jumpTimer.isOn())
            {
                animator.SetTrigger("jump");
                audioComponents[0].PlayOneShot(jumpSound);
                jumpTimer.turnOn();
                readyingJump = true;
                
            }
            
        }

        bool isFallingAnim = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_falling");
        if(!isGrounded && isFallingAnim) {
            bool isAboutToLand = false;
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(boxCollider.bounds.center, Vector2.down, 0.5f*boxCollider.size.y + landingRaySize);
            for(int i = 0; i < hits.Length; ++i) {
                RaycastHit2D hit = hits[i];
                if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger && hit.collider.gameObject.transform.parent != gameObject) {
                    isAboutToLand = true;
                    break;
                }
            }
            // Debug.DrawLine(boxCollider.bounds.center, boxCollider.bounds.center + (0.5f*boxCollider.size.y + landingRaySize)*Vector3.down);
            // Debug.Log("landing: " + animator.GetBool("landing"));
            // Debug.Log("velocity less: " + (rigidBody.velocity.y < 0));
            // Debug.Log("about to land: " + isAboutToLand);
            if(!animator.GetBool("landing") && rigidBody.velocity.y < 0 && isAboutToLand) {
                // Debug.Log("setLanding trigger");
                animator.SetTrigger("landing");
            }
        }
        
    }
    
    
    // This is for the physics 
    void FixedUpdate()
    {
        Vector2 movementForce = new Vector2(0, 0);
        if(canControlPlayer) {
            movementForce.x = Input.GetAxis("Horizontal");
        } else if(autoMoveTimer.isOn()) {
            bool finished = autoMoveTimer.updateTimer(Time.fixedDeltaTime);
            //the automovedirection should only be between -1 && 1
            movementForce = autoMoveDirection;
            if(finished) {
                autoMoveTimer.turnOff();
            }
        }

        float thisJmpAccel = jumpAccel;
        
        //NOTE(ollie): This is the jump timer so you get a nice jump. We propotion out the jump force over the jump
        if(jumpTimer.isOn()) {
            bool fin = jumpTimer.updateTimer(Time.fixedDeltaTime);
            if (!fin && jumpTimer.getCanoncial() < waitPercentToJump) { //has to be before the jump timer is finished 
                //wait till over a percentage
                
            } else {
                
                movementForce.y = 1;
                
                thisJmpAccel = Mathf.Lerp(jumpAccel, 0, jumpTimer.getCanoncial());
                if (fin)
                {
                    jumpTimer.turnOff();
                    
                }
            }
            
        } 
         
        movementForce.x *= moveAccel;
        movementForce.y *= thisJmpAccel;
        rigidBody.AddForce(movementForce);
        
    }
}
