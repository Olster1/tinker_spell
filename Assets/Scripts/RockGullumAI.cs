using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class RockGullumAI : MonoBehaviour, IHitBox
{
    public GameObject playerToFollow;
    private Transform playerTransform;
    private Transform thisTransform;
    private Rigidbody2D thisRigidbody;
    public float accelForce;
    public float attackForce; 
    private BoxCollider2D thisCollider;
    private Animator thisAnimator;
    [HideInInspector] public Timer deathTimer;
    [HideInInspector] public Timer walkTimer;
    private SpriteRenderer spRenderer;
    public float partolDistance;
    public float attackDistance;
    private Vector2 ForceToAdd;
    [HideInInspector] public int health;
    public int startHealth;
    private Vector3 startP;
    private Timer fadeInTimer;
    [HideInInspector] public Timer redHurtTimer;
    private Vector4 startTint;
    public GameObject damageNumbersObject;

    private bool applyForceForHit;
    private Vector3 directionOfForce;
    public float forceScale;
    public float attackForceScale;
    private bool isDying;
    public float rayCastSize;

    public GameObject healthBar;
    private GameObject healthInnerBar;
    private SpriteRenderer healthBarSpriteRenderer;
    private float startScale;

    private Color startColor;

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

    private Ai_State aiState;

    private Ai_SubState subAiState;

    private void getRandomAiSubState() {
        subAiState = (Ai_SubState)((int)Random.Range(0, (int)(Ai_SubState.AI_SUB_COUNT)));
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = playerToFollow.GetComponent<Transform>();
        thisCollider= gameObject.GetComponent<BoxCollider2D>();
        thisTransform = gameObject.GetComponent<Transform>();
        thisRigidbody = gameObject.GetComponent<Rigidbody2D>();
        thisAnimator = gameObject.GetComponent<Animator>();
        spRenderer= gameObject.GetComponent<SpriteRenderer>();
        deathTimer = new Timer(1.0f);
        walkTimer = new Timer(4.0f);
        aiState = Ai_State.AI_FIND;
        ForceToAdd = new Vector2();
        startP = thisTransform.position;
        fadeInTimer = new Timer(0.2f);
        fadeInTimer.turnOn();
        redHurtTimer = new Timer(0.4f);
        redHurtTimer.turnOff();
        health = startHealth;
        startTint = spRenderer.color;

        healthInnerBar = healthBar.transform.Find("health_bar").gameObject;
        //for color
        healthBarSpriteRenderer = healthInnerBar.GetComponent<SpriteRenderer>();
        //
        startColor = healthBarSpriteRenderer.color;

        startScale = healthInnerBar.transform.localScale.x;
        
    }

    public void flipGollumSprite() {
        if (thisRigidbody.velocity.x > 0)
        {
            spRenderer.flipX = true;
        }

        if (thisRigidbody.velocity.x < 0)
        {
            spRenderer.flipX = false;
        }
    }

    public void BeginFadeInTimer() {
        fadeInTimer.turnOn();
        health = startHealth;
        thisTransform.position = startP;
        isDying = false;
        Vector3 tempScale = healthInnerBar.transform.localScale;
        tempScale.x = startScale;
        healthInnerBar.transform.localScale = tempScale;
        healthBarSpriteRenderer.color = startColor;
        
    }

    public void wasHit(int damage, string type) {
        bool isHit = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");
       if (!isHit) //only the sword can hit the rock gollums
       {
           GameObject damageNumObj = Instantiate(damageNumbersObject,  transform.position, Quaternion.identity);
           DamageNumber damageNum = damageNumObj.GetComponent<DamageNumber>();
           damageNum.initializeObject(damage, type);


           thisAnimator.SetTrigger("WasHit");
           this.health -= damage;
           this.redHurtTimer.turnOn();

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

           applyForceForHit = true;
           directionOfForce = forceScale * Vector3.Normalize(thisTransform.position - playerTransform.position);

           //Instantiate(hitParticleSystem);

           if (this.health < 0)
           {
                thisAnimator.SetTrigger("isDead");
               //  thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rock_gollum_die");
               // this.deathTimer = new Timer_namespace.Timer(1.0f);
               // this.deathTimer.turnOn();
                isDying = true;

           }
       }
    }

    private void Update()
    {
        if(redHurtTimer.isOn()) {
            bool finished = redHurtTimer.updateTimer(Time.deltaTime);
            float canVal = redHurtTimer.getCanoncial();
            canVal = Mathf.Sin(canVal*Mathf.PI);
            float redValAlpha = Mathf.Lerp(1, 0, canVal);
            spRenderer.color = new Vector4(1, redValAlpha, redValAlpha, 1);
            if(finished) {
                if(!deathTimer.isOn()) {
                    spRenderer.color = Color.white;
                }
                redHurtTimer.turnOff();
            }
        }
        if(fadeInTimer.isOn()) {
            
            bool finished = fadeInTimer.updateTimer(Time.deltaTime);
            float canVal = fadeInTimer.getCanoncial();
            float alphaVal = Mathf.Lerp(0, 1, canVal);
            spRenderer.color = new Vector4(spRenderer.color.r, spRenderer.color.g, spRenderer.color.b, alphaVal);
            if(finished) {
                // thisRigidbody.detectCollisions = true;
                // thisCollider.enabled = true;
                fadeInTimer.turnOff();
            }

        }
        bool isAttacking = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rock_gollum_attack");
        //Debug.Log(isAttacking);
        bool isHit = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");
        // bool isDying = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rock_gollum_die");

        Vector2 diffVec = playerTransform.position - thisTransform.position;
        if(isDying) {
            
        } else if (isHit)
        {

        }
        else if (aiState == Ai_State.AI_ATTACK)
        {
            //spRenderer.color = Color.red;
            if (isAttacking)
            {

            }
            else
            {
                ForceToAdd += attackForce * Mathf.Sign(diffVec.x) * Vector2.right;
                aiState = Ai_State.AI_FIND;
            }
        }
        else if (aiState == Ai_State.AI_FIND)
        {
            //spRenderer.color = Color.blue;
            if (Vector2.SqrMagnitude(diffVec) < attackDistance)
            {
                aiState = Ai_State.AI_ATTACK;
                thisAnimator.SetTrigger("IsAttacking");
                applyForceForHit = true;
                directionOfForce = attackForceScale*Vector3.Normalize(playerTransform.position - thisTransform.position);


            }
            else if (Vector2.SqrMagnitude(diffVec) < partolDistance)
            {
                ForceToAdd += accelForce * Mathf.Sign(diffVec.x) * Vector2.right;
            }
            else
            {
                aiState = Ai_State.AI_PATROL;
            }
        }
        else if (aiState == Ai_State.AI_PATROL)
        {
            if (Vector2.SqrMagnitude(diffVec) < partolDistance)
            {
                aiState = Ai_State.AI_FIND;
            } else {
                if(walkTimer.isOn()) {

                } else {
                    walkTimer.turnOn();   
                    getRandomAiSubState();

                }
                bool finished = walkTimer.updateTimer(Time.deltaTime);
                if(finished) {
                    walkTimer.turnOff();
                    getRandomAiSubState();
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
                        RaycastHit2D[] hits = Physics2D.RaycastAll(thisCollider.bounds.center, Vector2.left, raySize);
                        Debug.DrawLine(thisCollider.bounds.center, transform.position + colOffset + raySize*Vector3.left);
                        for(int i = 0; i < hits.Length; ++i) {
                            RaycastHit2D hit = hits[i];
                            if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) {
                                subAiState = Ai_SubState.AI_SUB_RIGHT;
                                break;
                            }
                        }
                        
                        
                    } break;
                    case Ai_SubState.AI_SUB_RIGHT: {
                        ForceToAdd += accelForce * Vector2.right;
                        //NOTE(ollie): Do we want to use layers instead so we are using just one raycast?? Not sure if this would be faster
                        RaycastHit2D[] hits = Physics2D.RaycastAll(thisCollider.bounds.center, Vector2.right, raySize);
                        Debug.DrawLine(thisCollider.bounds.center, transform.position + colOffset + raySize*Vector3.right);
                        for(int i = 0; i < hits.Length; ++i) {
                            RaycastHit2D hit = hits[i];
                            if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) {
                                subAiState = Ai_SubState.AI_SUB_LEFT;
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

        thisAnimator.SetFloat("WalkSpeed", thisRigidbody.velocity.x);

        if (thisRigidbody.velocity.x > 0)
        {
            spRenderer.flipX = true;
        }

        if (thisRigidbody.velocity.x < 0)
        {
            spRenderer.flipX = false;
        }


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
    // Update is called once per frame
    void FixedUpdate()
    {
        if (applyForceForHit) {
            Vector2 forceVec = new Vector2(directionOfForce.x, directionOfForce.y);
            thisRigidbody.AddForce(forceVec);
            applyForceForHit = false;
        }

        thisRigidbody.AddForce(ForceToAdd);
        ForceToAdd.x = 0;
        ForceToAdd.y = 0;
    }


}
