using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyGameManager;

public class SkillSectiom : MonoBehaviour, IBlurInterface, IMenuItemInterface
{

	public SceneStateManager sceneManager;
	private bool isActive;
	public GameObject uiHud;

	private int index;
	public int maxIndex;

    private BeastryJournal beastJournal;

	public Transform[] trans = new Transform[1];
    public ParticleSystem[] bubbles = new ParticleSystem[1]; 

	public float speed; 
    private Timer slideTimer;
    private Timer amberTimer;

    [HideInInspector] public bool isEnabled;

    private Transform enterTransform;
    private Transform exitTransform;
    public Transform currentPage;
    private Transform lastPage;
    private float hiddenOffset = 37;

    public AudioSource amberDecreaseSound;

    private float[] skillAttributes = new float[1]; 

    private BlurPostProcess blurPostProcess;

    public SpriteRenderer blurSprite;

    public UISelection sideBarMenuSelection;
    // Start is called before the first frame update
    void Start()
    {
        slideTimer = new Timer(0.5f);
        blurPostProcess = Camera.main.GetComponent<BlurPostProcess>();
        amberTimer = new Timer(0.1f);
        amberTimer.turnOff();
        SetLocalAxis(index);
        isEnabled = false;

        beastJournal = Camera.main.GetComponent<BeastryJournal>();
    }

    public void ExitSkillSection(bool exitToWorld) {
        if(exitToWorld) {
            sideBarMenuSelection.Hide();
            uiHud.SetActive(true);
            sceneManager.useSpawnPoint = false;
            sceneManager.ChangeSceneWithId(sideBarMenuSelection.gameWorldLevelState);
             blurSprite.enabled = false;
        }
        
		slideTimer.turnOn();
		exitTransform = currentPage;
        enterTransform = null;
		isActive = false;
        isEnabled = false;
    }


    public void EnterSkillSection() {
        if(!isEnabled && !beastJournal.isActive) {
            isEnabled = true;
            blurPostProcess.StartBlur(this);    
        }
    }

    public void GetFocus() {
        isActive = true;
    }

    public void EnterMenu() {
        Debug.Log("ENTERING SKILL SECTION");
        Activate(false);
    }

    public void ExitFocus() {
        isActive = false;
        sideBarMenuSelection.Display(UICurrentSelection.UI_SPELL_SKILL, true);
    }

    public void ExitMenu() {
        ExitSkillSection(false);
    }

    public void Activate(bool comingFromWorld) {
        if(comingFromWorld) {
            blurSprite.enabled = true;
            uiHud.SetActive(false);
            sideBarMenuSelection.gameWorldLevelState = sceneManager.GetCurrentLevelState();    
            sideBarMenuSelection.Display(UICurrentSelection.UI_SPELL_SKILL, false);
        }
        
		sceneManager.useSpawnPoint = false;
		sceneManager.ChangeSceneWithId(LevelStateId.LEVEL_SKILL_TILES);
		enterTransform = currentPage;
        exitTransform = null;
        slideTimer.turnOn();

		isActive = true;
    }

    void SetLocalAxis(int i) {

        float yAxis = Mathf.Lerp(0.2f, 1.36f, skillAttributes[i]);

        Vector3 s = trans[i].localScale;
        s.y = yAxis;
        trans[i].localScale = s;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetButtonDown("Cancel")) {
            if(isEnabled) {
                ExitSkillSection(true);
            } else {
                EnterSkillSection();
            }
        }
        if(slideTimer.isOn()) {
            bool finished = slideTimer.updateTimer(Time.unscaledDeltaTime);
            if(enterTransform != null) {
                Vector3 pos = Vector3.Lerp(new Vector3(-hiddenOffset, 0, enterTransform.localPosition.z), new Vector3(0, 0, enterTransform.localPosition.z), Mathf.Sin(slideTimer.getCanoncial()*0.5f*Mathf.PI));
                enterTransform.localPosition = pos;
                Debug.Log("SPELL SKILL TREE");
            } 

            if(exitTransform != null) {
                Vector3 pos = Vector3.Lerp(new Vector3(0, 0, exitTransform.localPosition.z), new Vector3(-hiddenOffset, 0, exitTransform.localPosition.z), Mathf.Sin(slideTimer.getCanoncial()*0.5f*Mathf.PI));
                exitTransform.localPosition = pos;
            }

            if(finished) {
                slideTimer.turnOff();
            }
        }

        if(isActive) {

        // 	bool leftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
        // 	bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);

            // for(int i = 0; i < bubbles.Length; ++i) {
            //     ParticleSystem.MainModule m = bubbles[i].main;
            //     m.loop = false;
            // }

        	if(Input.GetButton("Fire1") && GameManager.amberCount > 0 && skillAttributes[index] < 1.0f) {
                if(amberTimer.isOn()) {
                    bool b = amberTimer.updateTimer(Time.deltaTime);    
                    if(b) {
                        GameManager.amberCount--;
                        skillAttributes[index] += speed*Time.deltaTime;
                        amberTimer.turnOn();
                        amberDecreaseSound.Play();
                    }
                } else {
                    amberTimer.turnOn();    
                }
                 
        		if(skillAttributes[index] > 1.0f) {
        			skillAttributes[index] = 1.0f;
        		}

        		SetLocalAxis(index);

                if(!bubbles[index].isPlaying) {
                    bubbles[index].Play();
                }
                ParticleSystem.MainModule m = bubbles[index].main;
                m.loop = true;
        	} else {
                ParticleSystem.MainModule m = bubbles[index].main;
                m.loop = false;
            }

            

            if(Input.GetButton("Fire2")) {
                ExitSkillSection(true);
            }
        }
    }
}
