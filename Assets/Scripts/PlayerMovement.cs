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
using EasyPlatform;

public class PlayerMovement : MonoBehaviour, IHitBox
{
    
    public enum IdleAnimation {
        ANIMATION_IDLE1, 
        ANIMATION_IDLE2, 
    }
    
    [HideInInspector] public Rigidbody2D rigidBody;
    public float jumpAccel;
    public float moveAccel;

    [HideInInspector] public PlatformMove currentParent;

    public float moveWhileJumpAccel;
    private Animator animator;
    public Image redHurtImage;

    private float timeInAir;
    
    public AudioSource hurtBreathing;

    public float velocityMargin; 
    
    private Transform beginParentTransform;

    [HideInInspector] public Timer earthTimer;
    [HideInInspector] public Timer waterTimer;
    [HideInInspector] public Timer fireTimer;
    
    public GameObject dustEffect;
    
    public SpellAi spell;
    
    public Timer flashTimer;
    
    public Animator camAnimator;
    public ParticleSystem ps;

    public float flipMargin;
    
    private Timer hurtPulseTimer;

    public EarthMoveValidator earthMoveValidator;
    
    public GameObject thisCamera;
    private int earthAttackLevel;
    
    public Image fadePanel;
    public SpriteRenderer spShadow;
    
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
    
    [HideInInspector] public bool isGrounded;

    [HideInInspector] public BoostPad booster;
    
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
    
    private int physicsLayerMask;
    
    public float attackForceUp;
    public float downwardAttackForce;
    
    public GameObject genericAttackObject;
    public GameObject damageNumbersObject;
    public GameObject[] earthAttackObjects;
    
    private Vector2 originalBoxSize;
    private Vector2 originalBoxOffset;
    private Vector2 originalJumpOffset;
    public float percentOfJump;
    public float maxJumpTime;
    private bool flipForEarthMove;
    
    private float shadowP;
    
    private ForceUpdator forceUpdator;

    [HideInInspector] public bool usingController;

    public float timeToAffectJump;

    private SpringJoint2D springForPlatform;
    
    public Vector2 earthOffset;
    [HideInInspector] public Timer globalPauseTimer;
    [HideInInspector] public Timer dieTimer;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        springForPlatform = gameObject.GetComponent<SpringJoint2D>();

        beginParentTransform = transform.parent;

        currentParent = null;
        
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        jumpTimer = new Timer(0.5f);
        jumpTimer.turnOff();
        autoMoveTimer = new Timer(autoMoveTime);
        canControlPlayer = true;
        
        idleAnimationTimer = new Timer(10.0f);

        flashTimer = new Timer(1.0f);
        flashTimer.turnOff();
        
        hurtPulseTimer = new Timer(1.0f);
        hurtPulseTimer.turnOn();
        
        forceUpdator = new ForceUpdator();
        
        physicsLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
        
        
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
        // if(swapAnimation) {
        //     Debug.Log("Swapped animation");
        //     Assert.IsTrue((int)toSwapTo < idleAnimations.Length);
        //     AnimatorOverrideController newController = idleAnimations[(int)toSwapTo];
        //     animator.runtimeAnimatorController = newController;
        // }
    }
    
    public bool CastGroundedRay(Vector3 colliderOffset) {
        Vector3 bottomCenter = boxCollider.bounds.center + new Vector3(0, -0.5f*boxCollider.size.y, 0);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(bottomCenter + colliderOffset, Vector2.down, jumpRaySize, physicsLayerMask);
        Debug.DrawLine(bottomCenter + colliderOffset, bottomCenter + colliderOffset + jumpRaySize*Vector3.down);
                                
        for(int i = 0; i < hits.Length; ++i) {
            RaycastHit2D hit = hits[i];
            if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) {
                float velMarginToUse = velocityMargin; //
                if(hit.collider.gameObject.tag == "WorldGeometrySlope") {
                    velMarginToUse = 50.0f;
                } 

                if(rigidBody.velocity.y <= velMarginToUse) {
                    isGrounded = true;

                    if(hit.collider.gameObject.tag == "WorldGeometryPlaform") {
                        PlatformMove m = hit.collider.gameObject.GetComponent<PlatformMove>();
                        if(m.type == PlatformType.PLATFORM_MOVE_LINEAR || m.type == PlatformType.PLATFORM_MOVE_CIRCLE || m.type == PlatformType.PLATFORM_NULL) {
                            transform.parent = hit.collider.gameObject.transform;
                            transform.localScale = new Vector3(1.0f/transform.parent.localScale.x, 1.0f/transform.parent.localScale.y, 1.0f/transform.parent.localScale.z);
                        }
                        currentParent = m;
                            

                        m.HitPlatform();
                        m.playerReference = this;
                        
                         
                    }
                    break;    
               }
                
            } 
        }
        return isGrounded;
    }

     public void CastShadowRay() {
        Vector3 bottomCenter = boxCollider.bounds.center + new Vector3(0, -0.5f*boxCollider.size.y, 0);
        float shortDist = Mathf.Infinity;
        RaycastHit2D[] hits = Physics2D.RaycastAll(bottomCenter, Vector2.down, Mathf.Infinity, physicsLayerMask);
        // Debug.DrawLine(bottomCenter + colliderOffset, bottomCenter + jumpRaySize*Vector3.down);
        bool  found = false; 
        // string name = "";
        for(int i = 0; i < hits.Length; ++i) {
            RaycastHit2D hit = hits[i];
            if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) {
                if(hit.distance < shortDist && hit.distance < 2) {
                    shortDist = hit.distance;
                    // Debug.Log("short disr: " + shortDist);
                    found = true;
                    // name = hit.collider.gameObject.name;
                } 
            } 
        }
        if(found) {
            // Debug.Log("name: " + name);
            Vector3 scale = spShadow.gameObject.transform.localScale;
            Vector3 localP = spShadow.gameObject.transform.position;
            localP.y = bottomCenter.y - shortDist + 0.2f;
            localP.x = transform.position.x;
            spShadow.gameObject.transform.position = localP;
            Assert.IsTrue(shortDist <= 2.0f);
            float s = 1.0f - (shortDist / 2.0f);
            s = Mathf.Clamp(s, 0.0f, 1.0f);
            // Debug.Log(s);
            scale.x = s;
            spShadow.gameObject.transform.localScale = scale;
        } else {
            spShadow.gameObject.transform.localScale = new Vector3(0, 1, 1);
        }
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

        public void CreateIdleAttack() {
        float signOfMovement = (spriteRenderer.flipX) ? -1: 1;
        
        GameObject attackObj = Instantiate(genericAttackObject, transform);
        
        Vector2 startPos = (Vector2)boxCollider.bounds.center + new Vector2(signOfMovement*(0.5f*boxCollider.size.y + 0.1f), 0);        
        Vector2 localStartPos = (Vector2)transform.InverseTransformPoint(startPos);
        AttackObjectCreator.initAttackObject(attackObj, localStartPos, localStartPos, 
                                             EnemyType.ENEMY_GOOD, 0.5f, 15, 22);
        
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
    
    public void ResetIdleTrigger() {
        animator.ResetTrigger("endIdle");


    }
    
    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        
        if(!flashTimer.isOn()) {
            // bool isHit = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");
            if (enemyType == EnemyType.ENEMY_EVIL) 
            {
                flashTimer.turnOn();
                GameObject damageNumObj = Instantiate(damageNumbersObject,  transform.position, Quaternion.identity);
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
                
                
                if(GameManager.playerHealth < 50) {
                    if(!redHurtImage.enabled) {
                        redHurtImage.enabled = true;
                    }
                    if(!hurtBreathing.isPlaying) {
                        hurtBreathing.Play();
                    }
                    hurtBreathing.volume = Mathf.Lerp(1.0f, 0.7f, GameManager.playerHealth/50.0f);
                    
                    redHurtImage.material.SetFloat("_Amount", Mathf.Lerp(0.1f, 0.0f, GameManager.playerHealth/100.0f));
                    hurtPulseTimer.turnOn();
                }
                //Instantiate(hitParticleSystem);
                
                if (GameManager.playerHealth < 0)
                {
                    hurtBreathing.Stop();
                    Time.timeScale = 0.0f;
                    GameManager.playerHealth = 100;
                    redHurtImage.enabled = false;
                    GameManager.updateHealth = true;
                }
            }
        }
    }
    
    public void ResetPositionFromFall() {
        rigidBody.simulated = true;
        int index = validPosIndexAt + 1;
        if(index >= lastValidPos.Length || lastValidPosTop < lastValidPos.Length) { //if at the end of the buffer or the buffer isn't filled yet
            index = 0;
        } 
        transform.position = lastValidPos[0];
        rigidBody.velocity = Vector2.zero;
        thisCamera.transform.position = new Vector3(transform.position.x, transform.position.y, thisCamera.transform.position.z);
    }
    
    public void Die() {
        panelAnimator.SetTrigger("FadeIn");
        panelAnimator.SetTrigger("FadeFromFall");
        rigidBody.simulated = false;
        
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
    
    public void CreateEarthMove() {
        
        // Handheld.Vibrate();
        
        float xDir = flipForEarthMove ? -1 : 1;
        earthTimer.turnOn();

        Vector3 startEarthP = spell.transform.position;
        startEarthP.y = transform.position.y;
        GameObject earthObj = Instantiate(earthAttackObjects[earthAttackLevel], startEarthP - new Vector3(xDir*earthOffset.x, earthOffset.y, 0),  Quaternion.identity);
        earthObj.GetComponent<SpriteRenderer>().flipX = xDir < 0;
        
        if(xDir < 0) {
            earthObj.GetComponent<EarthAttack>().rePosBoxes();    
        }
        
        GenericAttackObject earthAttack = earthObj.transform.GetChild(0).gameObject.GetComponent<GenericAttackObject>();
        earthAttack.startPos.x *= xDir;
        earthAttack.endPos.x *= xDir;
        earthAttack.attackType = "earth";
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
        if(flashTimer.isOn()) {
            bool flashDone = flashTimer.updateTimer(Time.deltaTime);
            
            float alpha = (float)0.5f*Mathf.Cos(10*Mathf.PI*flashTimer.getCanoncial()) + 0.5f;
            //Debug.Log(alpha);
            Assert.IsTrue(alpha >= 0.0f && alpha <= 1.0f);
            Color tempColor = spriteRenderer.color;
            tempColor.a = alpha;
            spriteRenderer.color = tempColor;
            if(flashDone) {
                flashTimer.turnOff();
                
            }
        }
        
        if(hurtPulseTimer.isOn()) {
            bool hrtDone = hurtPulseTimer.updateTimer(Time.deltaTime);
            float alpha = (float)-Mathf.Cos(2*Mathf.PI*hurtPulseTimer.getCanoncial()) + 1.0f;
            redHurtImage.material.SetFloat("_tAt", alpha);
            if(hrtDone) {
                hurtPulseTimer.turnOn();
            } 
        }
        
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

        if(!isGrounded) {
            transform.parent = beginParentTransform;
            currentParent = null;
            transform.localScale = new Vector3(1, 1, 1);
            springForPlatform.enabled = false;
            springForPlatform.connectedBody = null;
        }



        // Debug.Log("GROUNDED + " + isGrounded);
        CastShadowRay();

        // spriteRenderer.color = (isGrounded ? Color.red : Color.white); 
        
        if(!lastFameGrounded && isGrounded && !isJumping && timeInAir > 0.4f) {
            //landing
            if(currentParent && currentParent.type == PlatformType.PLATFORM_NULL) {
                //do boucy effect
                currentParent.StartSpring();
            }
            landingSoundSource.Play();
            Instantiate(dustEffect, transform.position - new Vector3(0, 2, 0), Quaternion.identity);
            
        }
        
        if(!isGrounded) {
            timeInAir += Time.deltaTime;
        } 

        animator.SetFloat("timeInAir", timeInAir);
        
        if(isGrounded && !lastFameGrounded) {
            if(timeInAir > 1.5f) {
                if(currentParent && currentParent.type == PlatformType.PLATFORM_NULL) {
                    //do boucy effect
                    currentParent.StartSpring();
                }
                camAnimator.SetTrigger("shake1");
            }
            timeInAir = 0;
        }
        
        
        
        bool isInAttackAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("idle_attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_attack2") || animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_attack1") || animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_downward_dash") || animator.GetBool("attack1") || animator.GetBool("idleAttack") || animator.GetBool("attack2") || animator.GetBool("downward_dash");
        bool isFallingAnim = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_falling");
        if (!isInAttackAnimation && canControlPlayer)
        {
            //get direction of the controller
            float xMove = Input.GetAxis("Horizontal");
            float yMove = Input.GetAxis("Vertical");
            
            if(Input.GetButton(ConfigControls.SPELLS_TRIGGER_BTN)) {
                //MAGIC MOVES 

                if(Input.GetButtonDown("Fire2") && isGrounded && GameManager.hasEarth1 && !earthTimer.isOn()) {
                    // if(earthMoveValidator.isOk() >= 0) {
                        earthAttackLevel = 2;//earthMoveValidator.isOk();
                        spell.thisAnimator.SetTrigger("exit_run");
                        flipForEarthMove = spriteRenderer.flipX;
                        spell.flipForDive = spriteRenderer.flipX;
                        spell.thisAnimator.SetTrigger("earth_dive");
                        camAnimator.SetTrigger("zoom");
                        earthMoveValidator.turnOff();
                    // } else {
                    //     earthMoveValidator.turnOn();
                    // }
                } 
            } else {
                earthMoveValidator.turnOff();
                if(Input.GetButtonDown("Fire1")) {
                    
                    bool isHorizontal = xMove != 0.0f;
                    bool isVertical = Mathf.Abs(yMove) > Mathf.Abs(xMove);
                    // audiosrc.PlayOneShot(attackSound);
                    animator.SetTrigger("endIdle");
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
                        animator.SetTrigger("idleAttack"); //don't yet have stationary attack
                        uppercutAttackSound.Play();
                        playAttackVoice();
                    }
                }
            }
        }
        
        
        ////Update the animation variable
        /// 
        /// 
        /// 

        animator.SetBool("UsingController", usingController); 
        
        animator.SetFloat("run_speed", rigidBody.velocity.x);
        if(rigidBody.velocity.x > 1 || rigidBody.velocity.x < -1 || usingController) {
            animator.SetTrigger("endIdle");
        }
        
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
                landingSoundSource.Play();
                Instantiate(dustEffect, transform.position - new Vector3(0, 2, 0), Quaternion.identity);
                animator.SetTrigger("endIdle");
                
                //start timing the jump
                timeInAir = 0;
                jumpAudioSrc.Play();
                Instantiate(dustEffect, transform.position - new Vector3(0, 1, 0), Quaternion.identity);
                jumpTimer.turnOn();
                jumpTimer.period = 0.5f;

                if(currentParent && currentParent.type == PlatformType.PLATFORM_NULL) {
                    //do boucy effect
                    currentParent.StartSpring();
                }

                // if(false) //Input.GetKeyDown(KeyCode.LeftShift)) 
                // {
                //     animator.SetTrigger("DoubleJump");
                //     jumpTimer.period = 0.5f;
                // } else 
                {
                    animator.SetTrigger("jump");
                }
                // readyingJump = true;
                
            } else {
                //Assert.IsTrue(false);
            }
            
        } else {
            // Debug.Log("couldn't jump");
        }
        
        
        if(isGrounded) {
            animator.ResetTrigger("landing");
             animator.ResetTrigger("landingSmall");
        }

        bool isLanding = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_landing") || animator.GetCurrentAnimatorStateInfo(0).IsName("landing_small");
        
        // Debug.Log("is Grounded: " + isGrounded);
        if(!isGrounded && isFallingAnim && landingRaySize > 0 && !isLanding) {
            bool isAboutToLand = false;
            
            Vector2 rayDir = rigidBody.velocity;
            rayDir.Normalize();
            Vector2 startP = boxCollider.bounds.center + new Vector3(0, -0.5f*boxCollider.size.y, 0);
            float rayLen = landingRaySize;
            RaycastHit2D[] hits = Physics2D.RaycastAll(startP, Vector2.down, rayLen, physicsLayerMask);
            float smallestDistance = rayLen;
            for(int i = 0; i < hits.Length; ++i) {
                RaycastHit2D landingHit = hits[i];
                if(landingHit && landingHit.distance < smallestDistance && landingHit.collider.gameObject != gameObject && !landingHit.collider.isTrigger && landingHit.collider.gameObject.transform.parent != gameObject) {
                    smallestDistance = landingHit.distance;
                    isAboutToLand = true;
                }
            }
            Debug.DrawLine(startP, startP + (landingRaySize*Vector2.down));
            // Debug.Log("landing: " + animator.GetBool("landing"));
            //Debug.Log("velocity less: " + (rigidBody.velocity.y < 0));
            // Debug.Log("about to land: " + isAboutToLand);
            if(!animator.GetBool("landing") && !animator.GetBool("landingSmall") && rigidBody.velocity.y < 0 && isAboutToLand) {
                // Debug.Log("setLanding trigger");
                // if(smallestDistance > 1.0f) {
                   animator.SetTrigger("landing");
                   // Debug.Log("LANDING BIG");
                // } else {
                //     animator.SetTrigger("landingSmall");
                //     Debug.Log("LANDING SMALL");
                // }
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
        
        // if(jumpTimer.isOn()) {
        //     if(Input.GetButton("Jump") && jumpTimer.tAt < timeToAffectJump) {
        //         jumpTimer.period += timeIncrease*Time.fixedDeltaTime;
        //         //Debug.Log("hey there");
                
        //     }
        // }
        
        Vector2 movementForce = new Vector2(0, 0);
        usingController = false;
        if(autoMoveTimer.isOn() && !autoMoveTimer.paused) {
            
            bool finished = autoMoveTimer.updateTimer(Time.fixedDeltaTime);
            // Debug.Log("auto move timer ON " + autoMoveTimer.tAt);
            //the automovedirection should only be between -1 && 1
            movementForce = autoMoveDirection;
            if(finished) {
                autoMoveTimer.turnOff();
            }
        } else if(canControlPlayer) {
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("idle_attack")) {
                
                float controllerX = Input.GetAxis("Horizontal");
                if(Mathf.Abs(controllerX) > 0.25f) {
                     
                     Vector2 unitVec = new Vector2(1, 0);
                    if(booster != null && rigidBody.velocity.x*booster.force.x > 0) { 
                        unitVec = 1.3f*booster.unitVec;

                        // Debug.Log("ni Vector2" + unitVec);
                    }

                    movementForce += unitVec*Mathf.Sign(controllerX)*Mathf.Max(Mathf.Abs(controllerX), 0.8f);

                    usingController = true;
                }
            }
            
        } 

        bool isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("player_idle");
        if(!isIdle) {
            if(canControlPlayer) {
                float i = Input.GetAxis("Horizontal");
                if(Mathf.Abs(i) > 0.25f) {
                    if(i > 0) {
                        // Debug.Log("Flipped to false");
                        spriteRenderer.flipX = false;
                    } else {
                        spriteRenderer.flipX = true;
                        // Debug.Log("Flipped to true");
                    }
                } else {
                    if (rigidBody.velocity.x > flipMargin)
                    {
                        spriteRenderer.flipX = false;
                    }
                    
                    if (rigidBody.velocity.x < -flipMargin)
                    {
                        spriteRenderer.flipX = true;
                    }
                }
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
                if(!Input.GetButton("Jump") && jumpTimer.tAt > timeToAffectJump) {
                    jumpTimer.turnOff();
                } else {
                    movementForce.y = 1;

                    float percent = jumpTimer.tAt / maxJumpTime;

                    if(percent > 1.0f) {
                        percent = 1.0f;
                    }
                    
                    thisJmpAccel = Mathf.Lerp(jumpAccel, percentOfJump*jumpAccel, percent);
                    if (fin)
                    {
                        jumpTimer.turnOff();
                        
                    }
                }
            }
            
        } 
        
        
        Vector2 f = forceUpdator.update();
        //if(movementForce.x == 0) 
        // {
        //     Vector2 vel = rigidBody.velocity;
        //     vel.x *= 0.8f;
        //     rigidBody.velocity = vel;
        // }
        if(isGrounded) {
            movementForce.x *= moveAccel;    
        } else {
            movementForce.x *= moveWhileJumpAccel;
        }
        
        movementForce.y *= thisJmpAccel;
        rigidBody.AddForce(movementForce + f);
    }
}
