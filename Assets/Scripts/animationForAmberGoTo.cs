using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyGameManager;

public class animationForAmberGoTo : MonoBehaviour
{
	public Vector2 finalPos;
	private Vector2 startPos;
	private RectTransform UI_Element;
	private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        
        timer = new Timer(0.3f);
        timer.turnOn();
    }

    public void SetStartPos(Vector2 pos) {
    	UI_Element = gameObject.GetComponent<RectTransform>();
    	UI_Element.anchoredPosition = pos;
    	startPos = pos;

    }

    // Update is called once per frame
    void Update()
    {
    	if(timer.isOn()) {
    		bool fin = timer.updateTimer(Time.deltaTime);
    		float canVal = timer.getCanoncial();
    		UI_Element.anchoredPosition = Vector2.Lerp(startPos, finalPos, canVal);
    		if(fin) {
    			GameManager.amberCount++;
    			Destroy(gameObject);
    		}	
    	}
        
    }
}
