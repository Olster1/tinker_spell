using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class DestructableEnvironment : MonoBehaviour, IHitBox
{
	public ParticleSystem ps;
    private ItemEmitter itemEmitter;
    // Start is called before the first frame update
    void Start()
    {
        itemEmitter = Camera.main.GetComponent<ItemEmitter>();
    }

    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        if (enemyType == EnemyType.ENEMY_GOOD) 
        {
        	// Instaniate falling leaves
            // emit amber 
            // set respawin timer
            itemEmitter.emitAmber(AmberType.AMBER_AMBER, 2, transform.position);
                

        }
    }

    // Update is called once per frame
    void Update()
    {
        //update respawn timer
    }
	
}
