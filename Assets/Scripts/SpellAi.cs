using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConfigControls_namespace;
using Timer_namespace;

public class SpellAi : MonoBehaviour
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
    [HideInInspector] public bool controllingSpell;
    private Vector2 movementForce;
    public float moveAccel;
    public CameraFollowPlayer cam;

    public Rigidbody2D rigidBody;
    // Start is called before the first frame update
    void Start()
    {
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
        controllingSpell = false;
        movementForce = new Vector2(0, 0);
    }

    public void InitEarthMove() {
        playerMovement.CreateEarthMove();
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

    public void defaultIdleFlip() {
        //this is is the idle animation never flips!
        thisSpriteRenderer.flipX = false;
    }

    private bool fireFlipx;

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
        thisAnimator.SetFloat("run_speed", velocity.magnitude);

        if(Input.GetButtonDown("Fire3")) {
            //This may need some work!!
            playerMovement.canControlPlayer = !playerMovement.canControlPlayer;
            controllingSpell = !playerMovement.canControlPlayer;

            if(controllingSpell) {
                cam.changeEntityToFollow(gameObject);
                rigidBody.simulated = true;
                rbCol.enabled = true;
            } else {
                cam.changeEntityToFollow(playerObj);
                rigidBody.simulated = false;
                rbCol.enabled = false;
            }
        }


        bool isInFireAnimation = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("spell_fire_twirl");
        if (Input.GetButton("Fire1") && Input.GetButton(ConfigControls.SPELLS_TRIGGER_BTN) && !isInFireAnimation && playerMovement.canControlPlayer)
        {
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

        // if(Input.GetButton("Jump")) {
        // 	thisAnimator.SetTrigger("fire_twirl");
        // }
        if(!isInFireAnimation) {
            if (velocity.x > 0)
            {
                thisSpriteRenderer.flipX = false;
            }

            if (velocity.x < 0)
            {
                thisSpriteRenderer.flipX = true;
            }
        } else {
            thisSpriteRenderer.flipX = fireFlipx;
        }

        // if(centerTimer.isOn()) {
        //     bool fin = centerTimer.updateTimer(Time.deltaTime);
        //     float can = centerTimer.getCanoncial();
        //     transform.localPosition = Vector3.Lerp(startPos - playerTransform.position, endPos, can);
        //     if(fin) {
        //         thisAnimator.SetTrigger("fire_twirl");
        //         centerTimer.turnOff();
        //     }
        // }
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

    void FixedUpdate()
    {
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
            movementForce.x = xMove * moveAccel;
            movementForce.y = yMove * moveAccel;
            rigidBody.AddForce(movementForce);
            velocity = rigidBody.velocity;
        }
    }
}
