using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyForceUpdator;

public class FireCoalAi : MonoBehaviour, IHitBox
{
    
	public enum CoalDirection {
		DIRECTION_RIGHT,
		DIRECTION_LEFT,
	}
    
	public Animator anim;
	public Rigidbody2D rb;
	public float knockBackForce;
	public GameObject damageNumbersObject;
	public float startHealth;
    private float health;
	private ForceUpdator forceUpdator;
	private Timer fadeOutTimer;
	private CoalDirection direction;
	public float movePower;
	private int physicsLayerMask;
	public BoxCollider2D thisCollider;
	private float raySize;
	public SpriteRenderer sp;
    private Vector3 startP;
    private Timer spawnTimer;
    private bool isOut;
    public GameObject attackObj;
    private HealthBar healthBar;
    private ItemEmitter itemEmitter;
    private BeastryJournal beastJournal;
    // Start is called before the first frame update
    void Start()
    {
        itemEmitter = Camera.main.GetComponent<ItemEmitter>();
        DebugEntityManager entManager = Camera.main.GetComponent<DebugEntityManager>();
        entManager.AddEntity(gameObject);

        beastJournal = Camera.main.GetComponent<BeastryJournal>();

        healthBar = transform.Find("Gollum_health-bar").gameObject.GetComponent<HealthBar>();

        
        forceUpdator = new ForceUpdator();
        fadeOutTimer = new Timer(1.0f);
        fadeOutTimer.turnOff();
        direction = CoalDirection.DIRECTION_LEFT;
        raySize = 0.5f*thisCollider.size.x + 0.3f;
        health = startHealth;
        physicsLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer) | Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("EnemyAiCollision"));
        startP = transform.position;
        
        spawnTimer = new Timer(4.0f);
        spawnTimer.turnOff();
    }
    
    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        // bool isHit = thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("RockGollumHit");
        if (enemyType == EnemyType.ENEMY_GOOD) 
        {
            GameObject damageNumObj = Instantiate(damageNumbersObject,  transform);
            DamageNumber damageNum = damageNumObj.GetComponent<DamageNumber>();
            damageNum.initializeObject(damage, type);
            
            ForceToAddStruct force = new ForceToAddStruct(0.2f, knockBackForce*((Vector2)transform.position - position));
            forceUpdator.AddForce(force);
            
            this.health -= damage;
            healthBar.UpdateHealthBar((int)health, (int)startHealth);

            beastJournal.FoundBeast(BeastId.FIRE_COAL);

            // this.redHurtTimer.turnOn();
            
            // Time.timeScale = 0.0f;
            // playerMovement.globalPauseTimer.turnOn();
            
            // rockHitSound.Play();
            
            // float healthAsPercent = ((float)health / (float)startHealth);
            // healthAsPercent = Mathf.Max(0, healthAsPercent);
            // Vector3 tempScale = healthInnerBar.transform.localScale;
            // tempScale.x = startScale * healthAsPercent;
            // healthInnerBar.transform.localScale = tempScale;
            // Color originalColor = healthBarSpriteRenderer.color;
            // if(healthAsPercent < 0.6f && healthAsPercent > 0.3f) {
            //  originalColor = new Vector4(1, 1, 0, 1);
            // } else if(healthAsPercent < 0.3f) {
            //  originalColor = Color.red;
            // }
            
            // healthBarSpriteRenderer.color = originalColor;
            
            //Instantiate(hitParticleSystem);
            
            if (this.health < 0)
            {
                // thisAnimator.SetTrigger("isDead");
                //  thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("rock_gollum_die");
                // this.deathTimer = new Timer_namespace.Timer(1.0f);
                // this.deathTimer.turnOn();
                // isDying = true;
                // if(isSentinel || isRangeGollum) {
                
                //     fadeInTimer.turnOn();
                // }
                if(!fadeOutTimer.isOn()) {
           		 fadeOutTimer.turnOn();
                 isOut = true;
                 attackObj.SetActive(false);
                 gameObject.GetComponent<ParticleSystem>().Stop();
                 thisCollider.enabled = false;
                 healthBar.Hide();
                 itemEmitter.emitAmber(AmberType.AMBER_HEALTH, 4, transform.position);
                }
                
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.x > 0.1f) {
        	sp.flipX = true;
        } else if(rb.velocity.x < -0.1f) {
        	sp.flipX = false;
        }
        
        
    }
    
    void FixedUpdate() {
        if(spawnTimer.isOn()) {
            bool f = spawnTimer.updateTimer(Time.fixedDeltaTime);
            if(f) {
                spawnTimer.turnOff();
                isOut = false;
                fadeOutTimer.turnOn();
                gameObject.GetComponent<ParticleSystem>().Play();
                attackObj.SetActive(true);
                health = startHealth;
                healthBar.ResetHealthBar();
                healthBar.Show();
                thisCollider.enabled = true;
            }
        } else {
        	Vector2 moveForce = new Vector2();
        	if(fadeOutTimer.isOn()) {
        		bool f = fadeOutTimer.updateTimer(Time.fixedDeltaTime);
                float alpha = fadeOutTimer.getCanoncial();
                if(isOut) {
                    alpha = 1.0f - alpha;  
                } 
        		sp.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        		if(f) {
                    if(isOut) {
                        spawnTimer.turnOn();
                    }   
                    
                    fadeOutTimer.turnOff();
        		}
        	} else {
        		
        		Vector2 dir = (direction == CoalDirection.DIRECTION_LEFT) ? Vector2.left : Vector2.right;
        		RaycastHit2D[] hits = Physics2D.RaycastAll(thisCollider.bounds.center, dir, raySize, physicsLayerMask);
        		for(int i = 0; i < hits.Length; ++i) {
        		    RaycastHit2D hit = hits[i];
        		    
        		    if(hit && hit.collider.gameObject != gameObject && !hit.collider.isTrigger && hit.collider.gameObject.name != "player") {
        		        if(direction == CoalDirection.DIRECTION_LEFT) {
        		        	direction = CoalDirection.DIRECTION_RIGHT;
        		        } else {
        		        	direction = CoalDirection.DIRECTION_LEFT;
        		        }
        		        
        		        break;
        		    }
        		}
                
        		if(direction == CoalDirection.DIRECTION_LEFT) {
        			moveForce = movePower*Vector2.left;
                    
        		} else if(direction == CoalDirection.DIRECTION_RIGHT) {
        			
        			moveForce = movePower*Vector2.right;
        		}
                
        		
        	}
            
            
        	anim.SetFloat("xSpeed", rb.velocity.x);
            
        	Vector2 totalForce = forceUpdator.update();        
        	
        	rb.AddForce(totalForce + moveForce);
        }
    }
}
