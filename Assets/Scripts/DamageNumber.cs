using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

public class DamageNumber : MonoBehaviour
{
	public SpriteRenderer[] spriteRenderers;
	public Sprite[] spriteNumbersMellee;
    public Sprite[] spriteNumbersEarth;
	private Timer aliveTimer;
	public float aliveTime;
	private int numberToDisplay;
	private string type;
	private Vector3 velocity;
	public float speed;
	private Color startColor;

    // Start is called before the first frame update
    void Start()
    {
        // transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    //did this because i think start func is called on instaiate() call, but the number to display isn't set
    public void initializeObject(int damage, string type) {
    	numberToDisplay = damage;
    	
    	this.type = type;

    	aliveTimer = new Timer(aliveTime);         
    	aliveTimer.turnOn();

    	bool hadSigNum = false;
    	//set the number sprites
    	//decrease because we want to see the most significant number
    	for(int i = (spriteRenderers.Length - 1); i >= 0; --i) {
    		SpriteRenderer renderer = spriteRenderers[i];
    		int exponent = i;
    		int retrievedNumber = numberToDisplay % (int)(Mathf.Pow(10, exponent + 1));
    		retrievedNumber = retrievedNumber / (int)(Mathf.Pow(10, exponent));
    		
    		if(retrievedNumber == 0) {
    			if(!hadSigNum) {
    				renderer.enabled = false;
    				continue;	
    			}
    			
    		} else {
    			hadSigNum = true;
    		}
    		Assert.IsTrue(retrievedNumber >= 0 && retrievedNumber < 10);
            if(type.Equals("mellee")) {
                renderer.sprite = spriteNumbersMellee[retrievedNumber];
            } else if (type.Equals("earth")) {
                renderer.sprite = spriteNumbersMellee[retrievedNumber];
                // renderer.sprite = spriteNumbersEarth[retrievedNumber];
            } else {
                renderer.sprite = spriteNumbersMellee[retrievedNumber];
            }

    		
    		startColor = renderer.color;

    	}
    	velocity = new Vector3(0, 1, 0);
    	velocity *= speed;

    }

    // Update is called once per frame
    void Update()
    {
     	   
    }

    void FixedUpdate() {
    	if(aliveTimer.isOn()) {
    		transform.position = transform.position + Time.fixedDeltaTime*velocity;

    		bool finished = aliveTimer.updateTimer(Time.fixedDeltaTime);
    		//update the alpha values
    		for(int i = 0; i < spriteRenderers.Length; ++i) {
    			SpriteRenderer renderer = spriteRenderers[i];
    			float canVal = aliveTimer.getCanoncial();
    			startColor.a = Mathf.Lerp(1, 0, canVal);
    			renderer.color = startColor;
    		}
    		if(finished) {
    			Destroy(gameObject);
    		}
    	} else {
    		Assert.IsTrue(false);
   		}
    } 
}
