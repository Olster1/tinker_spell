using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyGameManager;
using Timer_namespace;

public class pickup_fuelCell : MonoBehaviour
{
	public SpriteRenderer sp;
	private Timer fadeTimer;
	public AudioSource audioSrc;
    public ActivateQuote quote;
    public BoxCollider2D box;

    // Start is called before the first frame update
    void Start()
    {
        fadeTimer = new Timer(0.5f);
        fadeTimer.turnOff();
        box = gameObject.GetComponent<BoxCollider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
    	if(fadeTimer.isOn()) {
	        bool fin = fadeTimer.updateTimer(Time.deltaTime);
	        float t01 = 1.0f - fadeTimer.getCanoncial();
	        Color c = sp.color;
	        c.a = t01;
	        sp.color = c;
	        if(fin) {
	        	fadeTimer.turnOff();

	        	// Destroy(gameObject);
	        }
	    }
    }

    private void OnTriggerEnter2D(Collider2D other) {
    	if(other.gameObject.name == "Player") {
    		audioSrc.Play();
            box.enabled = false;
    		fadeTimer.turnOn();
    		GameManager.fuelCellCount++;
            if(quote != null) {
                quote.Activate();
            }

    	}
    }
}
