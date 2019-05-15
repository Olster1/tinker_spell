using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private BoxCollider2D hitBox;
    private Animator animator;
    private RockGullumAI rockGullumAI;
    public GameObject hitParticleSystem;
    
    private Rigidbody2D parentRigidBody;
    public GameObject player;
    private Transform parentTransform;
    private bool applyForceForHit;
    private Vector3 directionOfForce;
    public float forceScale;
    // Start is called before the first frame update
    void Start()
    {
        hitBox = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        parentRigidBody = gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
        parentTransform = gameObject.transform.parent.gameObject.GetComponent<Transform>();
        rockGullumAI = gameObject.transform.parent.gameObject.GetComponent<RockGullumAI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {

        

    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     bool isHit = animator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");
    //     if (!isHit && other.gameObject.tag == "Sword") //only the sword can hit the rock gollums
    //     {
    //         animator.SetTrigger("WasHit");
    //         rockGullumAI.health--;
    //         rockGullumAI.redHurtTimer.turnOn();


    //         Debug.Log("Hit Enemy");
    //         applyForceForHit = true;
    //         directionOfForce = Vector3.Normalize(parentTransform.position - player.transform.position);

    //         //Instantiate(hitParticleSystem);

    //         if (rockGullumAI.health == 0)
    //         {
    //             rockGullumAI.deathTimer = new Timer_namespace.Timer(1.0f);
    //             rockGullumAI.deathTimer.turnOn();

    //         }
    //     }
    // }


}
