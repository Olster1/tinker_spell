using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConfigControls_namespace;

public class SpellAi : MonoBehaviour
{
	public GameObject playerObj;
	private Transform playerTransform;
	private Transform thisTransform;
	private Vector2 velocity;
	public float dragFactor;
	public float accelPower;
	private Animator thisAnimator;
	private SpriteRenderer thisSpriteRenderer;
	private BoxCollider2D thisCollider;
	public float speedMargin;
	public Vector3 offset;
	public float circleSize;
	private Vector2 startOffset;
    private PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = playerObj.GetComponent<Transform>();
        thisTransform =  gameObject.GetComponent<Transform>();
        thisAnimator =  gameObject.GetComponent<Animator>();
        thisSpriteRenderer =  gameObject.GetComponent<SpriteRenderer>();
        thisCollider = gameObject.GetComponent<BoxCollider2D>();
        startOffset = thisCollider.offset;
        playerMovement = playerObj.GetComponent<PlayerMovement>();
        velocity = new Vector2(0, 0);
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

    public void defaultIdleFlip() {
        //this is is the idle animation never flips!
        thisSpriteRenderer.flipX = false;
    }

    // Update is called once per frame
    void Update()
    {
        thisAnimator.SetFloat("run_speed", velocity.magnitude);

        bool isInFireAnimation = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("spell_fire_twirl");
        if (Input.GetButton("Fire1") && Input.GetButton(ConfigControls.SPELLS_TRIGGER_BTN) && !isInFireAnimation && playerMovement.canControlPlayer)
        {
            thisAnimator.SetTrigger("fire_twirl");
            
        }

        if(isInFireAnimation) {
        	float tAt = 2*Mathf.PI*thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        	Vector2 offsetVal = new Vector2(Mathf.Cos(tAt), -Mathf.Sin(tAt));
        	offsetVal *= circleSize;
        	thisCollider.offset = startOffset + offsetVal;

        } else {
        	thisCollider.offset = startOffset;
        }

        // if(Input.GetButton("Jump")) {
        // 	thisAnimator.SetTrigger("fire_twirl");
        // }

        if (velocity.x > 0)
        {
            thisSpriteRenderer.flipX = false;
        }

        if (velocity.x < 0)
        {
            thisSpriteRenderer.flipX = true;
        }
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
    	float dt = Time.fixedDeltaTime;
    	Vector3 diffPos = (playerTransform.position + offset) - thisTransform.position;
    	Vector2 diffPos2 = Vector3.Normalize(diffPos);
        if(diffPos.magnitude > 3) {
            velocity += accelPower*diffPos2*dt;
        }
        velocity -= dragFactor*velocity;
        thisTransform.position = new Vector3(thisTransform.position.x + dt*velocity.x, thisTransform.position.y + dt*velocity.y, thisTransform.position.z);
    }
}
