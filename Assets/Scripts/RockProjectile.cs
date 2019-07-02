using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
public class RockProjectile : MonoBehaviour
{	
	public float deleteVel;
	private Timer timer;
	public Rigidbody2D rb;
	public SpriteRenderer sp;
	public GameObject attackObj;
	public ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!timer.isOn()) {
        	if(rb.velocity.magnitude < deleteVel) {
        		attackObj.SetActive(false);
        		timer.turnOn();
        		rb.simulated = false;
        		// ps.Play();
        	}
        } 

        if(timer.isOn()) {
        	bool b = timer.updateTimer(Time.deltaTime);
        	float f = 1.0f - timer.getCanoncial();
        	sp.color = new Color(1, 1, 1, f);
        	if(b) {
        		timer.turnOff();
        		Destroy(gameObject);
        	}
        }
    }
}
