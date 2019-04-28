using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using ConfigControls_namespace;

public class swordAttack : MonoBehaviour
{

    public enum AttackTypes {
        ATTACK_NULL,
        ATTACK_UP,
        ATTACK_SIDE_LEFT,
        ATTACK_SIDE_RIGHT,
        ATTACK_IDLE,
    }
    private Timer attackTimer;
    private BoxCollider2D sword;
    private Animator animator;
    public float attackTime;
    private AttackTypes attackType;
    private Rigidbody2D thisRigidBody;
    private Transform thisTrans;
    private AudioSource audiosrc;
    public AudioClip attackSound;
    private Vector2 forceToAdd;
    public float attackForce;
    private bool appliedAttackForce;
    // Start is called before the first frame update
    void Start()
    {
        sword = gameObject.GetComponent<BoxCollider2D>();
        thisTrans = gameObject.GetComponent<Transform>();
        animator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        thisRigidBody = gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
        attackTimer = new Timer(attackTime);
        sword.enabled = false;
        thisTrans.localScale = new Vector3(0, thisTrans.localScale.y, thisTrans.localScale.z);
        audiosrc = gameObject.GetComponent<AudioSource>();
        forceToAdd = new Vector2(0, 0);
    }

      void OnTriggerEnter2D(Collider2D other) {
        GameObject gm = other.gameObject;

        if(gm.tag == "Enemy" && !other.isTrigger) {
            IHitBox hb = (IHitBox)gm.GetComponent(typeof(IHitBox));
            if(hb != null) {
                hb.wasHit();
            }

        }
      }
    // Update is called once per frame
    void Update()
    {
        bool isInAttackAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_attack2") || animator.GetCurrentAnimatorStateInfo(0).IsName("tinker_attack1") || animator.GetBool("attack1") || animator.GetBool("attack2");
        
        if (Input.GetButtonDown("Fire1") && !Input.GetButton(ConfigControls.SPELLS_TRIGGER_BTN) && !isInAttackAnimation)
        {
            appliedAttackForce = false;
            float xMove = Input.GetAxis("Horizontal");
            float yMove = Input.GetAxis("Vertical");
            bool isHorizontal = xMove != 0.0f;
            bool isVertical = Mathf.Abs(yMove) > Mathf.Abs(xMove);
            audiosrc.PlayOneShot(attackSound);
            attackTimer.turnOn();
            if (isVertical) {
                animator.SetTrigger("attack1");
                attackType = AttackTypes.ATTACK_UP;
                
            } else if(isHorizontal) {
                animator.SetTrigger("attack2");
                if(xMove < 0) {
                    attackType = AttackTypes.ATTACK_SIDE_LEFT;    
                } else {
                    attackType = AttackTypes.ATTACK_SIDE_RIGHT;    
                }
                
            } else {
                animator.SetTrigger("attack2"); //don't yet have stationary attack
                attackType = AttackTypes.ATTACK_IDLE;
            }
        }

        if(attackTimer.isOn()) {
            bool isFinished = attackTimer.updateTimer(Time.deltaTime);
            float tWarped = attackTimer.tAt / attackTimer.period;
            if(tWarped > 0.3f) {
                sword.enabled = true;
            } 
            if(tWarped > 0.8f) {
                sword.enabled = false;
            }

            if(isFinished) { 
                tWarped = 1.0f;
                attackTimer.turnOff();
            }
            if (tWarped > 0.5f) { tWarped = 1.0f - tWarped; }
            tWarped *= 2.0f;



            switch(attackType) {
                case AttackTypes.ATTACK_SIDE_RIGHT: 
                case AttackTypes.ATTACK_SIDE_LEFT: {
                    if(!appliedAttackForce && sword.enabled) {
                        if(attackType == AttackTypes.ATTACK_SIDE_LEFT) { //left
                            forceToAdd.x += -attackForce;
                            // Debug.Log("adding LEFT force");
                        } else {
                            forceToAdd.x += attackForce;
                            // Debug.Log("adding RIGHT force");
                        }
                        appliedAttackForce = true;
                    }

                } break;
                case AttackTypes.ATTACK_UP: {
                    if(!appliedAttackForce && sword.enabled) {
                        forceToAdd.y += attackForce;
                        // Debug.Log("adding UP force");
                        appliedAttackForce = true;
                    }

                } break;
                case AttackTypes.ATTACK_IDLE: {


                } break;
                default: {

                } break;
            }
            float xVal = Mathf.Lerp(0, 3, tWarped);
            float xPos = 1;
            if(thisRigidBody.velocity.x < 0) {
                xPos *= -1;
            }
            thisTrans.localScale = new Vector3(xVal, thisTrans.localScale.y, thisTrans.localScale.z);
            thisTrans.localPosition = new Vector3(xPos, thisTrans.localPosition.y, thisTrans.localPosition.z);

        }
    }

    void FixedUpdate() {
        thisRigidBody.AddForce(forceToAdd);
        forceToAdd.x = 0;
        forceToAdd.y = 0;

    }
}
