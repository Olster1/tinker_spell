using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyGameManager;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;
using EasyAttackObjectCreator;
using ConfigControls_namespace;
using EasyForceUpdator;

public class PlayerMovement : MonoBehaviour, IHitBox
{

    public enum IdleAnimation {
        ANIMATION_IDLE1, 
        ANIMATION_IDLE2, 
    }

    private Rigidbody2D rigidBody;
    public float jumpAccel;
    public float moveAccel;
    public float moveWhileJumpAccel;
    private Animator animator;

    [HideInInspector] public Timer earthTimer;
    [HideInInspector] public Timer waterTimer;
    [HideInInspector] public Timer fireTimer;

    public ParticleSystem ps;

    public GameObject camera;

    public Image fadePanel;

    public Animator panelAnimator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public Vector3[] lastValidPos;
    private Timer validPosTimer;
    private int validPosIndexAt;
    private int lastValidPosTop;

    public AudioSource landingSoundSource;
    public AudioSource regularAttackSound;
    public AudioSource uppercutAttackSound;
    public AudioSource jumpAudioSrc;

    public AudioSource attackVoice;
    public AudioClip[] attackVoiceClips;
    public float reboundForce;

    public float raySize;
    public float speedMargin;
    [HideInInspector] public bool isGrounded;

    private Timer jumpTimer;
    // private bool readyingJump;
    public float waitPercentToJump;
    public float landingRaySize;
    [HideInInspector] public bool canControlPlayer;
    [HideInInspector] public Timer autoMoveTimer;
    [HideInInspector] public Vector2 autoMoveDirection;
    public float autoMoveTime;

    public float jumpRaySize;

    public AnimatorOverrideController[] idleAnimations;
    private Timer idleAnimationTimer;
    private bool swapAnimation;
    private IdleAnimation toSwapTo;
    
    private int physicsLayerMask;

    public float attackForceUp;
    public float downwardAttackForce;

    public GameObject genericAttackObject;
    public GameObject damageNumbersObject;
    public GameObject earthAttackObject;

    private Vector2 originalBoxSize;
    private Vector2 originalBoxOffset;
    private Vector2 originalJumpOffset;
    public float timeIncrease;


    private ForceUpdator forceUpdator;

    public Vector2 earthOffset;
    [HideInInspector] public Timer globalPauseTimer;
    [HideInInspector] public Timer dieTimer;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        jumpTimer = new Timer(0.5f);
        jumpTimer.turnOff();
        autoMoveTimer = new Timer(autoMoveTime);
        canControlPlayer = true;

        idleAnimationTimer = new Timer(10.0f);

        forceUpdator = new ForceUpdator();

        physicsLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);

        swapAnimation = false;
        toSwapTo = IdleAnimation.ANIMATION_IDLE1;

        globalPauseTimer = new Timer(0.1f);
        dieTimer = new Timer(0.7f);
        dieTimer.turnOff();
        ParticleSystem.MainModule main = ps.main;
        main.useUnscaledTime = true;

        earthTimer = new Timer(1.5f);
        earthTimer.turnOff();
        waterTimer = new Timer(0.5f);
        waterTimer.turnOff();
        fireTimer = new Timer(0.5f);
        fireTimer.turnOff();

        lastValidPos = new Vector3[16];
        validPosTimer = new Timer(0.1f);

        Time.timeScale = 1.0f;
        
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

    public void checkSwap() {
        if(swapAnimation) {
            Debug.Log("Swapped animation");
            Assert.IsTrue((int)toSwapTo < idleAnimations.Length);
            AnimatorOverrideController newController = idleAnimations[(int)toSwapTo];
            animator.runtimeAnimatorController = newController;
        }
    }

    public bool CastGroundedRay(Vector3 colliderOffset) {
        Vector3 bottomCenter = boxCollider.bounds.center + new Vector3(0, -0.5f*boxCollider.size.y, 0);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(bottomCenter + colliderOffset, Vector2.down, jumpRaySize, physicsLayerMask);
        for(int i = 0; i < hits.Length; ++i) {
            RaycastHit2D hit = hits[i];
            if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) {
                if(hit.collider.gameObject.tag == "Platform" && rigidBody.velocity.y > 0) {

                } else {
                    isGrounded = true;
                    break;    
                }
                
            } 
        }
        return isGrounded;
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

    public void scaleUp() {
        Vector3 tempScale = transform.localScale;
        tempScale.x = 1.5f;
        tempScale.y = 1.5f;
        transform.localScale = tempScale;
    }

    public void scaleDown() {
        Vector3 tempScale = transform.localScale;
        tempScale.x = 1.0f;
        tempScale.y = 1.0f;
        transform.localScale = tempScale;
    }

    public void downwardStrike() {
        float signOfMovement = Mathf.Sign(rigidBody.velocity.x);
        Vector2 directionAttack = new Vector2(signOfMovement*0.707f, -0.707f);

        ForceToAddStruct force = new ForceToAddStruct(0.1f, downwardAttackForce*directionAttack);
        forceUpdator.AddForce(force);

        GameObject attackObj = Instantiate(genericAttackObject, transform);

        Vector2 startPos = (Vector2)boxCollider.bounds.center + new Vector2(signOfMovement*0.5f*boxCollider.size.x + signOfMovement*0.1f, 0.5f*boxCollider.size.y + 0.1f);        
        Vector2 localStartPos = (Vector2)transform.InverseTransformPoint(startPos);
        AttackObjectCreator.initAttackObject(attackObj, localStartPos, localStartPos, 
                    EnemyType.ENEMY_GOOD, 1.0f, 22, 36);

        originalBoxSize = boxCollider.size;

        originalBoxOffset = boxCollider.offset;
        

        Vector2 tempSize = boxCollider.size;
        tempSize.y = 2;
        boxCollider.size = tempSize;

        Vector2 tempOffset = boxCollider.offset;
        tempOffset.y = 1;
        boxCollider.offset = tempOffset;

        // tempOffset = jumpBoxCollider.offset;
        // tempOffset.y = -1;
        // jumpBoxCollider.offset = tempOffset;

    }

    public void endDownwardStrike() {
        boxCollider.size = originalBoxSize;

        boxCollider.offset = originalBoxOffset;
        
        // jumpBoxCollider.offset = originalJumpOffset;
        Vector3 tempPos = transform.position;
        tempPos.y += 2;
        transform.position = tempPos;


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


           ForceToAddStruct force = new ForceToAddStruct(0.1f, reboundForce*dir);
           forceUpdator.AddForce(force);
           // thisAnimator.SetTrigger("WasHit");
           GameManager.playerHealth -= damage;
           GameManager.updateHealth = true;
           Time.timeScale = 0.0f;
           globalPauseTimer.turnOn();
           ps.Play();
           
           //Instantiate(hitParticleSystem);

           if (GameManager.playerHealth < 0)
           {
                Time.timeScale = 0.0f;
                GameManager.playerHealth = 100;
                GameManager.updateHealth = true;
           }
       }
    }

    public void ResetPositionFromFall() {
        rigidBody.simulated = true;
        int index = validPosIndexAt + 1;
        if(index >= lastValidPos.Length || lastValidPosTop < lastValidPos.Length) { //if at the end of the buffer or the buffer isn't filled yet
            index = 0;
        } 
        transform.position = lastValidPos[index];
        rigidBody.velocity = Vector2.zero;
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, camera.transform.position.z);
    }

    public void Die() {
        panelAnimator.SetTrigger("FadeIn");
        panelAnimator.SetTrigger("FadeFromFall");
        rigidBody.simulated = false;
        
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }


    public void flipSpriteXToNormal() {
        // spriteRenderer.flipX = false;
        
    }

    

    public void flipSprite() {
        float i = Input.GetAxis("Horizontal");
        if(Mathf.Abs(i) > 0.25f) {
            if(i > 0) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }
        } else {
            if (rigidBody.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }

            if (rigidBody.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
    
    public void turnOffJumpTimer() {
        jumpTimer.turnOff();
    }

    void playAttackVoice() {
        Assert.IsTrue(attackVoiceClips.Length > 0);

        int randIndex = (int)Random.Range(0, attackVoiceClips.Length);
        if(randIndex == attackVoiceClips.Length) {
            randIndex = attackVoiceClips.Length - 1;
        }
        attackVoice.clip = attackVoiceClips[randIndex];
        attackVoice.volume = 0.7f;
        // attackVoice.pitch = (float)Random.Range(0.8f, 1.2f);
        attackVoice.Play();
        
    }

    void Update() {

        //this is the global pause timer
        if(globalPauseTimer.isOn()) {
            bool fin = globalPauseTimer.updateTimer(Time.unscaledDeltaTime);
            // Time.timeScale = Mathf.Lerp(0.0f, 0.3f, globalPauseTimer.getCanoncial());
            if(fin) {
                globalPauseTimer.turnOff();
                Time.timeScale = 1.0f;
            }
        }

        if(dieTimer.isOn()) {
            bool fin = dieTimer.updateTimer(Time.unscaledDeltaTime);
            fadePanel.color = new Color(0, 0, 0, 1.0f - dieTimer.getCanoncial());
            // Time.timeScale = Mathf.Lerp(0.0f, 0.3f, dieTimer.getCanoncial());
            if(fin) {
                dieTimer.turnOff();
                SceneManager.LoadScene("LoadingScreen");
            }
        }

        bool isJumping = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_jump");

        bool lastFameGrounded = isGrounded;
        isGrounded = false;
        // if(rigidBody.velocity.y <= 0) 
        {
            Vector3 halfCollider = new Vector3(0.5f*boxCollider.size.x, 0, 0);
            if(!CastGroundedRay(new Vector3(0, 0, 0))) {
                if(!CastGroundedRay(-halfCollider)) {
                    if(!CastGroundedRay(halfCollider)) {
                    }       
                }                
            }
        }

        animator.SetBool("grounded", isGrounded);
        
        if(lastFameGrounded != isGrounded && !isJumping) {
            landingSoundSource.Play();

        }

        

        bool isInAttackAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_attack2") || animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_attack1") || animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_downward_dash") || animator.GetBool("attack1") || animator.GetBool("attack2") || animator.GetBool("downward_dash");
        bool isFallingAnim = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_falling");
        
        if (!isInAttackAnimation && canControlPlayer)
        {
            //get direction of the controller
            float xMove = Input.GetAxis("Horizontal");
            float yMove = Input.GetAxis("Vertical");

            if(Input.GetButton(ConfigControls.SPELLS_TRIGGER_BTN)) {
                //MAGIC MOVES 
                if(Input.GetButtonDown("Fire2") && isGrounded && GameManager.hasEarth1 && !earthTimer.isOn()) {
                    float xDir = Mathf.Sign(xMove);
                    earthTimer.turnOn();
                    GameObject earthObj = Instantiate(earthAttackObject, transform.position - new Vector3(xDir*earthOffset.x, earthOffset.y, 0),  Quaternion.identity);
                    earthObj.GetComponent<SpriteRenderer>().flipX = Mathf.Sign(xMove) < 0;
                    
                    if(xDir < 0) {
                        earthObj.GetComponent<EarthAttack>().rePosBoxes();    
                    }
                    
                    GenericAttackObject earthAttack = earthObj.transform.GetChild(0).gameObject.GetComponent<GenericAttackObject>();
                    earthAttack.startPos.x *= xDir;
                    earthAttack.endPos.x *= xDir;
                    earthAttack.attackType = "earth";
                    
                } 
            } else {
                
                if(Input.GetButtonDown("Fire1")) {
                    
                    bool isHorizontal = xMove != 0.0f;
                    bool isVertical = Mathf.Abs(yMove) > Mathf.Abs(xMove);
                    // audiosrc.PlayOneShot(attackSound);
                    if(!isGrounded && isFallingAnim && isHorizontal && (Mathf.Abs(xMove) > Mathf.Abs(yMove))) {
                        animator.SetTrigger("downward_dash");
                    } else if(!isGrounded) {
                        animator.SetTrigger("attack1"); 
                        uppercutAttackSound.Play();
                        playAttackVoice();
                    } else if (isVertical) {
                        animator.SetTrigger("attack1");
                        uppercutAttackSound.Play();
                        playAttackVoice();
                    } else if(isHorizontal) {
                        regularAttackSound.Play();
                        animator.SetTrigger("attack2");
                        playAttackVoice();
                    } else {
                        animator.SetTrigger("attack1"); //don't yet have stationary attack
                        uppercutAttackSound.Play();
                        playAttackVoice();
                    }
                }
            }
        }

        bool isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("player_idle");
        
        ////Update the animation variable
        /// 
        /// 
        /// 
        if(!isIdle) {
            float i = Input.GetAxis("Horizontal");
            if(Mathf.Abs(i) > 0.25f) {
                if(i > 0) {
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
            } else {
                if (rigidBody.velocity.x > 0)
                {
                    spriteRenderer.flipX = false;
                }

                if (rigidBody.velocity.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
            }
            // idleAnimationTimer.tAt = 0;
            idleAnimationTimer.turnOn(); 
            swapAnimation = true;
            toSwapTo = IdleAnimation.ANIMATION_IDLE1;
            checkSwap();
            // Debug.Log("can Val = " + idleAnimationTimer.getCanoncial());
        } else {
            bool fin = idleAnimationTimer.updateTimer(Time.deltaTime);
            float canVal = idleAnimationTimer.getCanoncial();

            if(canVal < 0.5f) {
                // if() {
                    
                    swapAnimation = true;
                    toSwapTo = IdleAnimation.ANIMATION_IDLE1;
                // }

            } else if(canVal >= 0.5f) {
                // Debug.Log("second idle at " + canVal);
                // if() {
                    swapAnimation = true;
                    toSwapTo = IdleAnimation.ANIMATION_IDLE2;
                // }
            }

            if(fin) {
                idleAnimationTimer.turnOn(); //basically reset the timer
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
        if (Mathf.Abs(rigidBody.velocity.x) > 0.1f)
        {
          // audioComponents[1].Play();
        } 


        if(isGrounded && !isJumping && !animator.GetBool("jump") && rigidBody.velocity.y <= 0) {
            //can't jump
            jumpTimer.turnOff();
        }
        
        if((Input.GetButtonDown("Jump") && isGrounded && canControlPlayer)) {
            if (!jumpTimer.isOn())
            {
                animator.SetTrigger("jump");
                jumpAudioSrc.Play();
                jumpTimer.turnOn();
                jumpTimer.period = 0.5f;
                // readyingJump = true;
                
            } else {
                Assert.IsTrue(false);
            }
            
        } else {
            // Debug.Log("couldn't jump");
        }

        

        if(isGrounded) {
            animator.ResetTrigger("landing");
        }

        if(!isGrounded && isFallingAnim) {
            bool isAboutToLand = false;
                
            Vector2 rayDir = rigidBody.velocity;
            rayDir.Normalize();
            RaycastHit2D[] hits = Physics2D.RaycastAll(boxCollider.bounds.center, rayDir, 0.5f*boxCollider.size.y + landingRaySize, physicsLayerMask);
            for(int i = 0; i < hits.Length; ++i) {
                RaycastHit2D landingHit = hits[i];
                if(landingHit && landingHit.collider.gameObject != gameObject && !landingHit.collider.isTrigger && landingHit.collider.gameObject.transform.parent != gameObject) {
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
        if(isGrounded) {
            if(validPosTimer.isOn()) {
                bool b = validPosTimer.updateTimer(Time.fixedDeltaTime);
                if(b) {
                    
                    lastValidPos[validPosIndexAt] = transform.position;
                    validPosIndexAt++;
                    if(lastValidPosTop < lastValidPos.Length) {
                        lastValidPosTop++;
                    }
                    if(validPosIndexAt >= lastValidPos.Length) {
                        validPosIndexAt = 0;
                    }
                    validPosTimer.turnOn();
                }

            }
            

        } else {
            validPosTimer.tAt = validPosTimer.period;
        }

        if(jumpTimer.isOn()) {
            if(Input.GetButton("Jump") && jumpTimer.period < 1.15f) {
                jumpTimer.period += timeIncrease*Time.fixedDeltaTime;

            }
        }
        
        Vector2 movementForce = new Vector2(0, 0);
        
        if(autoMoveTimer.isOn() && !autoMoveTimer.paused) {

            bool finished = autoMoveTimer.updateTimer(Time.fixedDeltaTime);
            // Debug.Log("auto move timer ON " + autoMoveTimer.tAt);
            //the automovedirection should only be between -1 && 1
            movementForce = autoMoveDirection;
            if(finished) {
                autoMoveTimer.turnOff();
            }
        } else if(canControlPlayer) {
            movementForce.x = Input.GetAxis("Horizontal");
            if(Mathf.Abs(movementForce.x) > 0.25f) {
                movementForce.x = Mathf.Sign(movementForce.x)*Mathf.Max(Mathf.Abs(movementForce.x), 0.8f);    
            }
            
        } 

        float thisJmpAccel = jumpAccel;
        
        //NOTE(ollie): This is the jump timer so you get a nice jump. We propotion out the jump force over the jump
        if(jumpTimer.isOn()) {
            bool fin = jumpTimer.updateTimer(Time.fixedDeltaTime);
            if(Input.GetButton("Jump")) {

            }
            
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
        if(isGrounded) {
            movementForce.x *= moveAccel;    
        } else {
            movementForce.x *= moveWhileJumpAccel;
        }
        
        movementForce.y *= thisJmpAccel;
        rigidBody.AddForce(movementForce + f);
    }
}
