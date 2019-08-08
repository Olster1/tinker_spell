using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

public class InteractWithIndicate : MonoBehaviour
{
	private float tAt;
	private Timer fadeTimer;
	public SpriteRenderer spRender;
	private float startY;
	private InteractIndicateState state;
	[HideInInspector] public bool available;

	public enum InteractIndicateState {
		INTERACT_NULL,
		INTERACT_IN,
		INTERACT_OUT,
	}
    // Start is called before the first frame update
    void Start()
    {
        fadeTimer = new Timer(0.5f);
        fadeTimer.turnOff();
        state = InteractIndicateState.INTERACT_NULL;
        startY = transform.position.x;
        spRender.color = new Color(1, 1, 1, 0);
        available = true;
    }

    public bool isOn() {
        return (state == InteractIndicateState.INTERACT_IN);
    }

    public void fadeIn() {
    	if(available) {
			fadeTimer.turnOn();			
			state = InteractIndicateState.INTERACT_IN;
			tAt = 0;
		}
    }

    public void fadeOut() {
    	if(available) {
			fadeTimer.turnOn();			
			state = InteractIndicateState.INTERACT_OUT;
		}
    }

    public void OnTriggerEnter2D(Collider2D other) {
    	GameObject gm = other.gameObject;
    	if(gm.name == "Player") {
    	  fadeIn();
    	}
    }

    public void OnTriggerExit2D(Collider2D other) {
    	GameObject gm = other.gameObject;
    	if(gm.name == "Player") {
    	  fadeOut();
    	}
    }

    public void OnTriggerStay2D(Collider2D other) {
    	GameObject gm = other.gameObject;
    	if(gm.name == "Player") {
    		if(state == InteractIndicateState.INTERACT_NULL || state == InteractIndicateState.INTERACT_OUT) {
    	  		fadeIn();
    	  	}
    	}
    }

    // Update is called once per frame
    void Update()
    {
        
        tAt += Time.deltaTime;
        transform.position = new Vector3(startY + 0.5f*Mathf.Sin(tAt), transform.position.y, transform.position.z);

        if(fadeTimer.isOn()) {
            bool finished = fadeTimer.updateTimer(Time.deltaTime);
            float alphaVal = 1;
            if(state == InteractIndicateState.INTERACT_IN) {
            	alphaVal = Mathf.Lerp(0, 1, fadeTimer.getCanoncial());	
            } else if (state == InteractIndicateState.INTERACT_OUT) {
            	alphaVal = Mathf.Lerp(1, 0, fadeTimer.getCanoncial());	
            } else {
            	Assert.IsTrue(false);
            }
            
            spRender.color = new Vector4(spRender.color.r, spRender.color.g, spRender.color.b, alphaVal); 
            if(finished) {
                fadeTimer.turnOff();
            }

        }
    }
}

