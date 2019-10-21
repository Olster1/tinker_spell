using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

public enum UICurrentSelection{
	UI_QUESTS,
	UI_BEASTERY,
	UI_SPELL_SKILL,
	UI_MAP,
	UI_INVETORY,

	////////
	UI_COUNT,


}

public class UISelection : MonoBehaviour
{
	[HideInInspector] public UICurrentSelection currentSelection;
	[HideInInspector] public bool controlling;
	[HideInInspector] private bool isActive;
	private Animator anim;

	public SpriteRenderer[] glowSprites = new SpriteRenderer[5]; 

    [HideInInspector] public bool transitioning;

    [HideInInspector] public UICurrentSelection lastMenuState;

	private bool yIsNew;
	private bool xIsNew;

	public AudioSource moveAudio;

    public Animator gradientBlackBackground;

    public SceneStateManager sceneManager;

	private int yCoord;

    private Timer glowTimer;
	[HideInInspector] public LevelStateId gameWorldLevelState;

	public BeastryJournal beastJournal;
	public SkillSectiom spellSkillery;
    public Journal questLog;
    public MiniMap miniMap;

    private IBlurInterface[] blurInterfaces = new IBlurInterface[(int)UICurrentSelection.UI_COUNT];

    public GameObject uiHud;

    private BlurPostProcess blurPostProcess;
    private bool menuIsOn;

    private Transform originalParent;

    public Transform backingGradient;

    private Vector3 backingOriginalScale;
    // Start is called before the first frame update
    void Start()
    {
        backingOriginalScale = backingGradient.localScale;
        blurPostProcess = Camera.main.GetComponent<BlurPostProcess>();
        anim = gameObject.GetComponent<Animator>();
        glowTimer = new Timer(1.0f);
        glowTimer.turnOff();
        originalParent = transform.parent;
        lastMenuState = UICurrentSelection.UI_QUESTS;
        menuIsOn = false;

        blurInterfaces[(int)UICurrentSelection.UI_QUESTS] = questLog;
        blurInterfaces[(int)UICurrentSelection.UI_BEASTERY] = beastJournal;
        blurInterfaces[(int)UICurrentSelection.UI_SPELL_SKILL] = spellSkillery;
        blurInterfaces[(int)UICurrentSelection.UI_MAP] = miniMap;
    }

    public void Display(UICurrentSelection s_, bool control, Transform parentT) {
    	int s = (int)s_;
    	if(isActive) {
            Debug.Log("Is Active already");
		} else {
			for(int i = 0; i < glowSprites.Length; ++i) {
				glowSprites[i].enabled = false;
			}
			isActive = true;
            gradientBlackBackground.SetTrigger("fadeIn");
			// anim.SetTrigger("Show");

            float height = (Camera.main.orthographicSize / sceneManager.defaultOrthoSize);
            transform.localScale = Vector3.one * height;
            backingGradient.localScale = backingOriginalScale * height;
		}

		currentSelection = s_;
		Assert.IsTrue(s < (int)UICurrentSelection.UI_COUNT);

        if(parentT != null) {
            transform.parent = parentT;  
            transform.localPosition = new Vector3(0, 0, 0);  
        }
        

		controlling = control;
		if(control) {
			glowSprites[s].enabled = true;	
		}
		yCoord = s;
    }

    public void SetDefaultParent() {
        transform.parent = originalParent;
    }

    public bool NotTransitioning() {
        return !transitioning;
    }

    public void Hide(Transform parentT) {
        menuIsOn = false;
    	Assert.IsTrue(isActive);
    	isActive = false;
        controlling = false;
        transform.parent = parentT;
        transform.localPosition = new Vector3(0, 0, 0);
        gradientBlackBackground.SetTrigger("fadeOut");
    	// anim.SetTrigger("Hide");
    }

    public void EndControlling() {
    	controlling = false;
    	glowSprites[yCoord].enabled = false;
    }

    public void ExitCurrentJournal(bool toWorld) {
        if(currentSelection == UICurrentSelection.UI_BEASTERY) {
            beastJournal.ExitMenu(toWorld);
        } else if(currentSelection == UICurrentSelection.UI_SPELL_SKILL) {
            spellSkillery.ExitMenu(toWorld);
        } else if(currentSelection == UICurrentSelection.UI_QUESTS) {
            questLog.ExitMenu(toWorld);
        } else if(currentSelection == UICurrentSelection.UI_MAP) {
            miniMap.ExitMenu(toWorld);
        }
    }
    // Update is called once per frame
    void Update()
    {

        if(Input.GetButtonDown("Submit")) {
            if(sceneManager.IsInGame() && NotTransitioning()) {
                if(!menuIsOn) { 
                    menuIsOn = true;
                    uiHud.SetActive(false);

                    Debug.Log(lastMenuState);
                    blurPostProcess.StartBlur(blurInterfaces[(int)lastMenuState], 0);
                }
            } else {
                ExitCurrentJournal(true);
            }
        }

        if(controlling) {
        	Assert.IsTrue(isActive);

        	float yAxis = Input.GetAxis("Vertical");
        	float xAxis = Input.GetAxis("Horizontal");
        	float threshold = 0.5f;
        	float lowerThreshold = 0.25f;
        	
        	bool upKeyDown = Input.GetKeyDown(KeyCode.UpArrow);
        	bool downKeyDown = Input.GetKeyDown(KeyCode.DownArrow);
        	bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);

        	if(yAxis < -threshold || downKeyDown) {
        		if(yIsNew || downKeyDown) {
        			yCoord++;
        			if(yCoord >= glowSprites.Length) {
        				yCoord--;
        			} else {
        				glowSprites[yCoord - 1].enabled = false;
        				glowSprites[yCoord].enabled = true;
        				moveAudio.Play();
        			}
        			yIsNew = false;
        		}
        	} 

        	if(yAxis > threshold || upKeyDown) {
        		if(yIsNew || upKeyDown) {
        			yCoord--;
        			if(yCoord < 0) {
        				yCoord = 0;
        			} else {
        				glowSprites[yCoord + 1].enabled = false;
        				glowSprites[yCoord].enabled = true;
                        glowTimer.turnOn();
        				moveAudio.Play();
        			}
        			yIsNew = false;
        		}
        	} 

        	if(xAxis > threshold || rightKeyDown) {
        		if(xIsNew || rightKeyDown) {
    				EndControlling();
    				moveAudio.Play();
        			xIsNew = false;
        			if(currentSelection == UICurrentSelection.UI_BEASTERY) {
        				beastJournal.GetFocus();
        			} else if(currentSelection == UICurrentSelection.UI_SPELL_SKILL) {
        				spellSkillery.GetFocus();
        			} else if(currentSelection == UICurrentSelection.UI_QUESTS) {
                        questLog.GetFocus();
                    } else if(currentSelection == UICurrentSelection.UI_MAP) {
                        miniMap.GetFocus();
                    }
        		}
        	} 

        	if(yAxis < lowerThreshold && yAxis > -lowerThreshold) {
        		yIsNew = true;
        	}

        	if(xAxis < lowerThreshold  && xAxis > -lowerThreshold) {
        		
        		xIsNew = true;
        	}

        	if(Input.GetButtonDown("Jump") && currentSelection != (UICurrentSelection)yCoord) {
        		EndControlling();

        		ExitCurrentJournal(false);

        		if((UICurrentSelection)yCoord == UICurrentSelection.UI_BEASTERY) {
        			beastJournal.EnterMenu();
                    lastMenuState = UICurrentSelection.UI_BEASTERY;
        		} else if((UICurrentSelection)yCoord == UICurrentSelection.UI_SPELL_SKILL) {
        			spellSkillery.EnterMenu();
                    lastMenuState = UICurrentSelection.UI_SPELL_SKILL;
        		} else if((UICurrentSelection)yCoord == UICurrentSelection.UI_QUESTS) {
                    questLog.EnterMenu();
                    lastMenuState = UICurrentSelection.UI_QUESTS;
                } else if((UICurrentSelection)yCoord == UICurrentSelection.UI_MAP) {
                    miniMap.EnterMenu();
                    lastMenuState = UICurrentSelection.UI_MAP;
                }
        		
        		currentSelection = (UICurrentSelection)yCoord;
        	}


        }

        if(glowTimer.isOn()) {
            bool finished = glowTimer.updateTimer(Time.unscaledDeltaTime);
            float colorVal = (float)-Mathf.Cos(2*Mathf.PI*glowTimer.getCanoncial()) + 1.0f;
            glowSprites[yCoord].transform.localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 1, 1), colorVal);
            if(finished) {
                if(controlling) {
                    glowTimer.turnOn();
                } else {
                    glowTimer.turnOff();
                }
                
            }
        }
    }
}
