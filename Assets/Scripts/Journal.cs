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
	public JournalItem(string title, string synopsis, bool completed, int id) {
    	this.title = title;
    	this.synopsis = synopsis;
    	this.completed = completed;
        this.id = id;
	}
}

public class Journal : MonoBehaviour, IMenuItemInterface
{
	public List<JournalItem> items;
	public Animator journalAnimator;
    private Timer slideTimer;

    private bool isActive;
    private bool gotFocus;
    private Timer glowTimer;

    public SpriteRenderer blurSprite;
    public UISelection sideBarMenuSelection;

    public AudioSource pageRustleAudio;
    public SceneStateManager sceneManager;

    private Transform enterTransform;
    private Transform exitTransform;

    public Transform listOfQuests;
    public Transform singleQuest;
    public Transform listOfQuestsCanvas;

    private float yIndex;
    private float pageOffset;

    public Text emptyQuestsText;

    private Transform currentPage;

    public GameObject journalItemPrefab;
    public GameObject uiHud;

    public float hiddenOffset;
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
    }

    public void EnterMenu() {
       slideTimer.turnOn();
       enterTransform = null;
       exitTransform = null;       
       pageRustleAudio.Play();

       currentPage = enterTransform = listOfQuests;
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
       }

       if(items.Count == 0) {
         emptyQuestsText.text = "You have no quests at the moment.";
       } else {
         emptyQuestsText.text = "";
       }

       yIndex = 0;
       pageOffset = 0;
       glowTimer.turnOn();
       
       sceneManager.useSpawnPoint = false;
       sceneManager.ChangeSceneWithId(LevelStateId.LEVEL_QUESTS);
       isActive = true;
    }

    public void ExitMenu() {
        ExitJournal(false);
    }

    public void ExitJournal(bool exitWholeJournal) {
        if(exitWholeJournal) {
            blurSprite.enabled = false;    
            uiHud.SetActive(true);
            sceneManager.useSpawnPoint = false;
            sceneManager.ChangeSceneWithId(sideBarMenuSelection.gameWorldLevelState);
            sideBarMenuSelection.Hide();
        }
        
        enterTransform = null;
        pageRustleAudio.Play();
        slideTimer.turnOn();
        exitTransform = currentPage;
        isActive = false;
    }

    public void ExitFocus() {
       sideBarMenuSelection.Display(UICurrentSelection.UI_QUESTS, true);
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
            }
        }

        if(isActive) {
            if(Input.GetButtonDown("Jump")) {
               
            }

            if(Input.GetButtonDown("Fire1")) {
                ExitJournal(true);
            }
        }

        if(glowTimer.isOn()) {
            bool finished = glowTimer.updateTimer(Time.unscaledDeltaTime);
            // SpriteRenderer sp = GetIndicator(xCoord, yCoord);
            // float colorVal = (float)-Mathf.Cos(2*Mathf.PI*glowTimer.getCanoncial()) + 1.0f;
            // sp.color = Vector4.Lerp(Color.white, Color.yellow, colorVal);
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
