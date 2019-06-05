using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyGameManager;

public class ControlSkillTree : MonoBehaviour
{

	public Animator earthEmblem1;
	public Animator sceneAnimator;
	public SceneStateManager sceneManager;
	public LevelStateId lastLevelToLoad;
    public CameraFollowPlayer cam;
    public PlayerMovement playerMovement;
    public AudioSource selectSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1")) {
        	if (!GameManager.hasEarth1) {
	        	// if(GameManager.amberCount >= 120 && GameManager.senintelHeadCount >= 1) {
	        		GameManager.amberCount -= 120;
	        		GameManager.senintelHeadCount--;

	        		GameManager.hasEarth1 = true;
                    selectSound.Play();

	        		earthEmblem1.SetTrigger("FadeIn");
	        	// }
	        }
        }
        if(Input.GetButtonDown("Fire2")) {
            cam.moveDownTimer.turnOn();
            
            playerMovement.canControlPlayer = true;
            sceneManager.useSpawnPoint = false;
            sceneManager.stateToLoad = lastLevelToLoad;
            sceneAnimator.SetTrigger("FadeIn");
        }
        
    }
}
