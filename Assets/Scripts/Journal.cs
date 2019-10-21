using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timer_namespace;

// [System.Serializable]
public class JournalItem {
	public int id;
	public string title;
	public string synopsis;
	public bool completed;
  public Image glowRenderer;

	public JournalItem(string title, string synopsis, bool completed, int id) {
    	this.title = title;
    	this.synopsis = synopsis;
    	this.completed = completed;
        this.id = id;
	}
}

public class Journal : MonoBehaviour, IMenuItemInterface, IBlurInterface
{
	public List<JournalItem> items;
	public Animator journalAnimator;
    private Timer slideTimer;

    private bool isActive;
    private bool gotFocus;
    private Timer glowTimer;
    public AudioSource moveAudio;
    public AudioSource selectAudio;

    public SpriteRenderer blurSprite;
    public UISelection sideBarMenuSelection;

    public AudioSource pageRustleAudio;
    public SceneStateManager sceneManager;

    private Transform enterTransform;
    private Transform exitTransform;

    public Transform listOfQuests;
    public Transform singleQuest;
    public Transform listOfQuestsCanvas;

    public Text singleQuestTitle;
    public Text singleQuestDescription;
    public Image singleQuestFaceImage;
    // public Text singleXpReward;

    private int yIndex;
    private float pageOffset;

    public Text emptyQuestsText;

    private Transform currentPage;

    public GameObject journalItemPrefab;
    public GameObject uiHud;

    private float hiddenOffset;
    private bool xIsNew;
    private bool yIsNew;
    // Start is called before the first frame update
    void Start()
    {
        items = new List<JournalItem>();

        slideTimer = new Timer(0.5f);
        slideTimer.turnOff();
        isActive = false;


        glowTimer = new Timer(1.0f);
        glowTimer.turnOff();
    }

    public bool hasJournalItem(int id) {
        bool result = false;
    	for(int i = 0; i < items.Count && !result; ++i) {
           JournalItem item = items[i];
           if(item.id == id) {
             result = true;
             break;
           }
       }
       return result;
    }

    public void GetFocus() {
        isActive = true;
        glowTimer.turnOn();
        gotFocus = true;
        if(items.Count == 0) {
          ExitFocus();  
        }
    }

    public void Activate(bool fromWorld) {

      slideTimer.turnOn();
      enterTransform = null;
      exitTransform = null;       
      pageRustleAudio.Play();

      currentPage = enterTransform = listOfQuests;

      float height = (Camera.main.orthographicSize / sceneManager.defaultOrthoSize);
      currentPage.localScale = Vector3.one * height;
      singleQuest.localScale = Vector3.one * height;
      hiddenOffset = height*sceneManager.defaultSafeZone;


      if(fromWorld) {
          blurSprite.enabled = true;
          sideBarMenuSelection.gameWorldLevelState = sceneManager.GetCurrentLevelState();    
          sideBarMenuSelection.Display(UICurrentSelection.UI_QUESTS, false, currentPage);
      }
      
      //reset the board
      for(int i = 0; i < items.Count; ++i) {
          JournalItem item = items[i];
          GameObject jItem = Instantiate(journalItemPrefab, listOfQuestsCanvas);
          Vector3 p = jItem.transform.localPosition;
          jItem.transform.localPosition = new Vector3(p.x, p.y - i, p.z);

          JournalItemManager manager = jItem.GetComponent<JournalItemManager>();
          manager.SetName(item.title);
          manager.SetDescription(item.synopsis);
          manager.SetCompletionState(item.completed);
          item.glowRenderer = manager.GetGlowRenderer();
          // manager.SetSpriteFace();
      }

      if(items.Count == 0) {
       emptyQuestsText.text = "You have no quests at the moment.";

      } else {
       glowTimer.turnOn();
       emptyQuestsText.text = "";
      }

      yIndex = 0;
      pageOffset = 0;
      
      sceneManager.useSpawnPoint = false;
      sceneManager.ChangeSceneWithId(LevelStateId.LEVEL_QUESTS);
      isActive = true;


      if(items.Count == 0) {
        ExitFocus();  
      }
    }

    public void EnterMenu() {
      Activate(false);
      
    }

    public void ExitMenu(bool toWorld) {
        ExitJournal(toWorld);
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

    public void ExitFocus() {
       sideBarMenuSelection.Display(UICurrentSelection.UI_QUESTS, true, null);
       if(items.Count > 0) {
        items[yIndex].glowRenderer.color = Color.white;
       }
       
       glowTimer.turnOff();
       isActive = false;
    }

    public void AddJournalItem(string title, string synopsis, int id) {
    	items.Add(new JournalItem(title, synopsis, false, id));
		journalAnimator.SetTrigger("go");
    }

    public void AddJournalItem(JournalItem item) {
    	items.Add(item);
    	journalAnimator.SetTrigger("go");
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
              if(yAxis < -threshold || downKeyDown) {
                if(yIsNew || downKeyDown) {
                  yIndex++;
                  if(yIndex >= items.Count) {
                    yIndex--;
                  } else {
                    moveAudio.Play();
                    items[yIndex].glowRenderer.color = Color.white;
                    glowTimer.turnOn();
                  }
                  yIsNew = false;
                }
              } 

              if(xAxis < -threshold || leftKeyDown) {
                if(xIsNew || leftKeyDown) {
                  ExitFocus();
                  xIsNew = false;
                }
              } 

              
              if(yAxis > threshold || upKeyDown) {
                if(yIsNew || upKeyDown) {
                  yIndex--;
                  if(yIndex < 0) {
                    yIndex = 0;
                  } else {
                    moveAudio.Play();
                    items[yIndex].glowRenderer.color = Color.white;
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
          }

          if(yAxis < lowerThreshold && yAxis > -lowerThreshold) {
            yIsNew = true;
          }

          if(Input.GetButtonDown("Jump") && items.Count > 0) {
            if(currentPage == listOfQuests) {
              slideTimer.turnOn();
              enterTransform = singleQuest;
              exitTransform = currentPage;

              currentPage = singleQuest;
              
              singleQuestTitle.text = items[yIndex].title;
              singleQuestDescription.text = items[yIndex].synopsis;
              // singleQuestFaceImage.sprite = 
              // items[yIndex].completed;
              // singleXpReward.text = ;

              pageRustleAudio.Play();
            }
          }

          if(Input.GetButtonDown("Fire1")) {
            if(currentPage == listOfQuests) {
              ExitJournal(true);
            } else {
              slideTimer.turnOn();
              enterTransform = listOfQuests;
              exitTransform = singleQuest;

              currentPage = listOfQuests;
              pageRustleAudio.Play();
            }
              
          }

          if(glowTimer.isOn() && items.Count > 0) {

              bool finished = glowTimer.updateTimer(Time.unscaledDeltaTime);
              float colorVal = (float)-Mathf.Cos(2*Mathf.PI*glowTimer.getCanoncial()) + 1.0f;
              items[yIndex].glowRenderer.color = Vector4.Lerp(Color.white, Color.yellow, colorVal);
              if(finished) {
                  // sp.color = Color.white;
                  if(isActive) {
                      glowTimer.turnOn();
                  } else {
                      glowTimer.turnOff();
                  }
                  
              }
          }
        }
    }
}
