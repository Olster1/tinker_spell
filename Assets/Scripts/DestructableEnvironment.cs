using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class DestructableEnvironment : MonoBehaviour, IHitBox
{
	public ParticleSystem ps;
    private ItemEmitter itemEmitter;
    [HideInInspector] public Timer growthTimer;
    public AudioSource rustleSound;
    // Start is called before the first frame update
    void Start()
    {
        growthTimer = new Timer(2.0f);
        growthTimer.turnOff();
        itemEmitter = Camera.main.GetComponent<ItemEmitter>();
    }

    private void SetYScale(float y) {
        Vector3 v = transform.localScale;
        v.y = y;
        transform.localScale = v;
    }

    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        if (enemyType == EnemyType.ENEMY_GOOD && !growthTimer.isOn()) 
        {
        	// Instaniate falling leaves
            // emit amber 
            // set respawin timer
            itemEmitter.emitAmber(AmberType.AMBER_AMBER, 2, transform.position);
            ps.Play();
            growthTimer.turnOn();
            SetYScale(0);
            rustleSound.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(growthTimer.isOn()) {
            bool b = growthTimer.updateTimer(Time.deltaTime);
            float f = growthTimer.getCanoncial();
            SetYScale(f);
            if(b) {
                growthTimer.turnOff();
            }
        }
        //update respawn timer
    }
	
}
