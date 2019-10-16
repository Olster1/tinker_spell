using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

public enum MiniMapType {
	MINI_MAP_TELEPORTER_ROOM,
	MINI_MAP_HUB, 


	//
	MINI_MAP_COUNT,

}
public class MiniMap : MonoBehaviour, IMenuItemInterface, IBlurInterface
{
	public SpriteRenderer sp; 

	public Transform playerIcon;

	private bool isActive;
	private Timer slideTimer;
	private bool gotFocus = false;

	public SpriteRenderer blurSprite;
	public UISelection sideBarMenuSelection;

	public AudioSource pageRustleAudio;
	public SceneStateManager sceneManager;

	private Transform enterTransform;
	private Transform exitTransform;

	public Transform mapTransform;

	public Transform currentMapT;

	public GameObject uiHud;

	private bool xIsNew = false;

	private float zoomSpeed;
	private bool wasDownInMap;

	private float hiddenOffset = 37;

	private Vector3 mapOffset;


	public Sprite[] maps = new Sprite[(int)MiniMapType.	MINI_MAP_COUNT];
    // Start is called before the first frame update
    void Start()
    {
        slideTimer = new Timer(0.5f);
        slideTimer.turnOff();
        isActive = false;
        zoomSpeed = 2;
    }

    public void GetFocus() {
        isActive = true;
        gotFocus = true;
        wasDownInMap = false;
    }

    public void Activate(bool fromWorld) {
    	if(fromWorld) {
		    blurSprite.enabled = true;
		    uiHud.SetActive(false);
		    sideBarMenuSelection.gameWorldLevelState = sceneManager.GetCurrentLevelState();    
		    sideBarMenuSelection.Display(UICurrentSelection.UI_MAP, false, mapTransform);
    	}
    	slideTimer.turnOn();
    	enterTransform = null;
    	exitTransform = null;       
    	pageRustleAudio.Play();
    	wasDownInMap = false;

    	enterTransform = mapTransform;
    	
    	sceneManager.useSpawnPoint = false;
    	sceneManager.ChangeSceneWithId(LevelStateId.LEVEL_MINI_MAP);
    	isActive = true;

    	currentMapT.localScale = new Vector3(1, 1, 1);
    	mapOffset = Vector3.zero;
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
            sideBarMenuSelection.Hide(mapTransform);
        }
        
        enterTransform = null;
        pageRustleAudio.Play();
        slideTimer.turnOn();
        exitTransform = mapTransform;
        isActive = false;
        currentMapT.localScale = new Vector3(1, 1, 1);
        mapOffset = Vector3.zero;
    }

    public void ExitFocus() {
       sideBarMenuSelection.Display(UICurrentSelection.UI_MAP, true, null);
       isActive = false;
       currentMapT.localScale = new Vector3(1, 1, 1);
       mapOffset = Vector3.zero;
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

          float xAxisRight = Input.GetAxis("RightHorizontal");
          float yAxisRight = Input.GetAxis("RightVertical");

          float scale = Time.deltaTime*zoomSpeed;

          Debug.Log("x axis: " + xAxisRight);
          Debug.Log("y axis: " + yAxisRight);
          if(!slideTimer.isOn()) {
          	//Prevent Drift!!
          	if(Mathf.Abs(xAxisRight) < threshold) {
          		xAxisRight = 0;
          	}
          	if(Mathf.Abs(yAxisRight) < threshold) {
          		yAxisRight = 0;
          	}
          	mapOffset += scale*new Vector3(yAxisRight, xAxisRight, 0);

          }

          currentMapT.localPosition = mapOffset;

          
          bool leftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
          bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);
          bool upKeyDown = Input.GetKeyDown(KeyCode.UpArrow);
          bool downKeyDown = Input.GetKeyDown(KeyCode.DownArrow);
          
            if(!gotFocus) {
              if(xAxis < -threshold || leftKeyDown) {
                if(xIsNew || leftKeyDown) {
                  ExitFocus();
                  xIsNew = false;
                }
              } 
            }

            gotFocus = false;

          //Reset the stick values
          if(xAxis < lowerThreshold  && xAxis > -lowerThreshold) {
            xIsNew = true;
          }

          if(!slideTimer.isOn() && Input.GetButtonDown("Jump")) {
          	wasDownInMap = true;
          }

         
          if(Input.GetButton("Jump") && wasDownInMap) {
          	//zoom in
          	if(currentMapT.localScale.x < 2.5f) {
          		currentMapT.localScale = currentMapT.localScale + new Vector3(scale, scale, 0);
          	}
          	
          }

          if(Input.GetButton("Fire2")) { //Circle??
          	//zoom Out
          	if(currentMapT.localScale.x > 0.5f) {
          		currentMapT.localScale = currentMapT.localScale - new Vector3(scale, scale,  0);
          	}
          	
          }

          if(Input.GetButtonDown("Fire1")) {
              ExitJournal(true);
          }
      }

    }

    public void SetCurrentMap(MiniMapType t, BoxCollider2D b, Transform playerTransform) {
    	Assert.IsTrue(t < MiniMapType.MINI_MAP_COUNT);
    	sp.sprite = maps[(int)t];

    	Vector3 lp = playerTransform.position - b.bounds.min;
    	Vector3 bs =  b.bounds.size;
    	lp = new Vector3(lp.x / bs.x, lp.y / bs.y, 0); //uv coords

    	lp -= new Vector3(0.5f, 0.5f, 0);

    	Vector3 s = sp.sprite.bounds.size;

    	lp = new Vector3(lp.x*s.x, lp.y*s.y, 0);

    	playerIcon.localPosition = new Vector3(lp.x, lp.y, playerIcon.localPosition.z);
    }
}
