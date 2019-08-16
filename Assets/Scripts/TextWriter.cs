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
	public bool showText;
	private int lastCharCount;
	public float timeToShow;
	private Color startColor;
	private uint hexColor;
	public Animator aButtonAnimator;
	public Animator quoteAnimator;
	private Timer fadeOutTimer;
	public bool isOn;
	public PlayerMovement player;
	public AudioSource audioSrc;

	[HideInInspector] public AudioClip[] clips;

	[HideInInspector] public ActivateQuote currentQuote;


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

	public void AddJournalItem() {
		currentQuote.EndQuote();
	}


	public void ActivateFontWriter() {
		showTimer = new Timer(timeToShow*(stringArray[stringAt].Length));
		
        textObject.text = stringArray[stringAt];

        showTimer.turnOn();
        showText = true;
        lastCharCount = 0;
        initGlyphInfos();
        //Debug.Log(currentQuote.unfreezePlayer);
        if(!currentQuote.unfreezePlayer) {
        	Debug.Log("unfreeze player");
        	player.canControlPlayer = false;
        	Time.timeScale = 0.0f;
        }
        

        if(clips.Length > 0) {
        	audioSrc.clip = clips[stringAt];
        	audioSrc.Play();
        }
	}

	public void CancelFontWriting() {
		showText = false;
    	showTimer.turnOff();
    	textObject.text = stringArray[0];
    	textObject.color = startColor;
    	stringAt = 0;
    	initGlyphInfos(); 
    	player.canControlPlayer = true;
    	Time.timeScale = 1.0f;
    	audioSrc.Stop();

	}

	void initGlyphInfos() {
		glyphInfos = new GlyphInfo[stringArray[stringAt].Length];
		for(int i = 0; i < glyphInfos.Length; ++i) {
			glyphInfos[i] = new GlyphInfo();
			glyphInfos[i].timer = new Timer(0.1f);
			glyphInfos[i].color = hexColor;
			glyphInfos[i].sizeAt = 15;
		}
	}
    // Start is called before the first frame update
    void Start()
    {

    // 	List<string> forcesToAdd = new List<string>();
	    
	   //  forcesToAdd.Add("apple");
	   //  forcesToAdd.Add("banana");
	   //  forcesToAdd.Add("carrot");
	   //  forcesToAdd.Add("danish");
	   //  forcesToAdd.Add("elephant");

	   //  for(int i = 0; i < forcesToAdd.Count; ) {
	   //  	Debug.Log(forcesToAdd[i]);
	   //  	bool increment = true;
	   //  	if(i == 1) {
				// forcesToAdd.RemoveAt(i);	  
				// increment = false;  		
	   //  	}

	   //  	if(increment) ++i;
	   //  }



    	textObject = gameObject.GetComponent<Text>();
    	stringAt = 0;

    	startColor = textObject.color;
    	hexColor = ((uint)(255*startColor.r) << 24) | ((uint)(255*startColor.g) << 16) | ((uint)(255*startColor.b) << 8) | ((uint)(255*startColor.a) << 0);

    	fadeOutTimer = new Timer(0.4f);
    	fadeOutTimer.turnOff();
    	// initGlyphInfos();
    }

    // Update is called once per frame
    void Update()
    {
    	if(showText) {
    		
    		float alphaValue = 1.0f;
    		if(fadeOutTimer.isOn()) {
    			bool fin = fadeOutTimer.updateTimer(Time.unscaledDeltaTime);
    			alphaValue = 1.0f - fadeOutTimer.getCanoncial();

    			if(fin) {
    				stringAt++;
    				ActivateFontWriter();
    				fadeOutTimer.turnOff();
    			}
    		}
    		string newString = "";
    		if(showTimer.isOn()) {
    			float dt = Time.unscaledDeltaTime;
    			int numOfCharacters = 0;
    			

	    		bool finished = showTimer.updateTimer(dt);

	    		if(Input.GetButtonDown("Jump") && !currentQuote.unfreezePlayer) {
    				showTimer.tAt = showTimer.period;
    				finished = true;	
    			} 

	    		numOfCharacters = (int)(showTimer.getCanoncial()*(stringArray[stringAt].Length - 1));
	    		if(numOfCharacters < 0) {
	    			numOfCharacters = 0;
	    		}
	    		
	    		if(numOfCharacters != lastCharCount) {
	    			for(int j = lastCharCount; j <= numOfCharacters; ++j) {
	    				glyphInfos[j].timer.turnOn();	
	    			}
	    			
	    			lastCharCount = numOfCharacters;
	    		}
	    		if(finished) {
	    			textObject.text = stringArray[stringAt];
	    			showTimer.turnOff();
	    			aButtonAnimator.SetTrigger("FadeIn");
	    		}
	    	} else {
	    		if(Input.GetButtonDown("Jump") || currentQuote.unfreezePlayer) {
	    			if(!fadeOutTimer.isOn()) {
			    		if(stringAt < (stringArray.Length - 1)) {
			    			
		    				

		    				// Debug.Log("finished show writer" + stringAt);
		    				fadeOutTimer.turnOn();
		    			} else {
		    				// fadeOutTimer.turnOn();

		    				if(currentQuote.toActivate != null) {
		    					currentQuote.toActivate.SetActive(true);
		    					currentQuote.gameObject.SetActive(false);
		    					currentQuote.toDeactivate.SetActive(false);
		    				}
		    				
		    				quoteAnimator.SetTrigger("OffscreenOut");
		    				CancelFontWriting();
		    			}
		    		}
	    		}
	    		
	    	}
    		for(int i = 0; i < stringArray[stringAt].Length; ++i) {
    			glyphInfos[i].update(Time.unscaledDeltaTime);
    			
    			// Debug.Log(stringToAdd);
    			// string stringToAdd = startString.Substring(i, 1);
    			//
    			//"<size=" + glyphInfos[i].sizeAt + ">" + 
    			uint hexColor1;
    			if(fadeOutTimer.isOn()) {
    				hexColor1 = hexColor | ((uint)(255*alphaValue) << 0);
				} else {
					hexColor1 = glyphInfos[i].color;
				}
    			string stringToAdd = "<color=#" + hexColor1.ToString("X") + ">" + stringArray[stringAt].Substring(i, 1) + "</color>";
    			// Debug.Log(stringToAdd);
    			newString += stringToAdd;
    		}
	    	textObject.text = newString;
    	} 
    }

}
