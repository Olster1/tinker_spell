using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterSpell : MonoBehaviour
{	
	private Animator thisAnimator;
	private Vector2 velocity;
	private SpriteRenderer thisSpriteRenderer;

	private Rigidbody2D rigidBody;
	public float moveAccel;
    // Start is called before the first frame update
    void Start()
    {
        thisAnimator = gameObject.GetComponent<Animator>();
        thisSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        velocity = new Vector2();
    }

    public void ClearExitRun() {
        thisAnimator.ResetTrigger("exit_run");
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

    // Update is called once per frame
    void Update()
    {
        thisAnimator.SetFloat("run_speed", velocity.magnitude);
    }

    void FixedUpdate() {
    	Vector2 movementForce = new Vector2();
    	float xMove = Input.GetAxis("Horizontal");
    	float yMove = Input.GetAxis("Vertical");
    	movementForce.x = xMove * moveAccel;
    	movementForce.y = yMove * moveAccel;
    	rigidBody.AddForce(movementForce);
    	velocity = rigidBody.velocity;
    }
}
