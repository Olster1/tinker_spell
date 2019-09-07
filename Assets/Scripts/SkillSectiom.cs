using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyGameManager;

public class SkillSectiom : MonoBehaviour, IBlurInterface
{

	public SceneStateManager sceneManager;
	private bool isActive;
	public GameObject uiHud;

	private int index;
	public int maxIndex;

	public Transform[] trans = new Transform[1];
    public ParticleSystem[] bubbles = new ParticleSystem[1]; 

	public float speed; 
    private Timer slideTimer;
    private Timer amberTimer;

    private Transform enterTransform;
    private Transform exitTransform;
    public Transform currentPage;
    private Transform lastPage;
    private float hiddenOffset = 37;

    public AudioSource amberDecreaseSound;

    private float[] skillAttributes = new float[1]; 

	private LevelStateId lastLevelState;

    private BlurPostProcess blurPostProcess;

    public SpriteRenderer blurSprite;
    // Start is called before the first frame update
    void Start()
    {
        slideTimer = new Timer(0.5f);
        blurPostProcess = Camera.main.GetComponent<BlurPostProcess>();
        amberTimer = new Timer(0.1f);
        amberTimer.turnOff();
        SetLocalAxis(index);
    }

    public void ExitSkillSection() {
		slideTimer.turnOn();
		uiHud.SetActive(true);
		sceneManager.useSpawnPoint = false;
		sceneManager.ChangeSceneWithId(lastLevelState);
		exitTransform = currentPage;
        enterTransform = null;
		isActive = false;
        blurSprite.enabled = false;
    }


    public void EnterSkillSection() {
        blurPostProcess.StartBlur(this);

    }

    public void Activate() {
        blurSprite.enabled = true;
    	uiHud.SetActive(false);
    	lastLevelState = sceneManager.stateToLoad;
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
                ExitSkillSection();
            }
        }
    }
}
