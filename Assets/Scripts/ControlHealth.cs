using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyGameManager;
using Timer_namespace;

public class ControlHealth : MonoBehaviour
{
	public RectTransform trans;
	public float maxScale;
	public float oldScale;
	public float newScale;
	public Vector3 originalScale;
	private Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = trans.localScale;
        timer = new Timer(0.2f);
    }

    // Update is called once per frame
    void Update()
    {
    	if(GameManager.updateHealth) {
    		timer.turnOn();
    		oldScale = trans.localScale.x;
    		newScale = (GameManager.playerHealth/100.0f)*maxScale;
    		GameManager.updateHealth = false;
    	}

    	if(timer.isOn()) {
    		bool fin = timer.updateTimer(Time.deltaTime);
    		float f = timer.getCanoncial();
    		float s = Mathf.Lerp(oldScale, newScale, f);
    		originalScale.x = s;
    		trans.localScale = originalScale;
    		if(fin) {
    			timer.turnOff();
    		}
    	}
        
    }
}
