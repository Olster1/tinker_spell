using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConfigControls_namespace;
using Timer_namespace;
using EasyForceUpdator;
using UnityEngine.Assertions;

public class SpellAi : MonoBehaviour, IHitBox
{
	public GameObject playerObj;
	private Transform playerTransform;
	private Transform thisTransform;
	private Vector2 velocity;
	public float dragFactor;
	public float accelPower;
	[HideInInspector] public Animator thisAnimator;
	private SpriteRenderer thisSpriteRenderer;
    private SpriteRenderer playerSpriteRenderer;
	public BoxCollider2D triggerCollider;
    public BoxCollider2D rbCol;
	public float speedMargin;
	public Vector3 offset;
    private Vector3 beginOffset;
	public float circleSize;
	private Vector2 startOffset;
    private PlayerMovement playerMovement;
    public float diffSize;
    private Timer centerTimer;
    private Vector3 startPos;
    private Vector3 endPos;
    [HideInInspector] public bool flipForDive;
    public bool controllingSpell;
    private Vector2 movementForce;
    public float moveAccel;
    public CameraFollowPlayer cam;
    public bool isSpellLevel;

    public float reboundForce;

    public GameObject damageNumbersObject;

    private Timer flashTimer;

    private ForceUpdator forceUpdator;

    public Rigidbody2D rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        flashTimer = new Timer(1.0f);
        flashTimer.turnOff();

        forceUpdator = new ForceUpdator();


        playerTransform = playerObj.GetComponent<Transform>();
        thisTransform =  gameObject.GetComponent<Transform>();
        thisAnimator =  gameObject.GetComponent<Animator>();
        thisSpriteRenderer =  gameObject.GetComponent<SpriteRenderer>();
        playerSpriteRenderer = playerObj.GetComponent<SpriteRenderer>();
        
        startOffset = triggerCollider.offset;
        playerMovement = playerObj.GetComponent<PlayerMovement>();
        velocity = new Vector2(0, 0);
        centerTimer = new Timer(0.3f);
        centerTimer.turnOff();
        beginOffset = offset;
        // controllingSpell = false;
        movementForce = new Vector2(0, 0);
    }

    public void InitEarthMove() {
        playerMovement.CreateEarthMove();
    }

    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        if(controllingSpell &&  !flashTimer.isOn()) {

            //NOTE(ol): Tell player they got hit
            flashTimer.turnOn();
            //

            //NOTE(ol): Create the damage number to let player know
            GameObject damageNumObj = Instantiate(damageNumbersObject,  transform.position, Quaternion.identity);
            DamageNumber damageNum = damageNumObj.GetComponent<DamageNumber>();
            damageNum.initializeObject(damage, type);
            //

            //NOTE(ol): Apply rebound force
            Vector2 dir = (Vector2)transform.position - position;
            dir.Normalize();
            ForceToAddStruct force = new ForceToAddStruct(0.1f, reboundForce*dir);
            forceUpdator.AddForce(force);
            ////
        }
    }

    public void flipSprite() {
        if (velocity.x > 0)
        {
            thisSpriteRenderer.flipX = false;
        }

        if (velocity.x < 0)
        {
            thisSpriteRenderer.flipX = true;
        }
    }

    public void GoInFrontOfPlayer() {
        thisSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
    }

    public void GoBehindOfPlayer() {
        thisSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
    }

    private bool fireFlipx;

    public void SetFlipForDive() {
        thisSpriteRenderer.flipX = flipForDive;
    }

    public void SetFlipForFire() {

    }

    public void SetFlipForIdle() {
        thisSpriteRenderer.flipX = false;
    }

    public void SetFlipForWalk() {
        if (velocity.x > 0)
        {
            thisSpriteRenderer.flipX = false;
        }

        if (velocity.x < 0)
        {
            thisSpriteRenderer.flipX = true;
        }
    }

    public void enterPlayerTransform() {
        
        // centerTimer.period = 0;
        // float mag = Vector3.Magnitude(playerTransform.position - transform.position);
        // if(mag > 1.0f) {
        //     centerTimer.period = Mathf.Lerp(0.1f, 2.0f, (mag / 12.0f));
        // }
        // centerTimer.turnOn();
        // transform.parent = playerTransform;
        // startPos = transform.position;
        // endPos = new Vector3(0, 2, startPos.z);
        fireFlipx = thisSpriteRenderer.flipX;
    }

    public void exitPlayerTransform() {
        // transform.parent = playerTransform.parent;
        offset = beginOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if(flashTimer.isOn()) {
            bool flashDone = flashTimer.updateTimer(Time.deltaTime);
            
            float alpha = (float)0.5f*Mathf.Cos(10*Mathf.PI*flashTimer.getCanoncial()) + 0.5f;
            //Debug.Log(alpha);
            Assert.IsTrue(alpha >= 0.0f && alpha <= 1.0f);
            Color tempColor = thisSpriteRenderer.color;
            tempColor.a = alpha;
            thisSpriteRenderer.color = tempColor;
            if(flashDone) {
                flashTimer.turnOff();
                
            }
        }

        thisAnimator.SetFloat("run_speed", velocity.magnitude);

        if(!isSpellLevel) {
            if(Input.GetButtonDown("Fire3")) {
                //This may need some work!!
                playerMovement.canControlPlayer = !playerMovement.canControlPlayer;
                controllingSpell = !playerMovement.canControlPlayer;

                if(controllingSpell) {
                    cam.changeEntityToFollow(BodyToFollow.SPELL_BODY);
                    rigidBody.simulated = true;
                    rbCol.enabled = true;
                } else {
                    cam.changeEntityToFollow(BodyToFollow.PLAYER_BODY);
                    rigidBody.simulated = false;
                    rbCol.enabled = false;
                    ClearForcesForSpell();
                }
            }


            bool isInFireAnimation = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("spell_fire_twirl");
            if (Input.GetButton("Fire1") && Input.GetButton(ConfigControls.SPELLS_TRIGGER_BTN) && !isInFireAnimation && playerMovement.canControlPlayer)
            {
                thisAnimator.SetTrigger("exit_run");
                thisAnimator.SetTrigger("fire_twirl");
                offset.x = 0;
                enterPlayerTransform();
            }

            if(isInFireAnimation) {
                float normalTime = thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            	float tAt = (float)(2*Mathf.PI*normalTime + (0.25*Mathf.PI));
                if(normalTime > 0.35f && normalTime < 0.8f) {
                    GoInFrontOfPlayer();
                } else if(normalTime >= 0.8f) {
                    GoBehindOfPlayer();
                }
            	Vector2 offsetVal = new Vector2(Mathf.Cos(tAt), -Mathf.Sin(tAt));
            	offsetVal *= circleSize;
                // transform.localPosition = offsetVal;
            	// thisCollider.offset = startOffset + offsetVal;

            } else {
                
            	// thisCollider.offset = startOffset;
            }
        }

        bool isWalking = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("spell_run");;
        if(isWalking) {
            SetFlipForWalk();
        }

        // Debug.Log("is spell level: " + isSpellLevel); 
        // Debug.Log("controlling spell: " + controllingSpell); 
    }

    void OnTriggerEnter2D(Collider2D other) {
    	GameObject gm = other.gameObject;
        bool isAttacking = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("spell_fire_twirl");
    
		if(gm.tag == "Enemy" && !other.isTrigger && isAttacking) {
			IHitBox hb = (IHitBox)gm.GetComponent(typeof(IHitBox));
			if(hb != null) {
				hb.wasHit(1, "mellee", EnemyType.ENEMY_GOOD, transform.position);
			}

	    }
    }

    public void ClearExitRun() {
        thisAnimator.ResetTrigger("exit_run");
    }

    public void ClearForcesForSpell() {
        forceUpdator.ClearForces();
    }

    void FixedUpdate()
    {
        if(!(thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("spell_dive") || thisAnimator.GetBool("earth_dive"))) {
            if(!controllingSpell) {
            // if(!centerTimer.isOn()) {
            	float dt = Time.fixedDeltaTime;
            	Vector3 diffPos = (playerTransform.position + offset) - thisTransform.position;
            	Vector2 diffPos2 = Vector3.Normalize(diffPos);
                if(diffPos.magnitude > diffSize) {
                    velocity += accelPower*diffPos2*dt;
                }
                velocity -= dragFactor*velocity;
                thisTransform.position = new Vector3(thisTransform.position.x + dt*velocity.x, thisTransform.position.y + dt*velocity.y, thisTransform.position.z);
            // }
            } else {
                
                float xMove = Input.GetAxis("Horizontal");
                float yMove = Input.GetAxis("Vertical");
                // Debug.Log("x value" + xMove);
                movementForce.x = xMove * moveAccel;
                movementForce.y = yMove * moveAccel;
                Vector2 f = forceUpdator.update();

                rigidBody.AddForce(movementForce + f);
                velocity = rigidBody.velocity;
            }
        }
    }
}
