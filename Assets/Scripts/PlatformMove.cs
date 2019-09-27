using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

namespace EasyPlatform {
    public enum PlatformType {
        PLATFORM_NULL,
        PLATFORM_ROTATE,
        PLATFORM_MOVE_LINEAR,
        PLATFORM_MOVE_CIRCLE,
        PLATFORM_FALL_APART,
        PLATFORM_FLIP,

    }
    public class PlatformMove : MonoBehaviour
    {

       
        private float tAt;
        public Transform startPos;
        public Transform endPos;
        public PlatformType type;

        private Timer springTimer;
        private Animator animator;
        public float radius;
        public float startRadians;
        private bool falledApart;

        public float bounceScale;

        private Timer respawnTimer;

        private Vector3 lastP;

        [HideInInspector] public PlayerMovement playerReference;

        [HideInInspector] public Rigidbody2D rb;

        private Vector3 tempStartP;
        public float period;

        // Start is called before the first frame update
        void Start()
        {
            respawnTimer = new Timer(2.0f);
            respawnTimer.turnOff();
            playerReference = null;
            rb = gameObject.GetComponent<Rigidbody2D>();

            springTimer = new Timer(0.5f);
            springTimer.turnOff();

            tempStartP = transform.position;
            falledApart = false;

            if(type == PlatformType.PLATFORM_ROTATE) {
                gameObject.GetComponent<EdgeCollider2D>().enabled = false;
                gameObject.GetComponent<PlatformEffector2D>().enabled = false;
            } else if(type != PlatformType.PLATFORM_NULL) {
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }

            // if(type == PlatformType.PLATFORM_FLIP) {

            // }

            if(type == PlatformType.PLATFORM_FALL_APART) {
                animator = gameObject.GetComponent<Animator>();
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }

        public void StartSpring() {
            if(!springTimer.isOn()) 
            {
                springTimer.turnOn();
            } else {
                if(springTimer.getCanoncial() > 0.5f) {
                    springTimer.tAt = springTimer.period - springTimer.tAt;     
                }
                
            }
                
        }
        public void HitPlatform() {
            if(type == PlatformType.PLATFORM_FALL_APART && !falledApart && !respawnTimer.isOn()) {
                animator.SetTrigger("FallApart");
                falledApart = true;
            }
        }

        public void FallApart() {
            respawnTimer.turnOn();
            gameObject.GetComponent<EdgeCollider2D>().enabled = false;
            gameObject.GetComponent<PlatformEffector2D>().enabled = false;
            
        }


        
        private void FixedUpdate()
        {
            if(respawnTimer.isOn()) {
                bool f = respawnTimer. updateTimer(Time.fixedDeltaTime);
                if(f) {
                    respawnTimer.turnOff();
                    animator.SetTrigger("Comeback");
                    falledApart = false;
                    gameObject.GetComponent<EdgeCollider2D>().enabled = true;
                    gameObject.GetComponent<PlatformEffector2D>().enabled = true;
                           
                }
            } 

            bool finTimer = false;
            tAt += Time.fixedDeltaTime;
            if(tAt > period) {
                tAt = tAt - period;
                finTimer = true;
            }
             float time = tAt / period;
            switch(type) {
                case PlatformType.PLATFORM_ROTATE: {
                    float addend = time*360;
                    rb.MoveRotation(transform.rotation.z + addend + startRadians);
                } break;
                case PlatformType.PLATFORM_MOVE_LINEAR: {
                    float lerpT = 0.5f*((float)Mathf.Sin(2*Mathf.PI*time) + 1.0f);
                    Vector3 newP = Vector3.Lerp(startPos.position, endPos.position, lerpT);
                    if(playerReference != null) {
                        if(playerReference.currentParent == this) {
                            // Vector3 normVec = (endPos.position - startPos.position);
                            // normVec.Normalize();
                            // Vector3 dir = lastP - newP;
                            // if(Vector2.Dot(normVec, dir) > 0) {
                            //     normVec = (startPos.position - endPos.position);
                            // } 
                            
                            // normVec.Normalize();
                            // playerReference.rigidBody.AddForce(Mathf.Lerp(0, 7000, lerpT)*normVec);    
                        } else {
                            playerReference = null;
                        }
                    }
                    lastP = transform.position;
                    transform.position = newP;

                } break;
                case PlatformType.PLATFORM_MOVE_CIRCLE: {
                    transform.position = new Vector3(radius*Mathf.Cos(2*time*Mathf.PI + startRadians), radius*Mathf.Sin(2*time*Mathf.PI + startRadians), transform.position.z) + new Vector3(startPos.position.x, startPos.position.y, 0);
                } break;
                case PlatformType.PLATFORM_FALL_APART: {
                    
                } break;
                case PlatformType.PLATFORM_NULL: {
                    if(springTimer.isOn()) {
                        bool b = springTimer.updateTimer(Time.fixedDeltaTime);

                        float lerpT = (float)Mathf.Sin(Mathf.PI*springTimer.getCanoncial());
                        Vector3 newP = Vector3.Lerp(tempStartP, tempStartP - new Vector3(0, bounceScale, 0), lerpT);
                        // Debug.Log("updateing spring");
                        transform.position = newP;
                        if(b) {
                            springTimer.turnOff();
                        }
                    }

                } break;
                case PlatformType.PLATFORM_FLIP: {
                    if(finTimer) {
                           
                    }
                } break;
                default: {
                    Assert.IsTrue(false);
                } break;
            }
        }
    }
}