using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock_senitnel_ai : MonoBehaviour
{
	private Rigidbody2D thisRigidbody;
	private Animator thisAnimator;
	private SpriteRenderer spRenderer;
	public GameObject playerToFollow;
	public float attackDistance;
	private Vector2 ForceToAdd;
	// private Ai_State aiState;
    // Start is called before the first frame update
    void Start()
    {
        thisRigidbody = gameObject.GetComponent<Rigidbody2D>();
        thisAnimator = gameObject.GetComponent<Animator>();
        spRenderer= gameObject.GetComponent<SpriteRenderer>();
        ForceToAdd = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
    	Vector2 diffVec = playerToFollow.transform.position - gameObject.transform.position;

    	Debug.Log("diff Vec: " + diffVec);
    	Debug.Log("sqr diff Vec: " + Vector2.SqrMagnitude(diffVec));
    	if (Vector2.SqrMagnitude(diffVec) < attackDistance)
    	{
    	    // aiState = Ai_State.AI_ATTACK;
    	    thisAnimator.SetTrigger("attack1");
    	}

    	if (thisRigidbody.velocity.x > 0)
    	{
    	    spRenderer.flipX = true;
    	}

    	if (thisRigidbody.velocity.x < 0)
    	{
    	    spRenderer.flipX = false;
    	}
        
    }

    void FixedUpdate()
    {
        thisRigidbody.AddForce(ForceToAdd);
        ForceToAdd.x = 0;
        ForceToAdd.y = 0;
    }
}
