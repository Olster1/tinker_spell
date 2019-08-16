using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyAttackObjectCreator;
using EasyForceUpdator;
using UnityEngine.Assertions;
public class RockGullumAI : MonoBehaviour, IHitBox
{
    public GameObject playerToFollow;
    public GameObject hitPs;
    private Transform playerTransform;
    private Transform thisTransform;
    private Rigidbody2D thisRigidbody;
    public float accelForce;
    public float attackForce; 
    private BoxCollider2D thisCollider;
    private Animator thisAnimator;
    
    [HideInInspector] public Timer walkTimer;
    private SpriteRenderer spRenderer;
    public Vector2 partolDistance;
    public float attackDistance;
    private Vector2 ForceToAdd;
    [HideInInspector] public int health;
    public int startHealth;
    private Vector3 startP;
    private Timer fadeInTimer;
    [HideInInspector] public Timer redHurtTimer;
    private Vector4 startTint;
    public GameObject amber;
    public GameObject damageNumbersObject;
    public GameObject genericAttackObject;
    private bool finishedAttack;
    public string nameCollider;

    private int hitCount;
    private float lastDiffVec;
    public ActivateQuote quote;
    
    public GameObject attackSwipe;
    
    public GameObject rockObj;

    public Timer respawnTimer;
    
    public Sprite[] rocks;
    public BossFightTrigger bossTriggerEnd;
    
    private PlayerMovement playerMovement;
    
    public AudioSource rockHitSound;
    public AudioSource sentinelAttack;
    
    public bool isSentinel;
    public bool isRangeGollum;
    
    public Vector2 earthOffset;
    
    public GameObject earthAttackObject;
    
    private bool flipForTurnAround;
    public bool sleeping;
    
    public float rayCastSize;
    
    private Vector3 newPos;

    private float lastVelocity;
    
    public float knockBackForce;
    public GameObject healthBar;
    private GameObject healthInnerBar;
    private SpriteRenderer healthBarSpriteRenderer;
    private float startScale;
    private int physicsLayerMask;
    private Timer timerForPatrol;
    private Timer spawnFadeTimer;
    
    public bool dead;
    
    private Color startColor;
    
    private ForceUpdator forceUpdator;
    
    public enum Ai_State {
        AI_NULL,
        AI_PATROL, 
        AI_FIND,
        AI_IDLE,
        AI_ATTACK,
        AI_OTHER
    }
    
    public enum Ai_SubState {
        AI_SUB_IDLE,
        AI_SUB_LEFT,
        AI_SUB_RIGHT,
        AI_SUB_COUNT,
    }
    
    public Ai_State aiState;
    
    public Ai_SubState subAiState;
    
    private void getRandomAiSubState() {
        subAiState = (Ai_SubState)((int)Random.Range(0, (int)(Ai_SubState.AI_SUB_COUNT)));
        // while(subAiState == Ai_SubState.AI_SUB_IDLE) { //for debug testing
        //     subAiState = (Ai_SubState)((int)Random.Range(0, (int)(Ai_SubState.AI_SUB_COUNT)));
        // } 
    }
    
    public void FireRock() {
        Vector2 diffVec = playerTransform.position - thisTransform.position;
        diffVec.Normalize();
        int indexAt = Random.Range(0, rocks.Length - 1);
        if(indexAt == rocks.Length - 1) {
            indexAt--;
        }
        
        Sprite s = rocks[indexAt];
        
        GameObject obj = Instantiate(rockObj, transform.position,  Quaternion.identity);
        SpriteRenderer thisSp = obj.GetComponent<SpriteRenderer>();
        thisSp.sprite = s;
        Rigidbody2D thisRb = obj.GetComponent<Rigidbody2D>();
        float rockForce = 4000;//(float)Random.Range(4000, 6000);
        thisRb.AddForce(rockForce*diffVec);
        thisRb.AddTorque(30);
        
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        DebugEntityManager entManager = Camera.main.GetComponent<DebugEntityManager>();
        entManager.AddEntity(gameObject);
        lagGoodies = new List<LagGoodie>();
        playerTransform = playerToFollow.GetComponent<Transform>();
        playerMovement = playerToFollow.GetComponent<PlayerMovement>();
        thisCollider= gameObject.GetComponent<BoxCollider2D>();
        thisTransform = gameObject.GetComponent<Transform>();
        thisRigidbody = gameObject.GetComponent<Rigidbody2D>();
        thisAnimator = gameObject.GetComponent<Animator>();
        spRenderer= gameObject.GetComponent<SpriteRenderer>();
        
        walkTimer = new Timer(1.0f);
        aiState = Ai_State.AI_PATROL;
        ForceToAdd = new Vector2();
        fadeInTimer = new Timer(0.2f);
        fadeInTimer.turnOff();
        redHurtTimer = new Timer(0.4f);
        redHurtTimer.turnOff();
        health = startHealth;
        startTint = spRenderer.color;

        respawnTimer = new Timer(3.0f);
        respawnTimer.turnOff();
        
        spawnFadeTimer = new Timer(0.5f);
        spawnFadeTimer.turnOff();
        startP = transform.position;
        
        finishedAttack = true;
        
        sleeping = true;
        
        timerForPatrol = new Timer(walkTimer.period);
        timerForPatrol.turnOff();
        
        healthInnerBar = healthBar.transform.Find("health_bar").gameObject;
        //for color
        healthBarSpriteRenderer = healthInnerBar.GetComponent<SpriteRenderer>();
        //
        startColor = healthBarSpriteRenderer.color;
        
        startScale = healthInnerBar.transform.localScale.x;
        
        physicsLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);// | Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("EnemyAiCollision"));
        
        forceUpdator = new ForceUpdator();
        
        newPos = healthBar.transform.localPosition;
    }
    
    public void flipGollumSprite() {
        // if (thisRigidbody.velocity.x > 0)
        // {
        //     spRenderer.flipX = true;
        // }
        
        // if (thisRigidbody.velocity.x < 0)
        // {
        //     spRenderer.flipX = false;
        // }
    }
    
    public List<LagGoodie> lagGoodies;
    
    public class LagGoodie {
        Timer timer;
        AmberType type;
        float angle;
        
        public LagGoodie(AmberType type, float lagTime, float angle) {
            this.type = type;
            timer = new Timer(lagTime);
            timer.turnOn();
            this.angle = angle;
        }
        
        public bool update(RockGullumAI ai) {
            bool result = false;
            if(timer.isOn()) {
                bool finished = timer.updateTimer(Time.deltaTime);
                if(finished) {
                    ai.CreateGoodie(type, angle);
                    result = true;
                    timer.turnOff();        
                }
            }
            return result;
        }
    }
    
    private void CreateGoodie(AmberType type, float angle = 0.0f) {
        
        GameObject objAmber = Instantiate(amber, transform.position,  Quaternion.identity);
        Rigidbody2D amberRb = objAmber.GetComponent<Rigidbody2D>();
        CollectAmber amberCollect = objAmber.transform.GetChild(0).gameObject.GetComponent<CollectAmber>();
        amberCollect.type = (AmberType)type;
        
        
        
        if(type == AmberType.AMBER_HEALTH || type == AmberType.AMBER_MANA) {
            float randScale = Random.Range(0.7f, 1.3f);
            objAmber.transform.localScale = new Vector3(randScale, randScale, 1);
            objAmber.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
        } else if (type == AmberType.AMBER_SENTINEL_HEAD) {
            objAmber.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
            if(quote != null) {
                amberCollect.SetQuoteOnCollect(quote);
            }
        } else {
            objAmber.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        float randomAngle = angle;
        if(angle == 0.0f) {
            randomAngle = Mathf.Lerp(0.25f*Mathf.PI, 0.75f*Mathf.PI, Random.Range(0.0f, 1.0f));
        }
        Vector2 newForce = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        amberRb.bodyType = RigidbodyType2D.Dynamic;
        float lerpVal = Mathf.Lerp(52000, 60000, Random.Range(0.0f, 1.0f));
        amberRb.AddForce(lerpVal*newForce);
    }
    
    private void emitAmber(AmberType type, int number, float angle = 0.0f, float lagTime = 0.0f) {
        for(int i = 0; i < number; ++i) {
            if(lagTime > 0.0f) {
                lagGoodies.Add(new LagGoodie(type, lagTime, angle));
            } else {
                CreateGoodie(type);
            }
            
        }
    }
    
    private void releaseGoodies() {
        emitAmber(AmberType.AMBER_AMBER, 4);
        emitAmber(AmberType.AMBER_HEALTH, 10);
        emitAmber(AmberType.AMBER_MANA, 6);
        if(isSentinel && hitCount <= 1) 
        {
            emitAmber(AmberType.AMBER_SENTINEL_HEAD, 1, 0.5f*Mathf.PI, 1.0f);
        }
    }
    
    public void Dead() {
        transform.position = startP;
        health = startHealth;
        Vector3 tempScale = healthInnerBar.transform.localScale;
        tempScale.x = startScale;
        healthInnerBar.transform.localScale = tempScale;
        healthBarSpriteRenderer.color = startColor;
        respawnTimer.period = (float)Random.Range(3.0f, 10.0f);
        aiState = Ai_State.AI_PATROL;
        respawnTimer.turnOn();
        redHurtTimer.turnOff();
        healthBar.SetActive(false);
    }
    
    public void BeginFadeInTimer() {
        fadeInTimer.turnOn();
        // health = startHealth;
        // thisTransform.position = startP;
        // isDying = false;
        // Vector3 tempScale = healthInnerBar.transform.localScale;
        // tempScale.x = startScale;
        // healthInnerBar.transform.localScale = tempScale;
        // healthBarSpriteRenderer.color = startColor;
        
        //releaseGoodies();
        
        // Destroy(gameObject);
        //Dead();
        
    }
    
    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        bool isHit = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");
        if (!isHit && enemyType == EnemyType.ENEMY_GOOD && !dead && !fadeInTimer.isOn()) 
        {
            GameObject damageNumObj = Instantiate(damageNumbersObject,   transform.position, Quaternion.identity);
            DamageNumber damageNum = damageNumObj.GetComponent<DamageNumber>();
            damageNum.initializeObject(damage, type);
            
            ForceToAddStruct force = new ForceToAddStruct(0.2f, knockBackForce*((Vector2)transform.position - position));
            forceUpdator.AddForce(force);
            
            thisAnimator.SetTrigger("WasHit");
            this.health -= damage;
            if(isSentinel || isRangeGollum) {
                this.redHurtTimer.turnOn();
            }
            
            
            Instantiate(attackSwipe, transform);
            
            // Time.timeScale = 0.0f;
            // playerMovement.globalPauseTimer.turnOn();
            
            rockHitSound.Play();
            
            float healthAsPercent = ((float)health / (float)startHealth);
            healthAsPercent = Mathf.Max(0, healthAsPercent);
            Vector3 tempScale = healthInnerBar.transform.localScale;
            tempScale.x = startScale * healthAsPercent;
            healthInnerBar.transform.localScale = tempScale;
            Color originalColor = healthBarSpriteRenderer.color;
            if(healthAsPercent < 0.6f && healthAsPercent > 0.3f) {
                originalColor = new Vector4(1, 1, 0, 1);
            } else if(healthAsPercent < 0.3f) {
                originalColor = Color.red;
            }
            
            healthBarSpriteRenderer.color = originalColor;
            
            //Instantiate(hitParticleSystem);
            
            if (this.health < 0)
            {
                hitCount++;
                dead = true;
                thisAnimator.SetTrigger("isDead");
                
                if(isSentinel || isRangeGollum) {
                    
                    fadeInTimer.turnOn();
                }
                if(bossTriggerEnd != null) {
                    bossTriggerEnd.bossDied();
                }
                
            }
        }
    }
    
    public void enforceFlip() {
        spRenderer.flipX = flipForTurnAround;
    }
    
    private Vector2 diffVecForAttack;
    public void enforceFlipForAttack() {
        Vector2 diffVec = playerTransform.position - thisTransform.position;
        if(diffVec.x < 0) {
            spRenderer.flipX = false;
        } else {
            spRenderer.flipX = true;
        }
        diffVecForAttack = diffVec;
    }
    
    
    public void applyAttackForce() {
        Vector2 diffVec = diffVecForAttack;
        ForceToAddStruct force = new ForceToAddStruct(0.2f, attackForce * Mathf.Sign(diffVec.x) * Vector2.right);
        forceUpdator.AddForce(force);
        Instantiate(hitPs, transform);
        // Debug.Log("applying attack force");
    }
    
    public void applyRangeAttackForce() {
        Vector2 diffVec = diffVecForAttack;
        diffVec.Normalize();
        ForceToAddStruct force = new ForceToAddStruct(0.2f, attackForce * diffVec);
        forceUpdator.AddForce(force);
        // Debug.Log("applying attack force");
    }
    
    
    public void CreateAttackObject() {
        Vector2 diffVec = diffVecForAttack;
        GameObject attackObj = Instantiate(genericAttackObject, transform);
        
        Vector2 startPos = new Vector2(1, 0);
        Vector2 directionOfAttack = Mathf.Sign(diffVec.x)*Vector2.right;
        AttackObjectCreator.initAttackObject(attackObj, startPos + 4*directionOfAttack, startPos + 4*directionOfAttack, 
                                             EnemyType.ENEMY_EVIL, 0.5f, 12, 16);
        
    }
    
    
    public void CreateRangeAttackObject() {
        Vector2 diffVec = diffVecForAttack;
        diffVec.Normalize();
        GameObject attackObj = Instantiate(genericAttackObject, transform);
        
        Vector2 startPos = new Vector2(0, 0);
        AttackObjectCreator.initAttackObject(attackObj, startPos + 4*diffVec, startPos + 4*diffVec, 
                                             EnemyType.ENEMY_EVIL, 0.5f, 12, 16);
        
    }
    
    public void endAttack() {
        Vector2 diffVec = playerTransform.position - thisTransform.position;
        aiState = Ai_State.AI_PATROL;
        walkTimer.turnOn();
        timerForPatrol.turnOn();
        subAiState = (diffVec.x > 0) ? Ai_SubState.AI_SUB_LEFT : Ai_SubState.AI_SUB_RIGHT;
        DoTurnAround(subAiState != Ai_SubState.AI_SUB_LEFT);
        finishedAttack = true;
        
        
    }
    
    public void CreateSentinelAttackObject() {
        Vector2 diffVec = diffVecForAttack;
        GameObject attackObj = Instantiate(genericAttackObject, transform);
        
        Vector2 directionOfAttack = Mathf.Sign(diffVec.x)*Vector2.right;
        Vector2 startPos = new Vector2(1, 0) + 4*directionOfAttack;
        Vector2 endPos = startPos + new Vector2(0, -2);
        
        AttackObjectCreator.initAttackObject(attackObj, startPos, endPos, 
                                             EnemyType.ENEMY_EVIL,0.5f, 22, 36);
        
        sentinelAttack.pitch = (float)Random.Range(0.9f, 1.15f);
        
        sentinelAttack.Play();
        Instantiate(hitPs, transform);
        
    }
    
    public void createEarthAttack() {
        float xDir = Mathf.Sign(diffVecForAttack.x);
        GameObject earthObj = Instantiate(earthAttackObject, transform.position - new Vector3(xDir*earthOffset.x, earthOffset.y, 0),  Quaternion.identity);
        earthObj.GetComponent<SpriteRenderer>().flipX = xDir < 0;
        
        if(xDir < 0) {
            earthObj.GetComponent<EarthAttack>().rePosBoxes();    
        }
        
        GenericAttackObject earthAttack = earthObj.transform.GetChild(0).gameObject.GetComponent<GenericAttackObject>();
        earthAttack.startPos.x *= xDir;
        earthAttack.endPos.x *= xDir;
        earthAttack.attackType = "earth";
        earthAttack.type = EnemyType.ENEMY_EVIL;
    }
    
    
    private void Update()
    {
        for(int i = 0; i < lagGoodies.Count; ) {
            bool rem = lagGoodies[i].update(this);
            if(rem) {
                lagGoodies.RemoveAt(i);
            } else {
                i++;
            }
        }

        if(respawnTimer.isOn()) {
            bool finished = respawnTimer.updateTimer(Time.deltaTime);
            spRenderer.color = Color.clear;
            if(finished) {
                dead = false;
                respawnTimer.turnOff();
                spawnFadeTimer.turnOn();
                healthBar.SetActive(true);
            }
        }
        
        if(spawnFadeTimer.isOn()) {
            bool finished = spawnFadeTimer.updateTimer(Time.deltaTime);
            float alphaVal = spawnFadeTimer.getCanoncial();
            spRenderer.color = new Vector4(1, 1, 1, alphaVal);
            if(finished) {
                
                spawnFadeTimer.turnOff();
                
            }
            
        }
        
        if(redHurtTimer.isOn()) {
            bool finished = redHurtTimer.updateTimer(Time.deltaTime);
            float canVal = redHurtTimer.getCanoncial();
            canVal = Mathf.Sin(canVal*Mathf.PI);
            float redValAlpha = Mathf.Lerp(1, 0, canVal);
            spRenderer.color = new Vector4(1, redValAlpha, redValAlpha, 1);
            if(finished) {
                if(!fadeInTimer.isOn() && !respawnTimer.isOn() && !dead) {
                    spRenderer.color = Color.white;
                }
                redHurtTimer.turnOff();
            }
        }
        
        if(fadeInTimer.isOn()) {
            // Debug.Log("fade in timer");
            bool finished = fadeInTimer.updateTimer(Time.deltaTime);
            float canVal = 1.0f - fadeInTimer.getCanoncial();
            
            spRenderer.color = new Vector4(spRenderer.color.r, spRenderer.color.g, spRenderer.color.b, canVal);
            
            if(finished) {
                if(isSentinel) {
                    Debug.Log("fadeInTimer working");
                }
                spRenderer.color = Color.clear;
                // thisRigidbody.detectCollisions = true;
                // thisCollider.enabled = true;
                fadeInTimer.turnOff();
                releaseGoodies();
                Dead();
                // Destroy(gameObject);
            }
            
        }
        
        //Assert.IsTrue(!dead);
        if(!dead) 
        {
            Vector2 diffVec = playerTransform.position - thisTransform.position;
            
            // if(Mathf.Abs(diffVec.x) < partolDistance.x && Mathf.Abs(diffVec.y) < partolDistance.y) {
            //     newPos.x = Mathf.Sign(diffVec.x)*4;
            //     healthBar.transform.localPosition = newPos;    
            // }
            
            
            bool isAttacking = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rock_gollum_attack");
            //Debug.Log(isAttacking);
            bool isHit = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");
            // bool isDying = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rock_gollum_die");
            
            
            thisAnimator.SetFloat("WalkSpeed", thisRigidbody.velocity.x);
            
            // bool isInTurnAround = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rock_gollum_turn_around");
            // bool isWalking = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rockGollumWalk");
            
            // //Debug.Log("last frame flip " + lastframFlip);
            // if(isWalking) {
            //     if (thisRigidbody.velocity.x > 0.3)
            //     {
            //         spRenderer.flipX = true;
            //     }
                
            //     if (thisRigidbody.velocity.x < -0.3)
            //     {
            //         spRenderer.flipX = false;
            //     }
            // }
            

            
            
            
            
            // if (isDying) {
            //     // bool finished = deathTimer.updateTimer(Time.deltaTime);
            //     // float canVal = deathTimer.getCanoncial();
            //     // float alphaVal = Mathf.Lerp(1, 0, canVal);
            //     // spRenderer.color = new Vector4(spRenderer.color.r, spRenderer.color.g, spRenderer.color.b, alphaVal);
            //     // thisRigidbody.detectCollisions = false;
            //     // thisCollider.enabled = false;
            
            //     AnimatorStateInfo info = thisAnimator.GetCurrentAnimatorStateInfo(0);
            //     bool isDeadTriggered = thisAnimator.GetBool("isDead");
            //     bool isInDieState = info.IsName("rock_gollum_die") || isDeadTriggered;
            //     if (!isInDieState) {
            //         // deathTimer.turnOff();
            
            //         // spRenderer.color = startTint;
            //     }
            // }
            
        }
        
    }

    void DoTurnAround(bool lookLeft) {
        // Debug.Log("turn_around");
        if(lookLeft != spRenderer.flipX) {
            if(isSentinel || isRangeGollum) {
                spRenderer.flipX = lookLeft;
            } else {
                thisAnimator.SetTrigger("turn_around");
                flipForTurnAround = lookLeft;        
            }
            
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // if(isSentinel) {
        //     Debug.Log("?????");
        // }
        
        if(!(dead || spawnFadeTimer.isOn() || fadeInTimer.isOn() || respawnTimer.isOn())) {
            bool isHit = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");

            if(!sleeping) {
                Vector2 diffVec = playerTransform.position - thisTransform.position;
                spRenderer.color = Color.white;
                // if(isSentinel) {
                //     Debug.Log("sentinelAttack");
                // }
               
                if(dead) {
                } if (isHit)
                {
                    if(!finishedAttack) {
                        endAttack();
                        finishedAttack = true;
                    }
                }
                else if (aiState == Ai_State.AI_ATTACK)
                {
                    // spRenderer.color = Color.red;
                    
                }
                else if (aiState == Ai_State.AI_FIND)
                {
                    // spRenderer.color = Color.blue;
                    //spRenderer.color = Color.blue;
                    // Debug.Log("Diff Vec: " + Vector2.SqrMagnitude(diffVec));
                    if (Vector2.SqrMagnitude(diffVec) < attackDistance)
                    {
                        aiState = Ai_State.AI_ATTACK;
                        finishedAttack = false;
                        thisAnimator.SetTrigger("IsAttacking");
                    }
                    else if (Mathf.Abs(diffVec.x) < partolDistance.x && Mathf.Abs(diffVec.y) < partolDistance.y)
                    {
                        float signToPlayer = Mathf.Sign(diffVec.x);
                        
                        ForceToAdd += accelForce * signToPlayer * Vector2.right;

                        if((signToPlayer > 0) !=  spRenderer.flipX){
                            DoTurnAround(signToPlayer > 0);
                        } 
                    }
                    else
                    {
                        aiState = Ai_State.AI_PATROL;
                    }
                }
                else if (aiState == Ai_State.AI_PATROL)
                {
                    // spRenderer.color = Color.yellow;
                    bool finishedForPatrol = timerForPatrol.updateTimer(Time.fixedDeltaTime);
                    if(finishedForPatrol) {
                        timerForPatrol.turnOff();
                    }
                    if (Mathf.Abs(diffVec.x) < partolDistance.x && Mathf.Abs(diffVec.y) < partolDistance.y && !timerForPatrol.isOn())
                    {
                        aiState = Ai_State.AI_FIND;
                    } else 
                    {
                        if(walkTimer.isOn()) {
                            
                        } else {
                            walkTimer.turnOn();   
                            Ai_SubState lastState = subAiState;
                            getRandomAiSubState();
                             if(lastState != subAiState && (subAiState == Ai_SubState.AI_SUB_RIGHT || subAiState == Ai_SubState.AI_SUB_LEFT)) {
                                // Debug.Log("finished walktimer 1");
                                Assert.IsTrue(subAiState != Ai_SubState.AI_SUB_IDLE);
                                DoTurnAround(subAiState != Ai_SubState.AI_SUB_LEFT);
                            }
                            
                        }
                        bool finished = walkTimer.updateTimer(Time.fixedDeltaTime);
                        if(finished) {
                            walkTimer.turnOn();
                             
                            Ai_SubState lastState = subAiState;
                            getRandomAiSubState();
                             if(lastState != subAiState && (subAiState == Ai_SubState.AI_SUB_RIGHT || subAiState == Ai_SubState.AI_SUB_LEFT)) {
                                // Debug.Log("finished walktimer 2: " + subAiState);
                                Assert.IsTrue(subAiState != Ai_SubState.AI_SUB_IDLE);
                                DoTurnAround(subAiState != Ai_SubState.AI_SUB_LEFT);
                            }
                        }
                        Vector2 colSize = thisCollider.size;
                        Vector3 colOffset = thisCollider.offset;
                        float raySize = 0.5f*colSize.x + rayCastSize;
                        
                        switch(subAiState) {
                            
                            case Ai_SubState.AI_SUB_IDLE: {
                                //do nothing
                            } break;
                            case Ai_SubState.AI_SUB_LEFT: {
                                
                                ForceToAdd += accelForce * Vector2.left;
                                //NOTE(ollie): Do we want to use layers instead so we are using just one raycast?? Not sure if this would be faster
                                RaycastHit2D[] hits = Physics2D.RaycastAll(thisCollider.bounds.center, Vector2.left, raySize, physicsLayerMask);
                                Debug.DrawLine(thisCollider.bounds.center, thisCollider.bounds.center + raySize*Vector3.left);
                                bool found = false;
                                for(int i = 0; i < hits.Length; ++i) {
                                    RaycastHit2D hit = hits[i];
                                    Assert.IsTrue(!found);
                                    if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) {
                                        // Debug.Log(hit.collider.gameObject.name);
                                        subAiState = Ai_SubState.AI_SUB_RIGHT;
                                        nameCollider = hit.collider.gameObject.name;
                                        // Debug.Log("hit left");
                                        DoTurnAround(true);
                                        found = true;
                                        break;
                                    }
                                }
                                
                                
                            } break;
                            case Ai_SubState.AI_SUB_RIGHT: {
                                ForceToAdd += accelForce * Vector2.right;

                                //NOTE(ollie): Do we want to use layers instead so we are using just one raycast?? Not sure if this would be faster
                                RaycastHit2D[] hits = Physics2D.RaycastAll(thisCollider.bounds.center, Vector2.right, raySize, physicsLayerMask);
                                Debug.DrawLine(thisCollider.bounds.center, thisCollider.bounds.center + raySize*Vector3.right);
                                // Debug.DrawLine(thisCollider.bounds.center, transform.position + colOffset + raySize*Vector3.right);
                                for(int i = 0; i < hits.Length; ++i) {
                                    RaycastHit2D hit = hits[i];
                                    
                                    if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) {
                                        // Debug.Log(hit.collider.gameObject.name);
                                        subAiState = Ai_SubState.AI_SUB_LEFT;
                                        nameCollider = hit.collider.gameObject.name;
                                        DoTurnAround(false);
                                        // Debug.Log("hit right");
                                        break;
                                    }
                                }
                                
                            } break;
                            default: {
                                
                            } break;
                        }    
                    }
                    //spRenderer.color = Color.green;
                    //do nothing for now
                } else if(aiState == Ai_State.AI_NULL) {
                    aiState = Ai_State.AI_FIND;
                }
                
            }
            
            Vector2 f = forceUpdator.update();        
            
            thisRigidbody.AddForce(ForceToAdd + f);
            ForceToAdd.x = 0;
            ForceToAdd.y = 0;
        }
    }
    
    
}
