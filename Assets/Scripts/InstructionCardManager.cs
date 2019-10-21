using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public enum InstructionCardType {
	SPELL_JOINED, 
	SPELL_LEVEL_UP, 


	///
	CARD_COUNT
}

[System.Serializable]
public class InstructionCard {
	public Transform[] Ts;
	public Transform parentT;
	public SliderIndicatorController sliderController;
}

public class InstructionCardManager : MonoBehaviour, IBlurInterface
{

	private InstructionCardType typeToLookAt;
	public InstructionCard[] instructionCards = new InstructionCard[(int)InstructionCardType.CARD_COUNT];

	public GameObject uiHud;

	private Timer autoTimer;

	private Transform enterTransform;
	private Transform exitTransform;

	private Timer slideTimer;

	public SceneStateManager sceneManager;

    public SpriteRenderer blurSprite;

    private float hiddenOffset;

    private LevelStateId gameWorldLevelState;

    private int indexAt;
    private int currentIndexMax;

    private bool isActive;

    private Transform currentTransform;

    private bool xIsNew;

    private float sideParameter;

    // Start is called before the first frame update
    void Start()
    {
        currentTransform = null;

        autoTimer = new Timer(1.0f);
        autoTimer.turnOff();

        float height = (Camera.main.orthographicSize / sceneManager.defaultOrthoSize);

        slideTimer = new Timer(0.5f);
    	slideTimer.turnOff();
    	isActive = false;
    }

    void GoToNextCard(float sideP) {
    	sideParameter = sideP;
    	slideTimer.turnOn();
    	enterTransform = instructionCards[(int)typeToLookAt].Ts[indexAt];
    	exitTransform = currentTransform;   
    	currentTransform = enterTransform;  
    	SliderIndicatorController sc = instructionCards[(int)typeToLookAt].sliderController;
    	if(sc != null) {
			sc.UpdateIndicator(indexAt);  
			sc.LoseParent();
		} else{
			autoTimer.turnOn();
		}
    }

    void ExitCards() {

        blurSprite.enabled = false;    
        
        sceneManager.useSpawnPoint = false;
        sceneManager.ChangeSceneWithId(gameWorldLevelState);
		sideParameter = 1.0f;
		slideTimer.turnOn();
		enterTransform = null;
		exitTransform = currentTransform; 
		SliderIndicatorController sc = instructionCards[(int)typeToLookAt].sliderController;
		if(sc != null) {
			sc.SetParent(exitTransform);
			uiHud.SetActive(true);	
		} else {

		}
		     
		isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
	    if(slideTimer.isOn()) {

	        bool finished = slideTimer.updateTimer(Time.unscaledDeltaTime);
	        if(enterTransform != null) {
	        	Vector3 pos = Vector3.Lerp(new Vector3(sideParameter*hiddenOffset, 0, enterTransform.localPosition.z), new Vector3(0, 0, enterTransform.localPosition.z), Mathf.Sin(slideTimer.getCanoncial()*0.5f*Mathf.PI));
	       		enterTransform.localPosition = pos;
	        } 

	        if(exitTransform != null) {
	        	Vector3 pos = Vector3.Lerp(new Vector3(0, 0, exitTransform.localPosition.z), new Vector3(-sideParameter*hiddenOffset, 0, exitTransform.localPosition.z), Mathf.Sin(slideTimer.getCanoncial()*0.5f*Mathf.PI));
	        	exitTransform.localPosition = pos;
	        }

	        if(finished) {
	            slideTimer.turnOff();
	        }
	    }

    	if(isActive) {

    		if(autoTimer.isOn()) {
    			bool b = autoTimer.updateTimer(Time.unscaledDeltaTime);
    			if(b) {
    				autoTimer.turnOff();
    				if(indexAt == (currentIndexMax - 1)) {
    					ExitCards();
    				} else {
    					GoToNextCard(1);	
    				}
    				
    			}
    		}
    		float xAxis = Input.GetAxis("Horizontal");
    		float threshold = 0.5f;
    		float lowerThreshold = 0.25f;

    		bool leftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
    		bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);
    		if(!slideTimer.isOn()) {
		    	if(xAxis > threshold || rightKeyDown) {
		    		if(xIsNew || rightKeyDown) {
		    			indexAt++;
		    			if(indexAt >= currentIndexMax) {
		    				indexAt--;
		    			} else {
		    				GoToNextCard(1);
		    			}

		    			xIsNew = false;
		    		}
		    	} 

		    	if(xAxis < -threshold || leftKeyDown) {
		    		if(xIsNew || leftKeyDown) {
		    			indexAt--;
		    			if(indexAt < 0) {
		    				indexAt = 0;
		    			} else {
		    				GoToNextCard(-1);
		    			}
		    			xIsNew = false;
		    		}
		    	} 

		    	if(Input.GetButtonDown("Jump")) {
		    		if(indexAt == (currentIndexMax - 1)) {
		    			ExitCards();  
		    		}	
		    	}
		    }
		    if(xAxis < lowerThreshold  && xAxis > -lowerThreshold) {
		    	xIsNew = true;
		    } 		    	
	    }
    }

    public void SetInstructionCardToLookAt(InstructionCardType t) {
    	typeToLookAt = t;
    }


    public void Activate(bool comingFromWorld) {

        float height = (Camera.main.orthographicSize / sceneManager.defaultOrthoSize);
        instructionCards[(int)typeToLookAt].parentT.localScale = Vector3.one * height;

        hiddenOffset = height*sceneManager.defaultSafeZone;
        
	    blurSprite.enabled = true;
	    gameWorldLevelState = sceneManager.GetCurrentLevelState();
       
       	sideParameter = 1.0f;
        slideTimer.turnOn();
        enterTransform = instructionCards[(int)typeToLookAt].Ts[0];
        exitTransform = null;   
        currentTransform = enterTransform;  

        SliderIndicatorController sc = instructionCards[(int)typeToLookAt].sliderController;
        if(sc != null) {
        	sc.SetParent(enterTransform);	
        } else {
        	autoTimer.turnOn();
        }
        

        indexAt = 0;
        currentIndexMax = instructionCards[(int)typeToLookAt].Ts.Length;
        
        sceneManager.useSpawnPoint = false;
        sceneManager.ChangeSceneWithId(LevelStateId.LEVEL_INSTRUCTION_CARD);
        isActive = true;
        uiHud.SetActive(false);


    }
}
