using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;

public class TextWriter : MonoBehaviour
{	
	private Text textObject;
	[HideInInspector] public string[] stringArray;
	private int stringAt;
	private Timer showTimer;
	private bool showText;
	private float charTimeLength;
	private int lastCharCount;
	public float timeToShow;
	private Color startColor;
	private uint hexColor;
	public Animator aButtonAnimator;
	public Animator quoteAnimator;
	private Timer fadeOutTimer;

	public class GlyphInfo {
		public float alphaAt;
		public float sizeAt;
		public Timer timer;
		public uint color;

		public void update(float dt) {
			if(timer.isOn()) {
				bool fin = timer.updateTimer(dt);
				float f = timer.getCanoncial();
				sizeAt = Mathf.Lerp(12, 15, f);
				alphaAt = Mathf.Lerp(0.0f, 1.0f, f);

				color = color | (uint)(255*alphaAt);
				if(fin) {
					timer.turnOff();
				}
			}
		}
	}

	private GlyphInfo[] glyphInfos;

	public void ClearFontWriter() {
		textObject.text = stringArray[0];
		textObject.color = startColor;
		initGlyphInfos();
	}


	public void ActivateFontWriter() {
		showTimer = new Timer(timeToShow);
        textObject.text = stringArray[stringAt];

        showTimer.turnOn();
        showText = true;
        charTimeLength = showTimer.period / (stringArray[stringAt].Length);
        lastCharCount = 0;
        initGlyphInfos();
	}

	public void CancelFontWriting() {
		showText = false;
    	showTimer.turnOff();
    	textObject.text = stringArray[0];
    	textObject.color = startColor;
    	stringAt = 0;
    	initGlyphInfos(); 

	}

	void initGlyphInfos() {
		glyphInfos = new GlyphInfo[stringArray[stringAt].Length];
		for(int i = 0; i < glyphInfos.Length; ++i) {
			glyphInfos[i] = new GlyphInfo();
			glyphInfos[i].timer = new Timer(0.4f);
			glyphInfos[i].color = hexColor;
			glyphInfos[i].sizeAt = 15;
		}
	}
    // Start is called before the first frame update
    void Start()
    {
    	textObject = gameObject.GetComponent<Text>();
    	stringAt = 0;

    	startColor = textObject.color;
    	hexColor = ((uint)(255*startColor.r) << 24) | ((uint)(255*startColor.g) << 16) | ((uint)(255*startColor.b) << 8) | ((uint)(255*startColor.a) << 0);

    	fadeOutTimer = new Timer(0.4f);
    	fadeOutTimer.turnOff();
    	initGlyphInfos();
    }

    // Update is called once per frame
    void Update()
    {
    	if(showText) {

    		float alphaValue = 0;
    		if(fadeOutTimer.isOn()) {
    			bool fin = fadeOutTimer.updateTimer(Time.deltaTime);
    			alphaValue = fadeOutTimer.getCanoncial();

    			if(fin) {
    				stringAt++;
    				ActivateFontWriter();
    			}
    		}
    		string newString = "";
    		if(showTimer.isOn()) {
    			Debug.Log("show time in");
	    		bool finished = showTimer.updateTimer(Time.deltaTime);
	    		int numOfCharacters = (int)(showTimer.tAt / charTimeLength);
	    		if(numOfCharacters < 0) {
	    			numOfCharacters = 0;
	    		}
	    		Debug.Log(lastCharCount);
	    		if(numOfCharacters != lastCharCount) {
	    			for(int j = lastCharCount; j <= numOfCharacters; ++j) {
	    				glyphInfos[j].timer.turnOn();	
	    			}
	    			
	    			lastCharCount = numOfCharacters;
	    		}
	    		if(finished) {
	    			Debug.Log("finished show time in");
	    			textObject.text = stringArray[stringAt];
	    			showTimer.turnOff();
	    			aButtonAnimator.SetTrigger("FadeIn");
	    		}
	    	} else {
	    		if(Input.GetButtonDown("Fire1")) {
	    			Debug.Log("finished show writer");
		    		if(stringAt < (stringArray.Length - 1)) {
	    				
	    				Debug.Log("finished show writer" + stringAt);
	    				fadeOutTimer.turnOn();
	    			} else {
	    				// fadeOutTimer.turnOn();
	    				quoteAnimator.SetTrigger("OffscreenOut");
	    			}
	    		}
	    		
	    	}
    		for(int i = 0; i < stringArray[stringAt].Length; ++i) {
    			glyphInfos[i].update(Time.deltaTime);
    			
    			// Debug.Log(stringToAdd);
    			// string stringToAdd = startString.Substring(i, 1);
    			//
    			//"<size=" + glyphInfos[i].sizeAt + ">" + 
    			uint hexColor;
    			if(fadeOutTimer.isOn()) {
    				hexColor = ((uint)(255*alphaValue) << 0);
				} else {
					hexColor = glyphInfos[i].color;
				}
    			string stringToAdd = "<color=#" + hexColor.ToString("X") + ">" + stringArray[stringAt].Substring(i, 1) + "</color>";
    			// Debug.Log(stringToAdd);
    			newString += stringToAdd;
    		}
	    	textObject.text = newString;
    	} 
    }

}
