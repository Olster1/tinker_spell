using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;

public class TextWriter : MonoBehaviour
{	
	private Text textObject;
	private string startString;
	private Timer showTimer;
	private bool showText;
	private float charTimeLength;
	private int lastCharCount;
	public float timeToShow;

	public void ClearFontWriter() {
		textObject.text = "";
	}

	public void ActivateFontWriter() {
		showTimer = new Timer(timeToShow);
        textObject.text = "";
        showTimer.turnOn();
        showText = true;
        charTimeLength = showTimer.period / (startString.Length);
        lastCharCount = 0;
	}

	public void CancelFontWriting() {
		showText = false;
    	showTimer.turnOff();
    	textObject.text = "";
	}
    // Start is called before the first frame update
    void Start()
    {
    	textObject = gameObject.GetComponent<Text>();
    	startString = textObject.text;
    	textObject.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
    	if(showText) {
    		if(showTimer.isOn()) {
	    		bool finished = showTimer.updateTimer(Time.fixedDeltaTime);
	    		int numOfCharacters = (int)(showTimer.tAt / charTimeLength);
	    		if(numOfCharacters < 0) {
	    			numOfCharacters = 0;
	    		}
	    		if(numOfCharacters != lastCharCount) {
	    			textObject.text = startString.Substring(0, numOfCharacters);
	    			lastCharCount = numOfCharacters;
	    		}
	    		if(finished) {
	    			textObject.text = startString;
	    			showTimer.turnOff();
	    		}
	    	}
    	}
    }
}
