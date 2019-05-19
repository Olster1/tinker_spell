using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyGameManager;
using UnityEngine.SceneManagement;
using EasyAttackObjectCreator;
using ConfigControls_namespace;
using EasyForceUpdator;

public class PlayerMovement : MonoBehaviour, IHitBox
{
    private Rigidbody2D rigidBody;
    public float jumpAccel;
    public float moveAccel;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private BoxCollider2D jumpBoxCollider;
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
    
    public float attackForceUp;
    public float downwardAttackForce;

    public GameObject genericAttackObject;
    public GameObject damageNumbersObject;
    public GameObject earthAttackObject;

    private Vector2 originalBoxSize;
    private Vector2 originalBoxOffset;
    private Vector2 originalJumpOffset;


    private ForceUpdator forceUpdator;

        

    public Vector2 earthOffset;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        jumpBoxCollider = transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        audioComponents =  GetComponents<AudioSource>();
        jumpTimer = new Timer(1.0f);
        jumpTimer.turnOff();
        autoMoveTimer = new Timer(autoMoveTime);
        canControlPlayer = true;

        forceUpdator = new ForceUpdator();

        
    }

    public void CreateUpwardAttackObject() {
        ForceToAddStruct force = new ForceToAddStruct(0.2f, attackForceUp*Vector2.up);
        forceUpdator.AddForce(force);
        
        GameObject attackObj = Instantiate(genericAttackObject, transform);

        Vector2 startPos = (Vector2)boxCollider.bounds.center + new Vector2(0, 0.5f*boxCollider.size.y + 0.1f);        
        Vector2 localStartPos = (Vector2)transform.InverseTransformPoint(startPos);
        AttackObjectCreator.initAttackObject(attackObj, localStartPos, localStartPos, 
                    EnemyType.ENEMY_GOOD, 0.5f, 22, 36);

    }

    public void CreateSidewardAttack() {
        float signOfMovement = Mathf.Sign(rigidBody.velocity.x);

        ForceToAddStruct force = new ForceToAddStruct(0.1f, Vector2.right*signOfMovement*attackForceUp);
        forceUpdator.AddForce(force);
        
        GameObject attackObj = Instantiate(genericAttackObject, transform);

        Vector2 startPos = (Vector2)boxCollider.bounds.center + new Vector2(signOfMovement*(0.5f*boxCollider.size.y + 0.1f), 0);        
        Vector2 localStartPos = (Vector2)transform.InverseTransformPoint(startPos);
        AttackObjectCreator.initAttackObject(attackObj, localStartPos, localStartPos, 
                    EnemyType.ENEMY_GOOD, 0.5f, 22, 36);

    }

    public void downwardStrike() {
        float signOfMovement = Mathf.Sign(rigidBody.velocity.x);
        Vector2 directionAttack = new Vector2(signOfMovement*0.707f, -0.707f);

        ForceToAddStruct force = new ForceToAddStruct(0.35f, downwardAttackForce*directionAttack);
        forceUpdator.AddForce(force);

        GameObject attackObj = Instantiate(genericAttackObject, transform);

        Vector2 startPos = (Vector2)boxCollider.bounds.center + new Vector2(signOfMovement*0.5f*boxCollider.size.x + signOfMovement*0.1f, 0.5f*boxCollider.size.y + 0.1f);        
        Vector2 localStartPos = (Vector2)transform.InverseTransformPoint(startPos);
        AttackObjectCreator.initAttackObject(attackObj, localStartPos, localStartPos, 
                    EnemyType.ENEMY_GOOD, 0.5f, 22, 36);

        originalBoxSize = boxCollider.size;

        originalBoxOffset = boxCollider.offset;
        
        originalJumpOffset = jumpBoxCollider.offset;


        Vector2 tempSize = boxCollider.size;
        tempSize.y = 2;
        boxCollider.size = tempSize;

        Vector2 tempOffset = boxCollider.offset;
        tempOffset.y = 1;
        boxCollider.offset = tempOffset;

        tempOffset = jumpBoxCollider.offset;
        tempOffset.y = -1;
        jumpBoxCollider.offset = tempOffset;

    }

    public void endDownwardStrike() {
        boxCollider.size = originalBoxSize;

        boxCollider.offset = originalBoxOffset;
        
        jumpBoxCollider.offset = originalJumpOffset;


    }

    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        // bool isHit = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");
       if (enemyType == EnemyType.ENEMY_EVIL) 
       {
           GameObject damageNumObj = Instantiate(damageNumbersObject,  transform);
           DamageNumber damageNum = damageNumObj.GetComponent<DamageNumber>();
           damageNum.initializeObject(damage, type);

           Vector2 dir = (Vector2)transform.position - position;
           dir.Normalize();


           ForceToAddStruct force = new ForceToAddStruct(0.1f, 1000*dir);

           // thisAnimator.SetTrigger("WasHit");
           GameManager.playerHealth -= damage;
           
           //Instantiate(hitParticleSystem);

           if (GameManager.playerHealth < 0)
           {
            GameManager.playerHealth = 100;
             // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
               //  thisAnimator.SetTrigger("isDead");
               // //  thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rock_gollum_die");
               // // this.deathTimer = new Timer_namespace.Timer(1.0f);
               // // this.deathTimer.turnOn();
               //  isDying = true;

           }
       }
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

        bool isInAttackAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_attack2") || animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_attack1") || animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_downward_dash") || animator.GetBool("attack1") || animator.GetBool("attack2") || animator.GetBool("downward_dash");
        bool isFallingAnim = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_falling");
        
        if (!Input.GetButton(ConfigControls.SPELLS_TRIGGER_BTN) && !isInAttackAnimation && canControlPlayer)
        {
            if(Input.GetButtonDown("Fire2") && isGrounded) {
                Instantiate(earthAttackObject, transform.position - new Vector3(earthOffset.x, earthOffset.y, 0),  Quaternion.identity);
            } else if(Input.GetButtonDown("Fire1")) {
                float xMove = Input.GetAxis("Horizontal");
                float yMove = Input.GetAxis("Vertical");
                bool isHorizontal = xMove != 0.0f;
                bool isVertical = Mathf.Abs(yMove) > Mathf.Abs(xMove);
                // audiosrc.PlayOneShot(attackSound);
                if(!isGrounded && isFallingAnim && isHorizontal && (Mathf.Abs(xMove) > Mathf.Abs(yMove))) {
                    animator.SetTrigger("downward_dash");
                } else if(!isGrounded) {
                    animator.SetTrigger("attack1"); 
                } else if (isVertical) {
                    animator.SetTrigger("attack1");
                } else if(isHorizontal) {
                    animator.SetTrigger("attack2");
                } else {
                    animator.SetTrigger("attack1"); //don't yet have stationary attack
                }
            }
        }

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
          // audioComponents[1].Play();
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


        Vector2 f = forceUpdator.update();
         
        movementForce.x *= moveAccel;
        movementForce.y *= thisJmpAccel;
        rigidBody.AddForce(movementForce + f);
    }
}
