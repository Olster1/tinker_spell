// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Timer_namespace;
// using ConfigControls_namespace;

// public class swordAttack : MonoBehaviour
// {

//     public enum AttackTypes {
//         ATTACK_NULL,
//         ATTACK_UP,
//         ATTACK_SIDE_LEFT,
//         ATTACK_SIDE_RIGHT,
//         ATTACK_DOWNWARD,
//         ATTACK_IDLE,
//     }
//     private Timer attackTimer;
//     private BoxCollider2D sword;
//     private Animator animator;
//     public float attackTime;
//     private AttackTypes attackType;
//     private Rigidbody2D thisRigidBody;
//     private Transform thisTrans;
//     private AudioSource audiosrc;
//     public AudioClip attackSound;
//     private Vector2 forceToAdd;
//     public float attackForce;
//     public float downwardAttackForce;
//     public float attackForceUp;
//     private bool appliedAttackForce;
//     private PlayerMovement playerMovement;
//     // Start is called before the first frame update
//     void Start()
//     {
//         sword = gameObject.GetComponent<BoxCollider2D>();
//         thisTrans = gameObject.GetComponent<Transform>();
//         animator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
//         thisRigidBody = gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
//         playerMovement = gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>();
//         attackTimer = new Timer(attackTime);
//         sword.enabled = false;
//         thisTrans.localScale = new Vector3(0, thisTrans.localScale.y, thisTrans.localScale.z);
//         audiosrc = gameObject.GetComponent<AudioSource>();
//         forceToAdd = new Vector2(0, 0);
//     }

//     public void downwardStrike() {
//         Vector2 directionAttack = new Vector2(Mathf.Sign(thisRigidBody.velocity.x)*0.707f, -0.707f);
//         forceToAdd += downwardAttackForce*directionAttack;
//         appliedAttackForce = true;
//     }

//       void OnTriggerEnter2D(Collider2D other) {
//         GameObject gm = other.gameObject;

//         IHitBox hb = (IHitBox)gm.GetComponent(typeof(IHitBox));
//         if(hb != null) {
//             int attackDamage = (int)Mathf.Lerp(25, 28, Random.Range(0.0f, 1.0f));
//             hb.wasHit(attackDamage, "mellee", EnemyType.ENEMY_GOOD, thisRigidBody.position);
//         }
//       }
//     // Update is called once per frame
//     void Update()
//     {

//         if(attackTimer.isOn()) {
//             bool isFinished = attackTimer.updateTimer(Time.deltaTime);
//             float tWarped = attackTimer.tAt / attackTimer.period;
//             if(tWarped > 0.5f) {
//                 sword.enabled = true;
//             } 
//             if(tWarped > 0.8f) {
//                 sword.enabled = false;
//             }

//             if(isFinished) { 
//                 tWarped = 1.0f;
//                 attackTimer.turnOff();
//             }
//             if (tWarped > 0.5f) { tWarped = 1.0f - tWarped; }
//             tWarped *= 2.0f;



//             switch(attackType) {
//                 case AttackTypes.ATTACK_SIDE_RIGHT: 
//                 case AttackTypes.ATTACK_SIDE_LEFT: {
//                     if(!appliedAttackForce && sword.enabled) {
//                         if(attackType == AttackTypes.ATTACK_SIDE_LEFT) { //left
//                             forceToAdd.x += -attackForce;
//                             // Debug.Log("adding LEFT force");
//                         } else {
//                             forceToAdd.x += attackForce;
//                             // Debug.Log("adding RIGHT force");
//                         }
//                         appliedAttackForce = true;
//                     }

//                 } break;
//                 case AttackTypes.ATTACK_UP: {
//                     if(!appliedAttackForce && sword.enabled) {
//                         forceToAdd.y += attackForceUp;
//                         // Debug.Log("adding UP force");
//                         appliedAttackForce = true;
//                     }

//                 } break;
//                 case AttackTypes.ATTACK_IDLE: {


//                 } break;
//                 default: {

//                 } break;
//             }
//             float xVal = Mathf.Lerp(0, 3, tWarped);
//             float xPos = 1;
//             if(thisRigidBody.velocity.x < 0) {
//                 xPos *= -1;
//             }
//             thisTrans.localScale = new Vector3(xVal, thisTrans.localScale.y, thisTrans.localScale.z);
//             thisTrans.localPosition = new Vector3(xPos, thisTrans.localPosition.y, thisTrans.localPosition.z);

//         }
//     }

//     void FixedUpdate() {
//         thisRigidBody.AddForce(forceToAdd);
//         forceToAdd.x = 0;
//         forceToAdd.y = 0;

//     }
// }
