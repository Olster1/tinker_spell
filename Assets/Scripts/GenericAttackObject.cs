using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public enum EnemyType {
	ENEMY_GOOD,
	ENEMY_EVIL,
}

public class GenericAttackObject : MonoBehaviour
{


	public float minDamage;
	public float maxDamage;
	private BoxCollider2D attackCollider;
	public Vector2 startPos;
	public Vector2 endPos;
	public float period;
	private Timer attackTimer;
	public EnemyType type;
  public List<int> idList;

    // Start is called before the first frame update
    void Start() {	
        attackTimer = new Timer(period);
        attackTimer.turnOn();
        idList = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {	
    	bool finished = attackTimer.updateTimer(Time.fixedDeltaTime);
    	float canVal = attackTimer.getCanoncial();
    	Vector2 midPos = Vector2.Lerp(startPos, endPos, canVal);
    	gameObject.transform.localPosition = new Vector3(midPos.x, midPos.y, gameObject.transform.localPosition.z);

    	if(finished) {
    		Destroy(gameObject);
    	}
    }

    void OnTriggerEnter2D(Collider2D other) {
      GameObject gm = other.gameObject;
      int id = gm.GetInstanceID();
      if(!idList.Contains(id)) {
        idList.Add(id);
        IHitBox hb = (IHitBox)gm.GetComponent(typeof(IHitBox));
        if(hb != null) {
            int attackDamage = (int)Mathf.Lerp(minDamage, maxDamage, Random.Range(0.0f, 1.0f));
            hb.wasHit(attackDamage, "mellee", type, transform.position);
        }
      }
    }

    void FixedUpdate () {
    	
    }
}
