using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;

public enum BeastId {
	ROCK_GOLLUM,
	RANGE_GOLLUM,
	ROCK_SENTINEL,
	FIRE_COAL,
	HYDRODOG,
	HOWLER_MALE,
	HOWLER_FEMALE,

	/////
	TOTAL_BEAST_COUNT
}

[System.Serializable]
public class BeastItem {
	public string title;

	[TextArea]
	public string synopsis;
	public int highestLevel;
	public bool found;
	public BeastItem(string title, string synopsis, int highestLevel) {
    	this.title = title;
    	this.synopsis = synopsis;
    	this.highestLevel = highestLevel;
    	this.found = false;
	}

	public void UpdateBeastItem(string title, string synopsis, int highestLevel) {
		if(highestLevel > this.highestLevel) {
			this.highestLevel = highestLevel;
		}

    	this.title = title;
    	this.synopsis = synopsis;
    	this.found = true;
	}
}

public class BeastryJournal : MonoBehaviour, IBlurInterface, IMenuItemInterface
{
	[HideInInspector] public bool isActive;
	
	private int xCoord;
	private int yCoord;
	private int pageIndex;

	public AudioSource moveAudio;
	public AudioSource selectAudio;
	public AudioSource pageRustleAudio;

	private bool xIsNew;
	private bool yIsNew;

	public int numOfColums;
	public int numOfRows;

	public Transform[] pages;
	public Transform closeupBeastPage;

	public Text closeupBeastTitle;
	public Text closeupBeastAbout;

	public Animator closeupBeastAnimator;

	public GameObject uiHud;

	private Transform enterTransform;
	private Transform exitTransform;
	private Transform currentPage;
	private Transform lastPage;

	private Timer glowTimer;

	private Timer slideTimer;

	public SceneStateManager sceneManager;

    private SkillSectiom skillSection;

    public SpriteRenderer blurSprite;

    public UISelection sideBarMenuSelection;

	public class CacheTransform {
		public Vector3 scale;
		public Vector3 position;
	}

    private bool gotFocus;
	private CacheTransform[] cachedTransforms;

	// this is the safe zone outside of the camera view to animate towards
	private float hiddenOffset;//game units  

	public Sprite hiddenSprite;
	public BeastItem[] beasts = new BeastItem[(int)BeastId.TOTAL_BEAST_COUNT];
	public GameObject[] objs = new GameObject[(int)BeastId.TOTAL_BEAST_COUNT];
	public Sprite[] defaultSprites = new Sprite[(int)BeastId.TOTAL_BEAST_COUNT];
	public AnimatorOverrideController[] beastAnimators = new AnimatorOverrideController[(int)BeastId.TOTAL_BEAST_COUNT];

	public Animator beastNotificationAnimator;

    void Awake()
    {
       
    }

    void Start() {
    	cachedTransforms = new CacheTransform[(int)BeastId.TOTAL_BEAST_COUNT];

        skillSection = Camera.main.GetComponent<SkillSectiom>();

        

    	for(int i = 0; i < (int)BeastId.TOTAL_BEAST_COUNT; ++i) {
    		CacheTransform item = cachedTransforms[i] = new CacheTransform();
    		Transform tra = objs[i].transform.GetChild(0).gameObject.transform;
    		item.scale = tra.localScale;
    		item.position = tra.localPosition;

    	}
    	enterTransform = null;
    	exitTransform = null;
    	lastPage = null;
    	currentPage = pages[0];

    	glowTimer = new Timer(1.0f);
    	glowTimer.turnOff();

    	slideTimer = new Timer(0.5f);
    	slideTimer.turnOff();
    }

    public bool hasBeastItem(int id) {
    	return false;
    }

    public void AddBeastItem(BeastId id, string title, string synopsis, int level) {
    	if(!beasts[(int)id].found) {
    		beasts[(int)id].UpdateBeastItem(title, synopsis, level);
		}
		beastNotificationAnimator.SetTrigger("go");
    }

    public void FoundBeast(BeastId id) {
    	if(!beasts[(int)id].found) {
    		beastNotificationAnimator.SetTrigger("go");
    	}
    	beasts[(int)id].found = true;
    }

    public void GetFocus() {
        isActive = true;
        glowTimer.turnOn();
        gotFocus = true;
    }

    public void EnterMenu() {
        Activate(false);
    }

    public void ExitMenu(bool exitWholeJournal) {
        ExitJournal(exitWholeJournal);
    }

    public void ExitFocus() {
       sideBarMenuSelection.Display(UICurrentSelection.UI_BEASTERY, true, null);
       glowTimer.turnOff();
       SpriteRenderer sp = GetIndicator(xCoord, yCoord);
       sp.color = Color.white;
       isActive = false;
    }

    public int GetMappedIndex(int x, int y) {
    	int result = x + (y*numOfColums);
    	return result;
    }

    public SpriteRenderer GetIndicator(int x, int y) {
    	int mappedIndex = GetMappedIndex(x, y);
        SpriteRenderer sp = objs[mappedIndex].GetComponent<SpriteRenderer>();
    	return sp;
    }

    public void ExitJournal(bool exitWholeJournal) {

        if(exitWholeJournal) {
            blurSprite.enabled = false;    
            uiHud.SetActive(true);
            sceneManager.useSpawnPoint = false;
            sceneManager.ChangeSceneWithId(sideBarMenuSelection.gameWorldLevelState);
            sideBarMenuSelection.Hide(currentPage);
        }
        
        enterTransform = null;
        pageRustleAudio.Play();
		slideTimer.turnOn();
		exitTransform = currentPage;
		isActive = false;
    }

    public void Activate(bool comingFromWorld) {

        currentPage = pages[0];

        hiddenOffset = sceneManager.defaultSafeZone;
        
        if(comingFromWorld) {
             sideBarMenuSelection.Display(UICurrentSelection.UI_BEASTERY, false, currentPage);
             blurSprite.enabled = true;
             sideBarMenuSelection.gameWorldLevelState = sceneManager.GetCurrentLevelState();
        }
       
        slideTimer.turnOn();
        enterTransform = null;
        exitTransform = null;       
        pageRustleAudio.Play();
        enterTransform = currentPage;

        //reset the board
        for(int i = 0; i < (int)BeastId.TOTAL_BEAST_COUNT; ++i) {
            BeastItem item = beasts[i];
            GameObject spObj = objs[i].transform.GetChild(0).gameObject;
            SpriteRenderer sp = spObj.GetComponent<SpriteRenderer>();
            sp.color = Color.white;
            CacheTransform trans = cachedTransforms[i];
            if(item.found) {
                sp.sprite = defaultSprites[i];
                spObj.transform.localScale = trans.scale;
                spObj.transform.localPosition = trans.position;

            } else {
                sp.sprite = hiddenSprite;
                spObj.transform.localScale = new Vector3(0.1f, 0.1f, 1);
                spObj.transform.localPosition = new Vector3(0, 0, trans.position.z);
            }
        }
        xCoord = 0;
        yCoord = 0;
        glowTimer.turnOn();
        pageIndex = 0;
        
        sceneManager.useSpawnPoint = false;
        sceneManager.ChangeSceneWithId(LevelStateId.LEVEL_JOURNAL);
        isActive = true;
    }
    // Update is called once per frame
    void Update()
    {
    	

	    if(slideTimer.isOn()) {
	        bool finished = slideTimer.updateTimer(Time.unscaledDeltaTime);
	        if(enterTransform != null) {
	        	Vector3 pos = Vector3.Lerp(new Vector3(-hiddenOffset, 0, enterTransform.localPosition.z), new Vector3(0, 0, enterTransform.localPosition.z), Mathf.Sin(slideTimer.getCanoncial()*0.5f*Mathf.PI));
	       		enterTransform.localPosition = pos;
	        } 

	        if(exitTransform != null) {
	        	Vector3 pos = Vector3.Lerp(new Vector3(0, 0, exitTransform.localPosition.z), new Vector3(hiddenOffset, 0, exitTransform.localPosition.z), Mathf.Sin(slideTimer.getCanoncial()*0.5f*Mathf.PI));
	        	exitTransform.localPosition = pos;
	        }

	        if(finished) {
	            slideTimer.turnOff();
                sideBarMenuSelection.SetDefaultParent();
	        }
	    }

        if(isActive) {
        	float xAxis = Input.GetAxis("Horizontal");
        	float yAxis = Input.GetAxis("Vertical");
        	float threshold = 0.5f;
        	float lowerThreshold = 0.25f;

        	
        	bool leftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
        	bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);
        	bool upKeyDown = Input.GetKeyDown(KeyCode.UpArrow);
        	bool downKeyDown = Input.GetKeyDown(KeyCode.DownArrow);
        	
            if(!gotFocus) {
            	if(xAxis > threshold || rightKeyDown) {
            		if(xIsNew || rightKeyDown) {
            			xCoord++;
            			if(xCoord >= numOfColums || GetMappedIndex(xCoord, yCoord) >= (int)BeastId.TOTAL_BEAST_COUNT) {
            				xCoord--;
            			} else {
            				moveAudio.Play();
            				GetIndicator(xCoord - 1, yCoord).color = Color.white;
            				glowTimer.turnOn();
            			}

            			xIsNew = false;
            		}
            	} 

            	
            	if(yAxis < -threshold || downKeyDown) {
            		if(yIsNew || downKeyDown) {
            			yCoord++;
            			if(yCoord >= numOfRows || GetMappedIndex(xCoord, yCoord) >= (int)BeastId.TOTAL_BEAST_COUNT) {
            				yCoord--;
            			} else {
            				moveAudio.Play();
            				GetIndicator(xCoord, yCoord -1).color = Color.white;
            				glowTimer.turnOn();
            			}
            			yIsNew = false;
            		}
            	} 

            	if(xAxis < -threshold || leftKeyDown) {
            		if(xIsNew || leftKeyDown) {
            			xCoord--;
            			if(xCoord < 0) {
            				xCoord = 0;
                            ExitFocus();
            			} else {
            				moveAudio.Play();
            				GetIndicator(xCoord + 1, yCoord).color = Color.white;
            				glowTimer.turnOn();
            			}
            			xIsNew = false;
            		}
            	} 

            	
            	if(yAxis > threshold || upKeyDown) {
            		if(yIsNew || upKeyDown) {
            			yCoord--;
            			if(yCoord < 0) {
            				yCoord = 0;
            			} else {
            				moveAudio.Play();
            				GetIndicator(xCoord, yCoord + 1).color = Color.white;
            				glowTimer.turnOn();
            			}
            			yIsNew = false;
            		}
            	} 
            }

            gotFocus = false;

        	//Reset the stick values
        	if(xAxis < lowerThreshold  && xAxis > -lowerThreshold) {
        		
        		xIsNew = true;
        	} else {
        		// Debug.Log("reset");
        		// Debug.Log(xAxis);
        	}

        	if(yAxis < lowerThreshold && yAxis > -lowerThreshold) {
        		yIsNew = true;
        	}

        	if(Input.GetButtonDown("Jump")) {
        		if(currentPage != closeupBeastPage && beasts[GetMappedIndex(xCoord, yCoord)].found) {
	        		slideTimer.turnOn();
	        		lastPage = currentPage;
	        		enterTransform = closeupBeastPage;
	        		exitTransform = currentPage;

	        		currentPage = closeupBeastPage;
	        		BeastItem item = beasts[GetMappedIndex(xCoord, yCoord)];
	        		closeupBeastTitle.text = item.title;
	        		closeupBeastAbout.text = item.synopsis;

	        		selectAudio.Play();

	        		AnimatorOverrideController newController = beastAnimators[GetMappedIndex(xCoord, yCoord)];
	        		closeupBeastAnimator.runtimeAnimatorController = newController;
        		}
        	}

        	if(Input.GetButtonDown("Fire1")) {
	        	if(currentPage == closeupBeastPage) {
	        		slideTimer.turnOn();
	        		enterTransform = lastPage;
	        		exitTransform = currentPage;

	        		currentPage = lastPage;
        		} else {
        			ExitJournal(true);
        			
        		}
        	}
        }

        if(glowTimer.isOn()) {
            bool finished = glowTimer.updateTimer(Time.unscaledDeltaTime);
            SpriteRenderer sp = GetIndicator(xCoord, yCoord);
            float colorVal = (float)-Mathf.Cos(2*Mathf.PI*glowTimer.getCanoncial()) + 1.0f;
            sp.color = Vector4.Lerp(Color.white, Color.yellow, colorVal);
            if(finished) {
            	sp.color = Color.white;
            	if(isActive) {
            		glowTimer.turnOn();
        		} else {
        			glowTimer.turnOff();
        		}
                
            }
        }
    }
}

