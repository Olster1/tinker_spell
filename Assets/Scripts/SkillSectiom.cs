using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSectiom : MonoBehaviour
{

	public SceneStateManager sceneManager;
	private bool isActive;
	public GameObject uiHud;

	private int index;
	public int maxIndex;

	public Transform tTo;

	private float tAt;

	public float speed; 


	private LevelStateId lastLevelState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ExitSkillSection() {
		// slideTimer.turnOn();
		uiHud.SetActive(true);
		sceneManager.useSpawnPoint = false;
		sceneManager.ChangeSceneWithId(lastLevelState);
		// exitTransform = currentPage;
		isActive = false;
    }

    public void EnterSkillSection() {
    	uiHud.SetActive(false);
    	lastLevelState = sceneManager.stateToLoad;
		sceneManager.useSpawnPoint = false;
		sceneManager.ChangeSceneWithId(lastLevelState);
		// exitTransform = currentPage;
		isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if(isActive) {

        // 	bool leftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
        // 	bool rightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);

        // 	if(Input.GetButton("Fire1")) {
        // 		tAt += speed*Time.deltaTime;

        // 		if(tAt > 1.0f) {
        // 			tAt = 1.0f;
        // 		}

        // 		float yAxis = Mathf.Lerp(0, 1.36f, tAt);

        // 		Vector3 s = tAt.localScale;
        // 		s.y = yAxis;
        // 		tAt.localScale = s;


        // 	}

        // }
    }
}
