using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyGameManager;

public class SkillSectiom : MonoBehaviour, IBlurInterface, IMenuItemInterface
{

    private class GrowthState {
        Timer timer;
        bool bigger;
        Transform t;
        float startScale;

        public GrowthState(Transform t, float startScale) {
            timer = new Timer(0.8f);
            timer.turnOff();
            this.t = t;
            this.startScale = startScale;
        }

        public void Activate(bool bigger) {
            timer.turnOn();
            this.bigger = bigger;
        }

        public void Update(float dt) {
            if(timer.isOn()) {
                bool b = timer.updateTimer(dt);
                float min = t.localScale.x;//bigger ? startScale : startScale + 0.2f;
                Debug.Log(min);
                float max = bigger ? startScale + 0.2f : startScale;
                float f = timer.getCanoncial();
                t.localScale = new Vector3(Mathf.Lerp(min, max, 0.1f), Mathf.Lerp(min, max, 0.1f), 1);
                if(b) {
                    timer.turnOff();
                }
            }
        }
    }

    private GrowthState[] growthStates;
	public SceneStateManager sceneManager;
	private bool isActive;
	public GameObject uiHud;

	private int index;
	private int maxIndex;

    private BeastryJournal beastJournal;

	public Transform[] trans = new Transform[1];
    public ParticleSystem[] bubbles = new ParticleSystem[1]; 

	public float speed; 
    private Timer slideTimer;
    private Timer amberTimer;

    public InstructionCardEvent instructionEvent;

    [HideInInspector] public bool isEnabled;

    private Transform enterTransform;
    private Transform exitTransform;
    public Transform currentPage;
    private Transform lastPage;
    private float hiddenOffset;

    public AudioSource amberDecreaseSound;

    private float[] skillAttributes;

    private BlurPostProcess blurPostProcess;

    public SpriteRenderer blurSprite;

    private bool xIsNew;
    private bool gotFocus;

    public UISelection sideBarMenuSelection;
    // Start is called before the first frame update
    void Start()
    {

        skillAttributes = new float[trans.Length];
        maxIndex = trans.Length;
        slideTimer = new Timer(0.5f);
        blurPostProcess = Camera.main.GetComponent<BlurPostProcess>();
        amberTimer = new Timer(0.1f);
        amberTimer.turnOff();
        SetLocalAxis(index);
        isEnabled = false;

        growthStates = new GrowthState[maxIndex];

        for(int i = 0; i < growthStates.Length; ++i) {
            growthStates[i] = new GrowthState(trans[i].parent, 1.0f);
        }

        beastJournal = Camera.main.GetComponent<BeastryJournal>();
    }

    public void ExitSkillSection(bool exitToWorld) {
        if(exitToWorld) {
            sideBarMenuSelection.Hide(currentPage);
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


    public void GetFocus() {
        isActive = true;
        growthStates[0].Activate(true);
    }

    public void EnterMenu() {
        Activate(false);
    }

    public void ExitFocus() {
        isActive = false;
        sideBarMenuSelection.Display(UICurrentSelection.UI_SPELL_SKILL, true, null);
    }

    public void ExitMenu(bool toWorld) {
        ExitSkillSection(toWorld);
    }

    public void Activate(bool comingFromWorld) {

        float height = (Camera.main.orthographicSize / sceneManager.defaultOrthoSize);
        currentPage.localScale = Vector3.one * height;
        hiddenOffset = height*sceneManager.defaultSafeZone;

        if(comingFromWorld) {
            blurSprite.enabled = true;
            uiHud.SetActive(false);
            sideBarMenuSelection.gameWorldLevelState = sceneManager.GetCurrentLevelState();    
            sideBarMenuSelection.Display(UICurrentSelection.UI_SPELL_SKILL, false, currentPage);
        }

        index = 0;

        growthStates[index].Activate(true);
        
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

        if(slideTimer.isOn()) {
            bool finished = slideTimer.updateTimer(Time.unscaledDeltaTime);
            if(enterTransform != null) {
                Vector3 pos = Vector3.Lerp(new Vector3(-hiddenOffset, 0, enterTransform.localPosition.z), new Vector3(0, 0, enterTransform.localPosition.z), Mathf.Sin(slideTimer.getCanoncial()*0.5f*Mathf.PI));
                enterTransform.localPosition = pos;
            } 

            if(exitTransform != null) {
                Vector3 pos = Vector3.Lerp(new Vector3(0, 0, exitTransform.localPosition.z), new Vector3(-hiddenOffset, 0, exitTransform.localPosition.z), Mathf.Sin(slideTimer.getCanoncial()*0.5f*Mathf.PI));
                exitTransform.localPosition = pos;
            }

            if(finished) {
                slideTimer.turnOff();
                sideBarMenuSelection.SetDefaultParent();
            }
        }

        if(isActive) {


            ////////////////////////JOY STICK MOVEMENT//////////////// 
            float xAxis = Input.GetAxis("Horizontal");
            float threshold = 0.5f;
            float lowerThreshold = 0.25f;

            
            bool leftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
            bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);
            
            if(!gotFocus) {
                if(xAxis > threshold || rightKeyDown) {
                    if(xIsNew || rightKeyDown) {
                        index++;
                        if(index >= maxIndex) {
                            index--;
                        } else {
                            amberDecreaseSound.Play();
                            growthStates[index - 1].Activate(false);
                            growthStates[index].Activate(true);
                        }

                        xIsNew = false;
                    }
                } 

                
                if(xAxis < -threshold || leftKeyDown) {
                    if(xIsNew || leftKeyDown) {
                        index--;
                        if(index < 0) {
                            index = 0;
                            growthStates[index].Activate(false);
                            ExitFocus();
                        } else {
                            amberDecreaseSound.Play();
                            growthStates[index + 1].Activate(false);
                            growthStates[index].Activate(true);
                        }
                        xIsNew = false;
                    }
                } 
            }

            gotFocus = false;

            //Reset the stick values
            if(xAxis < lowerThreshold  && xAxis > -lowerThreshold) {
                
                xIsNew = true;
            } 
            //////////////////////

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
                       
                        amberTimer.turnOn();
                        amberDecreaseSound.Play();
                    }
                } else {
                    amberTimer.turnOn();    
                }
                 skillAttributes[index] += speed*Time.deltaTime;
                 
        		if(skillAttributes[index] > 1.0f) {
        			skillAttributes[index] = 1.0f;
                    instructionEvent.Run();
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

        for(int gIndex = 0; gIndex < growthStates.Length; ++gIndex) {
            growthStates[gIndex].Update(Time.deltaTime);
        }
    }
}
